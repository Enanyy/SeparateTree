using UnityEngine;
using System.Collections;

/// <summary>
/// 该触发器根据Transform的包围盒区域触发
/// </summary>
public class TransformDetector : SeparateDetector
{
    public Vector3 detectorSize;

    private Bounds mBounds;

    public override bool IsDetected(Bounds bounds)
    {
        mBounds.center = Position;
        mBounds.size = detectorSize;
        return bounds.Intersects(mBounds);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Bounds b = new Bounds(transform.position, detectorSize);
        b.DrawBounds(Color.yellow);
    }
#endif
}
