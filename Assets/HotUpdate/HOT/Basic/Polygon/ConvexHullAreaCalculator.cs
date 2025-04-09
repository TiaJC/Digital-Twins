using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 多边形的面积
/// </summary>
public class ConvexHullAreaCalculator
{
    public static double Calculate2DPolygonArea(List<Vector3> points) {
        List<Vector3> temp = new List<Vector3>(points);
        if (temp.Count < 3)
        {
            Debug.LogError("多边形至少需要三个点！");
            return 0;
        }

        for (int i = 0; i < temp.Count; i++) {
            temp[i] = new Vector3(temp[i].x,0, temp[i].z);
        }
        return CalculatePolygonArea(temp);
    }

    /// <summary>
    /// 计算多边形的面积，输入是一个按顺序排列的点的List
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    public static double CalculatePolygonArea(List<Vector3> points)
    {
        List<Vector3> temp = new List<Vector3>(points);
        if (temp.Count < 3)
        {
            Debug.LogError("多边形至少需要三个点！");
            return 0;
        }

        double totalArea = 0.0;

        // 选择第一个点作为基准点
        Vector3 basePoint = temp[0];

        // 使用三角形剖分法，遍历点列表并计算面积
        for (int i = 1; i < temp.Count - 1; i++)
        {
            // 构成三角形的三个点
            Vector3 P2 = temp[i];
            Vector3 P3 = temp[i + 1];

            // 计算每个三角形的面积并加总
            double triangleArea = AreaOfTriangle(basePoint, P2, P3);
            Debug.Log($"第 {i} 个三角形的面积: {triangleArea}");
            totalArea += Math.Abs(triangleArea);  // 强制取绝对值
        }

        Debug.Log($"最终总面积: {totalArea}");

        return totalArea;
    }

    /// <summary>
    /// 计算三角形面积的静态方法，返回 double 类型的面积
    /// </summary>
    /// <param name="P1"></param>
    /// <param name="P2"></param>
    /// <param name="P3"></param>
    /// <returns></returns>
    public static double AreaOfTriangle(Vector3 P1, Vector3 P2, Vector3 P3)
    {
        // 计算向量
        Vector3 v1 = P2 - P1;  // P1P2
        Vector3 v2 = P3 - P1;  // P1P3

        // 叉积
        Vector3 crossProduct = Vector3.Cross(v1, v2);

        // 输出叉积和每个三角形的面积，帮助调试
        Debug.Log($"计算三角形：P1({P1}), P2({P2}), P3({P3})");
        Debug.Log($"叉积: {crossProduct}, 面积: {crossProduct.magnitude / 2.0}");

        // 返回三角形的面积
        return crossProduct.magnitude / 2.0;
    }

    
}
