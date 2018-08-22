using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SeparateTreeNode
{
    /// <summary>
    /// 节点包围盒
    /// </summary>
    public Bounds Bounds
    {
        get { return mBounds; }
    }

    /// <summary>
    /// 节点当前深度
    /// </summary>
    public int Depth
    {
        get { return mDepth; }
    }

    /// <summary>
    /// 节点数据列表
    /// </summary>
    public LinkedList<IEntity> Entities
    {
        get { return mEntities; }
    }

    private int mDepth;

    private Bounds mBounds;

    private Vector3 mHalfSize;

    private LinkedList<IEntity> mEntities;

    protected SeparateTreeNode[] mChildNodes;

    public SeparateTreeNode(Bounds bounds, int depth, int childCount)
    {
        mBounds = bounds;
        mDepth = depth;
        mEntities = new LinkedList<IEntity>();
        mChildNodes = new SeparateTreeNode[childCount];

        if (childCount == 8)
            mHalfSize = new Vector3(mBounds.size.x / 2, mBounds.size.y / 2, mBounds.size.z / 2);
        else
            mHalfSize = new Vector3(mBounds.size.x / 2, mBounds.size.y, mBounds.size.z / 2);
    }

    public void Clear()
    {
        for (int i = 0; i < mChildNodes.Length; i++)
        {
            if (mChildNodes[i] != null)
                mChildNodes[i].Clear();
        }
        if (mEntities != null)
            mEntities.Clear();
    }

    public bool Contains(IEntity entity)
    {
        for (int i = 0; i < mChildNodes.Length; i++)
        {
            if (mChildNodes[i] != null && mChildNodes[i].Contains(entity))
                return true;
        }

        if (mEntities != null && mEntities.Contains(entity))
            return true;
        return false;
    }

    public SeparateTreeNode Insert(IEntity entity, int depth, int maxDepth)
    {
        if (mEntities.Contains(entity))
            return this;
        if (depth < maxDepth)
        {
            SeparateTreeNode node = GetContainerNode(entity, depth);
            if (node != null)
                return node.Insert(entity, depth + 1, maxDepth);
        }
        mEntities.AddFirst(entity);

        entity.Node = this;

        return this;
    }

    public bool Remove(IEntity entity)
    { 
        if (mEntities != null && mEntities.Contains(entity))
        {
           return mEntities.Remove(entity);
        }
        else
        {
            for (int i = 0; i < mChildNodes.Length; i++)
            {
                if (mChildNodes[i] != null)
                {
                    if(mChildNodes[i].Remove(entity))
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

        for (int i = 0; i < mChildNodes.Length; i++)
        {
            var node = mChildNodes[i];
            if (node != null)
                node.Trigger(detector, handle);
        }

        if (detector.IsDetected(mBounds))
        {
            var node = mEntities.First;
            while (node != null)
            {
                if (detector.IsDetected(node.Value.Bounds))
                    handle(node.Value);
                node = node.Next;
            }
        }
    }

    protected SeparateTreeNode GetContainerNode(IEntity entity, int depth)
    {
        SeparateTreeNode result = null;
        int ix = -1;
        int iz = -1;
        int iy = mChildNodes.Length == 4 ? 0 : -1;

        int nodeIndex = 0;
        for (int i = ix; i <= 1; i += 2) //i = -1, 1
        {
            for (int j = iz; j <= 1; j += 2) //j = -1, 1
            {
                for (int k = iy; k <= 1; k += 2) //k = 4 or -1,1
                {
                    result = CreateNode(ref mChildNodes[nodeIndex], depth, mBounds.center + new Vector3(i* mHalfSize.x / 2, k*mHalfSize.y/2, j* mHalfSize.z / 2),
            mHalfSize, entity);
                    if (result != null)
                        return result;
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
            if (bounds.IsBoundsContainsAnotherBounds(entity.Bounds))
            {
                SeparateTreeNode newNode = new SeparateTreeNode(bounds, depth + 1, mChildNodes.Length);
                node = newNode;
                result = node;
            }
        }
        else if (node.Bounds.IsBoundsContainsAnotherBounds(entity.Bounds))
        {
            result = node;
        }
        return result;
    }

#if UNITY_EDITOR
    public void DrawNode(Color treeMinDepthColor, Color treeMaxDepthColor, Color objColor, Color hitObjColor, int drawMinDepth, int drawMaxDepth, bool drawEntity, int maxDepth)
    {
        if (mChildNodes != null)
        {
            for (int i = 0; i < mChildNodes.Length; i++)
            {
                var node = mChildNodes[i];
                if (node != null)
                    node.DrawNode(treeMinDepthColor, treeMaxDepthColor, objColor, hitObjColor, drawMinDepth, drawMaxDepth, drawEntity, maxDepth);
            }
        }

        if (mDepth >= drawMinDepth && mDepth <= drawMaxDepth)
        {
            float d = ((float)mDepth) / maxDepth;
            Color color = Color.Lerp(treeMinDepthColor, treeMaxDepthColor, d);

            mBounds.DrawBounds(color);
        }
        if (drawEntity)
        {
            var node = mEntities.First;
            while (node != null)
            {
                var sceneobj = node.Value as SeparateEntity;
                if (sceneobj != null)
                    sceneobj.DrawEntity(objColor, hitObjColor);
                node = node.Next;
            }  
        }

    }

#endif
}
