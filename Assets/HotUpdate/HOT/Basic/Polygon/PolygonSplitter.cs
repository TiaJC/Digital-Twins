using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 平均高度分割多边形 ChatGPT
/// </summary>
public class PolygonSplitter
{
    public static void SplitPolygonByHeight(List<Vector3> vertices, float averageHeight,out List<Vector3> highVertices, out List<Vector3> lowVertices)
    {
        highVertices = new List<Vector3>();  // 高于平均值的顶点
        lowVertices = new List<Vector3>();   // 低于平均值的顶点

        // 遍历每个顶点，分配到高或低的列表中
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 currentVertex = vertices[i];
            if (currentVertex.y > averageHeight)
            {
                highVertices.Add(currentVertex);  // 高于平均高度的顶点
            }
            else
            {
                lowVertices.Add(currentVertex);  // 低于平均高度的顶点
            }

            // 处理裁剪逻辑：如果当前顶点和下一个顶点的高度不同，计算它们的交点
            if (i < vertices.Count - 1)
            {
                Vector3 nextVertex = vertices[i + 1];
                // 判断两个顶点是否跨越裁剪平面
                if ((currentVertex.y > averageHeight && nextVertex.y < averageHeight) || (currentVertex.y < averageHeight && nextVertex.y > averageHeight))
                {
                    Vector3 intersection = GetIntersection(currentVertex, nextVertex, averageHeight);
                    highVertices.Add(intersection);  // 将交点添加到高顶点列表
                    lowVertices.Add(intersection);   // 将交点添加到低顶点列表
                }
            }
        }
    }

    // 计算交点
    private static Vector3 GetIntersection(Vector3 p1, Vector3 p2, float height)
    {
        // 插值计算交点
        float t = (height - p1.y) / (p2.y - p1.y);
        float x = p1.x + t * (p2.x - p1.x);
        float z = p1.z + t * (p2.z - p1.z);
        return new Vector3(x, height, z);
    }

    // 计算物体的平均高度
    private static float CalculateAverageHeight(List<Vector3> vertices)
    {
        float totalHeight = 0;
        foreach (var vertex in vertices)
        {
            totalHeight += vertex.y;
        }
        return totalHeight / vertices.Count;
    }

}
