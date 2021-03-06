﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 场景物体接口：需要插入到场景四叉树并实现动态显示与隐藏的物体实现该接口
/// </summary>
public interface IEntity
{
    /// <summary>
    /// 该物体的包围盒
    /// </summary>
    Bounds bounds { get; }

    /// <summary>
    /// 该物体所在的节点
    /// </summary>
    SeparateTreeNode node { get; set; }

    /// <summary>
    /// 该物体进入显示区域时调用（在这里处理物体的加载或显示）
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    bool OnShow();

    /// <summary>
    /// 该物体离开显示区域时调用（在这里处理物体的卸载或隐藏）
    /// </summary>
    void OnHide();

}