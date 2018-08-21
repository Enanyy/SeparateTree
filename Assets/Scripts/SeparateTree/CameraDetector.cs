using UnityEngine;
using System.Collections;

/// <summary>
/// 该触发器根据相机裁剪区域触发
/// </summary>
public class CameraDetector : SeparateDetector
{
    private Camera mCamera;

    void Start()
    {
        mCamera = gameObject.GetComponent<Camera>();
    }

    public override bool IsDetected(Bounds bounds)
    {
        if (mCamera == null)
            return false;
        return bounds.IsBoundsInCamera(mCamera);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Camera camera = gameObject.GetComponent<Camera>();
        if (camera)
            GizmosEx.DrawViewFrustum(camera, Color.yellow);
    }
#endif
}
