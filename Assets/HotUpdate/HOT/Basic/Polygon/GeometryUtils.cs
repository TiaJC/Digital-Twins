using UnityEngine;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// 返回多边形的矩形
/// </summary>
public static class GeometryUtils
{
    public static List<Vector3> GetBoundingBoxVertices(List<Vector3> vertices)
    {
        // 计算最小和最大坐标
        float minX = vertices.Min(v => v.x);
        float minY = vertices.Min(v => v.y);
        float minZ = vertices.Min(v => v.z);

        float maxX = vertices.Max(v => v.x);
        float maxY = vertices.Max(v => v.y);
        float maxZ = vertices.Max(v => v.z);

        // 计算三维包围盒的八个顶点
        List<Vector3> boundingBoxVertices = new List<Vector3>
    {
        new Vector3(minX, minY, minZ), // 1. 左下后
        new Vector3(maxX, minY, minZ), // 2. 右下后
        new Vector3(maxX, maxY, minZ), // 3. 右上后
        new Vector3(minX, maxY, minZ), // 4. 左上后
        
        new Vector3(minX, minY, maxZ), // 5. 左下前
        new Vector3(maxX, minY, maxZ), // 6. 右下前
        new Vector3(maxX, maxY, maxZ), // 7. 右上前
        new Vector3(minX, maxY, maxZ)  // 8. 左上前
    };

        return boundingBoxVertices; // 返回按顺序排列的八个顶点
    }

}
