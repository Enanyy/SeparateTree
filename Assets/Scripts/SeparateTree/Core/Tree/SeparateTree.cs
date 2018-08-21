using UnityEngine;
using System.Collections;

public delegate void TriggerHandle<T>(T trigger);

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
            if (m_Root != null)
                return m_Root.Bounds;
            return default(Bounds);
        }
    }

    public int MaxDepth
    {
        get { return m_MaxDepth; }
    }

    public SeparateTreeType CurrentTreeType { get { return m_TreeType; } }

    /// <summary>
    /// 根节点
    /// </summary>
    private SeparateTreeNode<T> m_Root;

    /// <summary>
    /// 最大深度
    /// </summary>
    private int m_MaxDepth;

    private SeparateTreeType m_TreeType;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="treeType">树类型</param>
    /// <param name="center">树中心</param>
    /// <param name="size">树区域大小</param>
    /// <param name="maxDepth">树最大深度</param>
    public SeparateTree(SeparateTreeType treeType, Vector3 center, Vector3 size, int maxDepth)
    {
        this.m_TreeType = treeType;
        this.m_MaxDepth = maxDepth;
        if (treeType == SeparateTreeType.QuadTree)
            this.m_Root = new SeparateTreeNode<T>(new Bounds(center, size), 0, 4);
        else
            this.m_Root = new SeparateTreeNode<T>(new Bounds(center, size), 0, 8);
    }

    public void Add(T item)
    {
        m_Root.Insert(item, 0, m_MaxDepth);
    }

    public void Clear()
    {
        m_Root.Clear();
    }

    public bool Contains(T item)
    {
        return m_Root.Contains(item);
    }

    public void Remove(T item)
    {
         m_Root.Remove(item);
    }

    public void Trigger(IDetector detector, TriggerHandle<T> handle)
    {
        if (handle == null)
            return;
        m_Root.Trigger(detector, handle);
    }

    public static implicit operator bool(SeparateTree<T> tree)
    {
        return tree != null;
    }

#if UNITY_EDITOR
    public void DrawTree(Color treeMinDepthColor, Color treeMaxDepthColor, Color objColor, Color hitObjColor, int drawMinDepth, int drawMaxDepth, bool drawObj)
    {
        if (m_Root != null)
            m_Root.DrawNode(treeMinDepthColor, treeMaxDepthColor, objColor, hitObjColor, drawMinDepth, drawMaxDepth, drawObj, m_MaxDepth);
    }
#endif
}
