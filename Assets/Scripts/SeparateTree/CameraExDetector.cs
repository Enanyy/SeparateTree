using UnityEngine;
using System.Collections;

/// <summary>
/// 该触发器根据相机裁剪区域触发，且根据相机运动趋势改变裁剪区域
/// </summary>
public class CameraExDetector : SeparateDetector
{
    #region 裁剪区域扩展趋势参数
    /// <summary>
    /// 水平方向扩展距离，当相机往左右移动时的裁剪区域扩展
    /// </summary>
    public float horizontalExtDis;
    /// <summary>
    /// 顶部方向扩展距离，当相机往前移动时的裁剪区域扩展
    /// </summary>
    public float topExtDis;
    /// <summary>
    /// 底部方向扩展距离，当相机往后移动时的裁剪区域扩展
    /// </summary>
    public float bottomExtDis;
    #endregion

    private Camera mCamera;
    private Vector3 mPosition;

    private float mLeftEx;
    private float mRightEx;
    private float mUpEx;
    private float mDownEx;

    void Start()
    {
        mCamera = gameObject.GetComponent<Camera>();
        mPosition = transform.position;
    }

    void Update()
    {
        Vector3 movedir = -transform.worldToLocalMatrix.MultiplyPoint(mPosition);
        mPosition = transform.position;
            
        mLeftEx = movedir.x < -Mathf.Epsilon ? -horizontalExtDis : 0;
        mRightEx = movedir.x > Mathf.Epsilon ? horizontalExtDis : 0;
        mUpEx = movedir.y > Mathf.Epsilon ? topExtDis : 0;
        mDownEx = movedir.y < -Mathf.Epsilon ? -bottomExtDis : 0;
    }

    public override bool IsDetected(Bounds bounds)
    {
        if (mCamera == null)
            return false;
        
        return bounds.IsBoundsInCameraEx(mCamera, mLeftEx, mRightEx, mDownEx, mUpEx);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Camera camera = gameObject.GetComponent<Camera>();
        if (camera)
            GizmosEx.DrawViewFrustumEx(camera, mLeftEx, mRightEx, mDownEx, mUpEx, Color.yellow);
    }
#endif
}
