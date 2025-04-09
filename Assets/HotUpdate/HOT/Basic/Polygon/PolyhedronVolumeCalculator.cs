using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 四面体分解法 ChatGPT
/// </summary>
public class PolyhedronVolumeCalculator 
{
    /// <summary>
    /// 计算多面体体积
    /// </summary>
    /// <param name="vertices"></param>
    /// <returns></returns>
    public static float CalculatePolyhedronVolume(List<Vector3> vertices)
    {
        Vector3 centroid = CalculateCentroid(vertices);
        float totalVolume = 0f;

        // 遍历顶点，计算每个四面体的体积
        for (int i = 1; i < vertices.Count - 1; i++)
        {
            Vector3 p1 = centroid;
            Vector3 p2 = vertices[0];
            Vector3 p3 = vertices[i];
            Vector3 p4 = vertices[i + 1];

            totalVolume += CalculateTetrahedronVolume(p1, p2, p3, p4);
        }

        // 确保体积为正值并返回
        return Mathf.Abs(totalVolume);
    }

    // 计算四面体体积（使用交叉积和点积）
    private static float CalculateTetrahedronVolume(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        Vector3 v1 = p2 - p1;
        Vector3 v2 = p3 - p1;
        Vector3 v3 = p4 - p1;

        // 计算体积
        float volume = Mathf.Abs(Vector3.Dot(v1, Vector3.Cross(v2, v3))) / 6.0f;
        return volume;
    }

    // 计算多面体的质心
    private static Vector3 CalculateCentroid(List<Vector3> vertices)
    {
        Vector3 centroid = Vector3.zero;
        foreach (var vertex in vertices)
        {
            centroid += vertex;
        }
        centroid /= vertices.Count;
        return centroid;
    }
}
