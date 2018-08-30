using UnityEngine;
using System.Collections;

public static class BoundsEx
{
    /// <summary>
    /// 绘制包围盒
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="color"></param>
    public static void DrawEx(this Bounds bounds, Color color)
    {
        Gizmos.color = color;

        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }

    /// <summary>
    /// 判断包围盒是否被相机裁剪
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="camera"></param>
    /// <returns></returns>
    public static bool IsBoundsInCameraEx(this Bounds bounds, Camera camera)
    {

        Matrix4x4 matrix = camera.projectionMatrix * camera.worldToCameraMatrix;
        float halfX = bounds.size.x / 2;
        float halfY = bounds.size.y / 2;
        float halfZ = bounds.size.z / 2;
        int code = ComputeOutCode(new Vector4(bounds.center.x + halfX, bounds.center.y + halfY, bounds.center.z + halfZ, 1), matrix);


        code &= ComputeOutCode(new Vector4(bounds.center.x - halfX, bounds.center.y + halfY, bounds.center.z + halfZ, 1), matrix);

        code &= ComputeOutCode(new Vector4(bounds.center.x + halfX, bounds.center.y - halfY, bounds.center.z + halfZ, 1), matrix);

        code &= ComputeOutCode(new Vector4(bounds.center.x - halfX, bounds.center.y - halfY, bounds.center.z + halfZ, 1), matrix);

        code &= ComputeOutCode(new Vector4(bounds.center.x + halfX, bounds.center.y + halfY, bounds.center.z - halfZ, 1), matrix);

        code &= ComputeOutCode(new Vector4(bounds.center.x - halfX, bounds.center.y + halfY, bounds.center.z - halfZ, 1), matrix);

        code &= ComputeOutCode(new Vector4(bounds.center.x + halfX, bounds.center.y - halfY, bounds.center.z - halfZ, 1), matrix);

        code &= ComputeOutCode(new Vector4(bounds.center.x - halfX, bounds.center.y - halfY, bounds.center.z - halfZ, 1), matrix);


        if (code != 0) return false;

        return true;
    }

    public static bool IsBoundsInCameraEx(this Bounds bounds, Camera camera, float leftex, float rightex, float downex, float upex)
    {

        Matrix4x4 matrix = camera.projectionMatrix * camera.worldToCameraMatrix;
        float halfX = bounds.size.x / 2;
        float halfY = bounds.size.y / 2;
        float halfZ = bounds.size.z / 2;
        int code = ComputeOutCodeEx(new Vector4(bounds.center.x + halfX, bounds.center.y + halfY, bounds.center.z + halfZ, 1), matrix, leftex, rightex, downex, upex);


        code &= ComputeOutCodeEx(new Vector4(bounds.center.x - halfX, bounds.center.y + halfY, bounds.center.z + halfZ, 1), matrix, leftex, rightex, downex, upex);

        code &= ComputeOutCodeEx(new Vector4(bounds.center.x + halfX, bounds.center.y - halfY, bounds.center.z + halfZ, 1), matrix, leftex, rightex, downex, upex);

        code &= ComputeOutCodeEx(new Vector4(bounds.center.x - halfX, bounds.center.y - halfY, bounds.center.z + halfZ, 1), matrix, leftex, rightex, downex, upex);

        code &= ComputeOutCodeEx(new Vector4(bounds.center.x + halfX, bounds.center.y + halfY, bounds.center.z - halfZ, 1), matrix, leftex, rightex, downex, upex);

        code &= ComputeOutCodeEx(new Vector4(bounds.center.x - halfX, bounds.center.y + halfY, bounds.center.z - halfZ, 1), matrix, leftex, rightex, downex, upex);

        code &= ComputeOutCodeEx(new Vector4(bounds.center.x + halfX, bounds.center.y - halfY, bounds.center.z - halfZ, 1), matrix, leftex, rightex, downex, upex);

        code &= ComputeOutCodeEx(new Vector4(bounds.center.x - halfX, bounds.center.y - halfY, bounds.center.z - halfZ, 1), matrix, leftex, rightex, downex, upex);

        if (code != 0) return false;

        return true;
    }

    private static int ComputeOutCode(Vector4 pos, Matrix4x4 projection)
    {
        pos = projection*pos;
        int code = 0;
        if (pos.x < -pos.w) code |= 0x01;
        if (pos.x > pos.w) code |= 0x02;
        if (pos.y < -pos.w) code |= 0x04;
        if (pos.y > pos.w) code |= 0x08;
        if (pos.z < -pos.w) code |= 0x10;
        if (pos.z > pos.w) code |= 0x20;
        return code;
    }

    private static int ComputeOutCodeEx(Vector4 pos, Matrix4x4 projection, float leftex, float rightex, float downex, float upex)
    {
        pos = projection * pos;
        int code = 0;
        if (pos.x < (-1+leftex)*pos.w) code |= 0x01;
        if (pos.x > (1+rightex)*pos.w) code |= 0x02;
        if (pos.y < (-1+downex)*pos.w) code |= 0x04;
        if (pos.y > (1+upex)*pos.w) code |= 0x08;
        if (pos.z < -pos.w) code |= 0x10;
        if (pos.z > pos.w) code |= 0x20;
        return code;
    }

    /// <summary>
    /// 判断包围盒是否包含另一个包围盒
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="compareTo"></param>
    /// <returns></returns>
    public static bool ContainsEx(this Bounds bounds, Bounds compareTo)
    {
        float halfX = compareTo.size.x / 2;
        float halfY = compareTo.size.y / 2;
        float halfZ = compareTo.size.z / 2;

        if (!bounds.Contains(compareTo.center + new Vector3(-halfX, halfY, -halfZ)))
            return false;
        if (!bounds.Contains(compareTo.center + new Vector3(halfX, halfY, -halfZ)))
            return false;
        if (!bounds.Contains(compareTo.center + new Vector3(halfX, halfY, halfZ)))
            return false;
        if (!bounds.Contains(compareTo.center + new Vector3(-halfX, halfY, halfZ)))
            return false;
        if (!bounds.Contains(compareTo.center + new Vector3(-halfX, -halfY, -halfZ)))
            return false;
        if (!bounds.Contains(compareTo.center + new Vector3(halfX, -halfY, -halfZ)))
            return false;
        if (!bounds.Contains(compareTo.center + new Vector3(halfX, -halfY, halfZ)))
            return false;
        if (!bounds.Contains(compareTo.center + new Vector3(-halfX, -halfY, halfZ)))
            return false;
        return true;
    }


    public static string ToStringEx(this Bounds bounds)
    {
        return string.Format("({0},{1},{2},{3},{4},{5})", bounds.center.x, bounds.center.y, bounds.center.z, bounds.size.x, bounds.size.y, bounds.size.z);
    }

}
