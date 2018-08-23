using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SeparateTreeNode
{
    /// <summary>
    /// 节点包围盒
    /// </summary>
    public Bounds bounds
    {
        get { return mBounds; }
    }

    /// <summary>
    /// 节点当前深度
    /// </summary>
    public int depth
    {
        get { return mDepth; }
    }

    /// <summary>
    /// 节点数据列表
    /// </summary>
    public LinkedList<IEntity> entities
    {
        get { return mEntities; }
    }

    /// <summary>
    /// 子节点，可能为空
    /// </summary>
    public SeparateTreeNode[] children { get { return mChildren; } }

    private int mDepth;

    private Bounds mBounds;

    private LinkedList<IEntity> mEntities;

    /// <summary>
    /// 子节点个数
    /// </summary>
    private int mChildCount = 0;
    protected SeparateTreeNode[] mChildren;

    public SeparateTreeNode(Bounds bounds, int depth, int childCount)
    {
        mBounds = bounds;
        mDepth = depth;
        mChildCount = childCount;
        mEntities = new LinkedList<IEntity>();
    }

    public void Clear()
    {
        if (mChildren != null)
        {
            for (int i = 0; i < mChildren.Length; i++)
            {
                if (mChildren[i] != null)
                {
                    mChildren[i].Clear();
                }
            }
        }

        if (mEntities != null)
        {
            mEntities.Clear();
        }
    }

    public bool Contains(IEntity entity)
    {   
        if (mChildren != null)
        {
            for (int i = 0; i < mChildren.Length; i++)
            {
                if (mChildren[i] != null && mChildren[i].Contains(entity))
                    return true;
            }
        }
        if (mEntities != null && mEntities.Contains(entity))
        {
            return true;
        }
        return false;
    }

    public SeparateTreeNode Insert(IEntity entity, int depth, int maxDepth)
    {
        if (mEntities.Contains(entity))
        {
            return this;
        }

        if (depth < maxDepth)
        {
            SeparateTreeNode node = GetContainerNode(entity, depth);
            if (node != null)
            {
                return node.Insert(entity, depth + 1, maxDepth);
            }
        }
        mEntities.AddFirst(entity);

        entity.node = this;

        return this;
    }

    public bool Remove(IEntity entity)
    { 
        if (mEntities != null && mEntities.Contains(entity))
        {
           return mEntities.Remove(entity);
        }
        else if(mChildren!=null)
        {
            for (int i = 0; i < mChildren.Length; i++)
            {
                if (mChildren[i] != null)
                {
                    if(mChildren[i].Remove(entity))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void Trigger(IDetector detector, TriggerHandler handle)
    {
        if (handle == null)
            return;

        if (mChildren != null)
        {
            for (int i = 0; i < mChildren.Length; i++)
            {
                var node = mChildren[i];
                if (node != null)
                {
                    node.Trigger(detector, handle);
                }
            }
        }

        if (detector.IsDetected(mBounds))
        {
            var node = mEntities.First;
            while (node != null)
            {
                if (detector.IsDetected(node.Value.bounds))
                {
                    handle(node.Value);
                }

                node = node.Next;
            }
        }
    }

    protected SeparateTreeNode GetContainerNode(IEntity entity, int depth)
    {
        SeparateTreeNode result = null;
        int ix = -1;
        int iz = -1;

        int iy = mChildCount == 4 ? 0 : -1;

        if(mChildren == null)
        {
            mChildren = new SeparateTreeNode[mChildCount];
        }

        int nodeIndex = 0;

        Vector3 halfSize = halfSize = new Vector3(mBounds.size.x / 2, mChildCount == 4? mBounds.size.y : mBounds.size.y / 2, mBounds.size.z / 2);
        
        for (int i = ix; i <= 1; i += 2) //i = -1, 1
        {
            for (int j = iz; j <= 1; j += 2) //j = -1, 1
            {
                for (int k = iy; k <= 1; k += 2) //k = 4 or -1,1
                {
                    result = CreateNode(ref mChildren[nodeIndex],
                                        depth,
                                        mBounds.center + new Vector3(i * halfSize.x / 2, k * halfSize.y / 2, j * halfSize.z / 2),
                                        halfSize,
                                        entity);

                    if (result != null)
                    {
                        return result;
                    }

                    nodeIndex += 1;
                }
            }
        }
        return null;
    }

    protected SeparateTreeNode CreateNode(ref SeparateTreeNode node, int depth, Vector3 center, Vector3 size, IEntity entity) 
    {
        SeparateTreeNode result = null;
        if (node == null)
        {
            Bounds bounds = new Bounds(center, size);
            if (bounds.IsBoundsContainsAnotherBounds(entity.bounds))
            {
                SeparateTreeNode newNode = new SeparateTreeNode(bounds, depth + 1, mChildCount);
                node = newNode;
                result = node;
            }
        }
        else if (node.bounds.IsBoundsContainsAnotherBounds(entity.bounds))
        {
            result = node;
        }
        return result;
    }

#if UNITY_EDITOR
    public void DrawNode(Color treeMinDepthColor, Color treeMaxDepthColor, Color objColor, Color hitObjColor, int drawMinDepth, int drawMaxDepth, bool drawEntity, int maxDepth)
    {
        if (mChildren != null)
        {
            for (int i = 0; i < mChildren.Length; i++)
            {
                var node = mChildren[i];
                if (node != null)
                    node.DrawNode(treeMinDepthColor, treeMaxDepthColor, objColor, hitObjColor, drawMinDepth, drawMaxDepth, drawEntity, maxDepth);
            }
        }

        if (mDepth >= drawMinDepth && mDepth <= drawMaxDepth)
        {
            float d = ((float)mDepth) / maxDepth;
            Color color = Color.Lerp(treeMinDepthColor, treeMaxDepthColor, d);

            mBounds.DrawEx(color);
        }
        if (drawEntity)
        {
            var node = mEntities.First;
            while (node != null)
            {
                var entity = node.Value ;
                if (entity != null)
                {
                    entity.bounds.DrawEx(objColor);
                }
                node = node.Next;
            }  
        }

    }

#endif
}
