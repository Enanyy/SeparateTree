using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 场景物件加载控制器
/// </summary>
public class SeparateEntityController : MonoBehaviour
{

    private WaitForEndOfFrame mWaitForFrame;

    /// <summary>
    /// 当前场景资源四叉树
    /// </summary>
    private SeparateTree mSeparateTree;

    /// <summary>
    /// 刷新时间
    /// </summary>
    private float mRefreshTime;
    /// <summary>
    /// 销毁时间
    /// </summary>
    private float mDestroyRefreshTime;
    
    private Vector3 mOldRefreshPosition;
    private Vector3 mOldDestroyRefreshPosition;

    /// <summary>
    /// 异步任务队列
    /// </summary>
    private Queue<SeparateEntity> mProcessTaskQueue;

    /// <summary>
    /// 已加载的物体列表
    /// </summary>
    private List<SeparateEntity> mLoadedEntityList;

    /// <summary>
    /// 待销毁物体列表
    /// </summary>
    private PriorityQueue<SeparateEntity> mPreDestroyQueue;

    private bool mIsTaskRunning;

    private bool mIsInitialized;

    private int mMaxCreateCount;
    private int mMinCreateCount;
    private float mMaxRefreshTime;
    private float mMaxDestroyTime;
    private bool mIsAsyn;

    private IDetector mDetector;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="center">场景区域中心</param>
    /// <param name="size">场景区域大小</param>
    /// <param name="asyn">是否异步</param>
    /// <param name="maxCreateCount">最大创建数量</param>
    /// <param name="minCreateCount">最小创建数量</param>
    /// <param name="maxRefreshTime">更新区域时间间隔</param>
    /// <param name="maxDestroyTime">检查销毁时间间隔</param>
    /// <param name="quadTreeDepth">四叉树深度</param>
    public void Init(Vector3 center, Vector3 size, bool asyn, int maxCreateCount, int minCreateCount, float maxRefreshTime, float maxDestroyTime, SeparateTreeType treeType , int quadTreeDepth = 5)
    {
        if (mIsInitialized)
            return;
        quadTreeDepth = Mathf.Clamp(quadTreeDepth, 1, 6);
        mSeparateTree = new SeparateTree(treeType, center, size, quadTreeDepth);
        mLoadedEntityList = new List<SeparateEntity>();
        mPreDestroyQueue = new PriorityQueue<SeparateEntity>(new SeparateEntityWeightComparer());

        mMaxCreateCount = Mathf.Max(0, maxCreateCount);
        mMinCreateCount = Mathf.Clamp(minCreateCount, 0, mMaxCreateCount);
        mMaxRefreshTime = maxRefreshTime;
        mMaxDestroyTime = maxDestroyTime;
        mIsAsyn = asyn;

        mIsInitialized = true;

        mRefreshTime = maxRefreshTime;
    }

  

    void OnDestroy()
    {
        if (mSeparateTree)
            mSeparateTree.Clear();
        mSeparateTree = null;
        if (mProcessTaskQueue != null)
            mProcessTaskQueue.Clear();
        if (mLoadedEntityList != null)
            mLoadedEntityList.Clear();
        mProcessTaskQueue = null;
        mLoadedEntityList = null;
    }

    /// <summary>
    /// 添加场景物体
    /// </summary>
    /// <param name="obj"></param>
    public void AddSceneEntity(IEntity entity)
    {
        if (!mIsInitialized)
        {
            return;
        }
        if (mSeparateTree == null)
        {
            return;
        }
        if (entity == null)
        {
            return;
        }
        //使用SeparateEntity包装
        SeparateEntity se = new SeparateEntity(entity);
        mSeparateTree.Add(se);
        //如果当前触发器存在，直接物体是否可触发，如果可触发，则创建物体
        if (mDetector != null && mDetector.IsDetected(se.bounds))
        {
            CreateInternal(se);
        }
    }

    /// <summary>
    /// 刷新触发器
    /// </summary>
    /// <param name="detector">触发器</param>
    public void RefreshDetector(IDetector detector)
    {
        if (!mIsInitialized)
            return;
        //只有坐标发生改变才调用
        if (mOldRefreshPosition != detector.position)
        {
            mRefreshTime += Time.deltaTime;
            //达到刷新时间才刷新，避免区域更新频繁
            if (mRefreshTime > mMaxRefreshTime)
            {
                mOldRefreshPosition = detector.position;
                mRefreshTime = 0;
                mDetector = detector;
                //进行触发检测
                mSeparateTree.Trigger(detector, OnTrigger);
                //标记超出区域的物体
                MarkOutofBoundsEntity();
                //m_IsInitLoadComplete = true;
            }
        }
        if (mOldDestroyRefreshPosition != detector.position)
        {
            if(mPreDestroyQueue != null && mPreDestroyQueue.Count >= mMaxCreateCount && mPreDestroyQueue.Count > mMinCreateCount)
            //if (m_PreDestroyObjectList != null && m_PreDestroyObjectList.Count >= m_MaxCreateCount)
            {
                mDestroyRefreshTime += Time.deltaTime;
                if (mDestroyRefreshTime > mMaxDestroyTime)
                {
                    mOldDestroyRefreshPosition = detector.position;
                    mDestroyRefreshTime = 0;
                    //删除超出区域的物体
                    DestroyOutOfBoundsEntity();
                }
            }
        }
    }

