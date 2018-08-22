using UnityEngine;
using System.Collections;

public delegate void TriggerHandler(IEntity trigger);

public enum SeparateTreeType
{
    OcTree, //八叉树
    QuadTree, //四叉树
}

public class SeparateTree
{

    public Bounds bounds
    {
        get
        {
            if (mRoot != null)
                return mRoot.bounds;
            return default(Bounds);
        }
    }

    public int maxDepth
    {
        get { return mMaxDepth; }
    }

    public SeparateTreeType treeType { get { return mTreeType; } }

    /// <summary>
    /// 根节点
    /// </summary>
    private SeparateTreeNode mRoot;

    /// <summary>
    /// 最大深度
    /// </summary>
    private int mMaxDepth;

    private SeparateTreeType mTreeType;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="treeType">树类型</param>
    /// <param name="center">树中心</param>
    /// <param name="size">树区域大小</param>
    /// <param name="maxDepth">树最大深度</param>
    public SeparateTree(SeparateTreeType treeType, Vector3 center, Vector3 size, int maxDepth)
    {
        mTreeType = treeType;
        mMaxDepth = maxDepth;

        mRoot = new SeparateTreeNode(new Bounds(center, size), 0, mTreeType == SeparateTreeType.QuadTree ? 4 : 8);
    }

    public void Add(IEntity entity)
    {
        mRoot.Insert(entity, 0, mMaxDepth);
    }

    public void Clear()
    {
        mRoot.Clear();
    }

    public bool Contains(IEntity entity)
    {
        return mRoot.Contains(entity);
    }

    public void Remove(IEntity entity)
    {
         mRoot.Remove(entity);
    }

    public void Trigger(IDetector detector, TriggerHandler handle)
    {
        if (handle == null)
            return;
        mRoot.Trigger(detector, handle);
    }

    public static implicit operator bool(SeparateTree tree)
    {
        return tree != null;
    }

#if UNITY_EDITOR
    public void DrawTree(Color treeMinDepthColor, Color treeMaxDepthColor, Color objColor, Color hitObjColor, int drawMinDepth, int drawMaxDepth, bool drawObj)
    {
        if (mRoot != null)
            mRoot.DrawNode(treeMinDepthColor, treeMaxDepthColor, objColor, hitObjColor, drawMinDepth, drawMaxDepth, drawObj, mMaxDepth);
    }
#endif
}
