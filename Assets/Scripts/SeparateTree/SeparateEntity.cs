using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 场景物件（用来包装实际用于动态加载的物体）
/// </summary>
public class SeparateEntity : IEntity
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
    public Bounds bounds
    {
        get { return mTargetEntity.bounds; }
    }

    public SeparateTreeNode node { get; set; }

    public float weight
    {
        get { return mWeight; }
        set { mWeight = value; }
    }

    /// <summary>
    /// 被包装的实际用于动态加载和销毁的场景物体
    /// </summary>
    public IEntity targetEntity
    {
        get { return mTargetEntity; }
    }

    public CreateFlag createFlag { get; set; }

    public CreatingProcessFlag processFlag { get; set; }

    private IEntity mTargetEntity;

    private float mWeight;

    public SeparateEntity(IEntity entity)
    {
        mWeight = 0;
        mTargetEntity = entity;
    }

   
    public void OnHide()
    {
        weight = 0;
        mTargetEntity.OnHide();
    }

    public bool OnShow()
    {
        return mTargetEntity.OnShow();
    }

#if UNITY_EDITOR
    public void DrawEntity(Color color, Color hitColor)
    {
        if (createFlag == CreateFlag.New || createFlag == CreateFlag.Old)
        {
            mTargetEntity.bounds.DrawBounds(hitColor);
        }
        else 
        mTargetEntity.bounds.DrawBounds(color);
    }
#endif
}