    /// <summary>
    /// 四叉树触发处理函数
    /// </summary>
    /// <param name="data">与当前包围盒发生触发的场景物体</param>
    void OnTrigger(IEntity data)
    {
        if (data == null)
        {
            return;
        }
        SeparateEntity entity = data as SeparateEntity;

        if (entity.createFlag == SeparateEntity.CreateFlag.Old) //如果发生触发的物体已经被创建则标记为新物体，以确保不会被删掉
        {
            entity.weight ++;
            entity.createFlag = SeparateEntity.CreateFlag.New;
        }
        else if (entity.createFlag == SeparateEntity.CreateFlag.OutofBounds)//如果发生触发的物体已经被标记为超出区域，则从待删除列表移除该物体，并标记为新物体
        {
            entity.createFlag = SeparateEntity.CreateFlag.New;
           
            mLoadedEntityList.Add(entity);
            
        }
        else if (entity.createFlag == SeparateEntity.CreateFlag.None) //如果发生触发的物体未创建则创建该物体并加入已加载的物体列表
        {
            CreateInternal(entity);
        }
    }

    //执行创建物体
    private void CreateInternal(SeparateEntity data)
    {
        //加入已加载列表
        mLoadedEntityList.Add(data);
        //创建物体
        CreateEntity(data, mIsAsyn);
    }

    /// <summary>
    /// 标记离开视野的物体
    /// </summary>
    void MarkOutofBoundsEntity()
    {
        if (mLoadedEntityList == null)
            return;
        int i = 0;
        while (i < mLoadedEntityList.Count)
        {
            if (mLoadedEntityList[i].createFlag == SeparateEntity.CreateFlag.Old)//已加载物体标记仍然为Old，说明该物体没有进入触发区域，即该物体在区域外
            {
                mLoadedEntityList[i].createFlag = SeparateEntity.CreateFlag.OutofBounds;
               
                if (mMinCreateCount == 0)//如果最小创建数为0直接删除
                {
                    DestroyEntity(mLoadedEntityList[i], mIsAsyn);
                }
                else
                {
                   
                    mPreDestroyQueue.Push(mLoadedEntityList[i]);//加入待删除队列
                }
                mLoadedEntityList.RemoveAt(i);

            }
            else
            {
                mLoadedEntityList[i].createFlag = SeparateEntity.CreateFlag.Old;//其它物体标记为旧
                i++;
            }
        }
    }

    /// <summary>
    /// 删除超出区域外的物体
    /// </summary>
    void DestroyOutOfBoundsEntity()
    {
        while(mPreDestroyQueue.Count>mMinCreateCount)
        {

           
            var entity = mPreDestroyQueue.Pop();
            if (entity == null)
                continue;
            if (entity.createFlag == SeparateEntity.CreateFlag.OutofBounds)
            {
                DestroyEntity(entity, mIsAsyn);
            }
        }
    }

    /// <summary>
    /// 创建物体
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="asyn"></param>
    private void CreateEntity(SeparateEntity entity, bool asyn)
    {
        if (entity == null)
            return;
        if (entity.targetEntity == null)
            return;
        if (entity.createFlag == SeparateEntity.CreateFlag.None)
        {
            if (!asyn)
                CreateEntitySync(entity);
            else
                ProcessEntityAsyn(entity, true);
            entity.createFlag = SeparateEntity.CreateFlag.New;//被创建的物体标记为New
        }
    }

    /// <summary>
    /// 删除物体
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="asyn"></param>
    private void DestroyEntity(SeparateEntity entity, bool asyn)
    {
        if (entity == null)
            return;
        if (entity.createFlag == SeparateEntity.CreateFlag.None)
            return;
        if (entity.targetEntity == null)
            return;
        if (!asyn)
            DestroyEntitySync(entity);
        else
            ProcessEntityAsyn(entity, false);
        entity.createFlag = SeparateEntity.CreateFlag.None;//被删除的物体标记为None
    }

