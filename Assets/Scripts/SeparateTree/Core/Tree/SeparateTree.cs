using UnityEngine;
using System.Collections;

public delegate void TriggerHandler<T>(T trigger);

public enum SeparateTreeType
{
    OcTree,
    QuadTree,
}

public class SeparateTree<T> where T : ISeparateEntity, ILinkedListNode
{

    public Bounds Bounds
    {
        get
        {
            if (mRoot != null)
                return mRoot.Bounds;
            return default(Bounds);
        }
    }

    public int MaxDepth
    {
        get { return mMaxDepth; }
    }

    public SeparateTreeType CurrentTreeType { get { return mTreeType; } }

    /// <summary>
    /// 根节点
    /// </summary>
    private SeparateTreeNode<T> mRoot;

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
        this.mTreeType = treeType;
        this.mMaxDepth = maxDepth;
        if (treeType == SeparateTreeType.QuadTree)
            this.mRoot = new SeparateTreeNode<T>(new Bounds(center, size), 0, 4);
        else
            this.mRoot = new SeparateTreeNode<T>(new Bounds(center, size), 0, 8);
    }

    public void Add(T item)
    {
        mRoot.Insert(item, 0, mMaxDepth);
    }

    public void Clear()
    {
        mRoot.Clear();
    }

    public bool Contains(T item)
    {
        return mRoot.Contains(item);
    }

    public void Remove(T item)
    {
         mRoot.Remove(item);
    }

    public void Trigger(IDetector detector, TriggerHandler<T> handle)
    {
        if (handle == null)
            return;
        mRoot.Trigger(detector, handle);
    }

    public static implicit operator bool(SeparateTree<T> tree)
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
