using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 场景物件（用来包装实际用于动态加载的物体）
/// </summary>
public class SeparateEntity : ISeparateEntity, ILinkedListNode
{
    /// <summary>
    /// 场景物件创建标记
    /// </summary>
    public enum CreateFlag
    {
        /// <summary>
        /// 未创建
        /// </summary>
        None,
        /// <summary>
        /// 标记为新物体
        /// </summary>
        New,
        /// <summary>
        /// 标记为旧物体
        /// </summary>
        Old,
        /// <summary>
        /// 标记为离开视野区域
        /// </summary>
        OutofBounds,
    }

    /// <summary>
    /// 场景物体加载标记
    /// </summary>
    public enum CreatingProcessFlag
    {
        None,
        /// <summary>
        /// 准备加载
        /// </summary>
        IsPrepareCreate,
        /// <summary>
        /// 准备销毁
        /// </summary>
        IsPrepareDestroy,
    }

    /// <summary>
    /// 场景物体包围盒
    /// </summary>
    public Bounds Bounds
    {
        get { return mTargetEntity.Bounds; }
    }

    public float Weight
    {
        get { return mWeight; }
        set { mWeight = value; }
    }

    /// <summary>
    /// 被包装的实际用于动态加载和销毁的场景物体
    /// </summary>
    public ISeparateEntity TargetEntity
    {
        get { return mTargetEntity; }
    }

    public CreateFlag Flag { get; set; }

    public CreatingProcessFlag ProcessFlag { get; set; }

    private ISeparateEntity mTargetEntity;

    private float mWeight;

    private object mNode;

    public SeparateEntity(ISeparateEntity entity)
    {
        mWeight = 0;
        mTargetEntity = entity;
    }

    public LinkedListNode<T> GetLinkedListNode<T>() where T : ISeparateEntity
    {
        return (LinkedListNode<T>)mNode;
    }

    public void SetLinkedListNode<T>(LinkedListNode<T> node)
    {
        mNode = node;
    }

    public void OnHide()
    {
        Weight = 0;
        mTargetEntity.OnHide();
    }

    public bool OnShow(Transform parent)
    {
        return mTargetEntity.OnShow(parent);
    }

#if UNITY_EDITOR
    public void DrawArea(Color color, Color hitColor)
    {
        if (Flag == CreateFlag.New || Flag == CreateFlag.Old)
        {
            mTargetEntity.Bounds.DrawBounds(hitColor);
        }
        else 
        mTargetEntity.Bounds.DrawBounds(color);
    }
#endif
}