    /// <summary>
    /// 同步方式创建物体
    /// </summary>
    /// <param name="obj"></param>
    private void CreateEntitySync(SeparateEntity entity)
    {
        if (entity.processFlag == SeparateEntity.CreatingProcessFlag.IsPrepareDestroy)//如果标记为IsPrepareDestroy表示物体已经创建并正在等待删除，则直接设为None并返回
        {
            entity.processFlag = SeparateEntity.CreatingProcessFlag.None;
            return;
        }
        entity.OnShow();//执行OnShow
    }

    /// <summary>
    /// 同步方式销毁物体
    /// </summary>
    /// <param name="obj"></param>
    private void DestroyEntitySync(SeparateEntity entity)
    {
        if (entity.processFlag == SeparateEntity.CreatingProcessFlag.IsPrepareCreate)//如果物体标记为IsPrepareCreate表示物体未创建并正在等待创建，则直接设为None并放回
        {
            entity.processFlag = SeparateEntity.CreatingProcessFlag.None;
            return;
        }
        entity.OnHide();//执行OnHide
    }

    /// <summary>
    /// 异步处理
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="create"></param>
    private void ProcessEntityAsyn(SeparateEntity entity, bool create)
    {
        if (create)
        {
            if (entity.processFlag == SeparateEntity.CreatingProcessFlag.IsPrepareDestroy)//表示物体已经创建并等待销毁，则设置为None并跳过
            {
                entity.processFlag = SeparateEntity.CreatingProcessFlag.None;
                return;
            }
            if (entity.processFlag == SeparateEntity.CreatingProcessFlag.IsPrepareCreate)//已经开始等待创建，则跳过
                return;
            entity.processFlag = SeparateEntity.CreatingProcessFlag.IsPrepareCreate;//设置为等待开始创建
        }
        else
        {
            if (entity.processFlag == SeparateEntity.CreatingProcessFlag.IsPrepareCreate)//表示物体未创建并等待创建，则设置为None并跳过
            {
                entity.processFlag = SeparateEntity.CreatingProcessFlag.None;
                return;
            }
            if (entity.processFlag == SeparateEntity.CreatingProcessFlag.IsPrepareDestroy)//已经开始等待销毁，则跳过
                return;
            entity.processFlag = SeparateEntity.CreatingProcessFlag.IsPrepareDestroy;//设置为等待开始销毁
        }
        if (mProcessTaskQueue == null)
            mProcessTaskQueue = new Queue<SeparateEntity>();
        mProcessTaskQueue.Enqueue(entity);//加入
        if (!mIsTaskRunning)
        {
            StartCoroutine(AsynTaskProcess());//开始协程执行异步任务
        }
    }

    /// <summary>
    /// 异步任务
    /// </summary>
    /// <returns></returns>
    private IEnumerator AsynTaskProcess()
    {
        if (mProcessTaskQueue == null)
            yield return 0;
        mIsTaskRunning = true;
        while (mProcessTaskQueue.Count > 0)
        {
            var entity = mProcessTaskQueue.Dequeue();
            if (entity != null)
            {
                if (entity.processFlag == SeparateEntity.CreatingProcessFlag.IsPrepareCreate)//等待创建
                {
                    entity.processFlag = SeparateEntity.CreatingProcessFlag.None;
                    if (entity.OnShow())
                    {
                        if (mWaitForFrame == null)
                            mWaitForFrame = new WaitForEndOfFrame();
                        yield return mWaitForFrame;
                    }
                }
                else if (entity.processFlag == SeparateEntity.CreatingProcessFlag.IsPrepareDestroy)//等待销毁
                {
                    entity.processFlag = SeparateEntity.CreatingProcessFlag.None;
                    entity.OnHide();
                    if (mWaitForFrame == null)
                        mWaitForFrame = new WaitForEndOfFrame();
                    yield return mWaitForFrame;
                }
            }
        }
        mIsTaskRunning = false;
    }

    private class SeparateEntityWeightComparer : IComparer<SeparateEntity>
    {

        public int Compare(SeparateEntity x, SeparateEntity y)
        {
            if (y.weight < x.weight)
                return 1;
            else if (y.weight == x.weight)
                return 0;
            return -1;
        }
    }

#if UNITY_EDITOR
    public int debug_DrawMinDepth = 0;
    public int debug_DrawMaxDepth = 5;
    public bool debug_DrawEntity = true;
    void OnDrawGizmosSelected()
    {
        Color mindcolor = new Color32(0, 66, 255, 255);
        Color maxdcolor = new Color32(133, 165, 255, 255);
        Color objcolor = new Color32(0, 210, 255, 255);
        Color hitcolor = new Color32(255, 216, 0, 255);
        if (mSeparateTree != null)
            mSeparateTree.DrawTree(mindcolor, maxdcolor, objcolor, hitcolor, debug_DrawMinDepth, debug_DrawMaxDepth, debug_DrawEntity);
    }
#endif
}
