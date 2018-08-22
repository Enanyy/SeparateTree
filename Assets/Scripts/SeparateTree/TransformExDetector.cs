using UnityEngine;
using System.Collections;

/// <summary>
/// 该触发器根据Transform的包围盒区域触发-且根据Transform运动趋势扩展包围盒
/// </summary>
public class TransformExDetector : SeparateDetector
{
    public Vector3 detectorSize;

    #region 包围盒扩展趋势参数

    public float leftExtDis;
    public float rightExtDis;
    public float topExtDis;
    public float bottomExtDis;
    #endregion

    private Vector3 mPosition;

    private Bounds mBounds;

    private Vector3 mPosOffset;
    private Vector3 mSizeEx;

    void Start()
    {
        mPosition = transform.position;
    }

    void Update()
    {
        Vector3 movedir = transform.position - mPosition;
        mPosition = transform.position;

        float xex = 0,zex = 0;
        if (movedir.x < -Mathf.Epsilon)
            xex = -leftExtDis;
        else if (movedir.x > Mathf.Epsilon)
            xex = rightExtDis;
        else
            xex = 0;
        if (movedir.z < -Mathf.Epsilon)
            zex = -bottomExtDis;
        else if (movedir.z > Mathf.Epsilon)
            zex = topExtDis;
        else
            zex = 0;
        mPosOffset = new Vector3(xex*0.5f, 0, zex*0.5f);
        mSizeEx = new Vector3(Mathf.Abs(xex), 0, Mathf.Abs(zex));
    }

    public override bool IsDetected(Bounds bounds)
    {
        mBounds.center = position + mPosOffset;
        mBounds.size = detectorSize + mSizeEx;
        return bounds.Intersects(mBounds);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Bounds b = new Bounds(transform.position + mPosOffset, detectorSize + mSizeEx);
        b.DrawBounds(Color.yellow);
    }
#endif
}
