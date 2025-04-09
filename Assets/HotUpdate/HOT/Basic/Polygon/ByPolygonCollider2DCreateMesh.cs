using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通过PolygonCollider2D创建Mesh
/// </summary>
public class ByPolygonCollider2DCreateMesh
{
    /// <summary>
    /// 创建Mesh
    /// </summary>
    /// <param name="points"></param>
    /// <param name="shader"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public static GameObject CreateMeshByPolygonCollider2D(Vector2[] points,Shader shader,Color color) {
        GameObject go = new GameObject();
        go.AddComponent<MeshFilter>().mesh = CreateMeshByPolygonCollider2D(go, points);
        go.AddComponent<MeshRenderer>().material = CreateMaterial(shader, color, 0.2F);
        return go;
    }

    /// <summary>
    /// 创建Mesh
    /// </summary>
    /// <param name="go"></param>
    /// <param name="points"></param>
    /// <returns></returns>
    private static Mesh CreateMeshByPolygonCollider2D(GameObject go, Vector2[] points)
    {
        PolygonCollider2D collider2D = go.AddComponent<PolygonCollider2D>();
        collider2D.SetPath(0, points);
        return collider2D.CreateMesh(false, false);
    }

    /// <summary>
    /// 设置材质球
    /// </summary>
    /// <param name="shader"></param>
    /// <param name="color"></param>
    /// <param name="alpha"></param>
    /// <returns></returns>
    private static Material CreateMaterial(Shader shader, Color color, float alpha)
    {
        Material material = new Material(shader);//Shader.Find("Custom/Area")
        material.SetColor("_BaseColor", color);
        return material;
    }

    //传入顶点集合，得到高阶的贝塞尔曲线，顶点数量不限
    //vertexCount 为构建曲线的顶点数，此数值越大曲线越平滑
    private static Vector3[] GetBezierCurveWithUnlimitPoints(Vector3[] vertex, int vertexCount)
    {
        List<Vector3> pointList = new List<Vector3>();
        pointList.Clear();
        for (float ratio = 0; ratio <= 1; ratio += 1.0f / vertexCount)
        {
            pointList.Add(UnlimitBezierCurve(vertex, ratio));
        }
        pointList.Add(vertex[vertex.Length - 1]);

        return pointList.ToArray();
    }

    private static Vector3 UnlimitBezierCurve(Vector3[] vecs, float t)
    {
        Vector3[] temp = new Vector3[vecs.Length];
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] = vecs[i];
        }
        //顶点集合有多长，曲线的每一个点就需要计算多少次。

        int n = temp.Length - 1;
        for (int i = 0; i < n; i++)
        {
            //依次计算各两个相邻的顶点的插值，并保存，每次计算都会进行降阶。剩余多少阶计算多少次。直到得到最后一条线性曲线。

            for (int j = 0; j < n - i; j++)
            {
                temp[j] = Vector3.Lerp(temp[j], temp[j + 1], t);
            }
        }
        //返回当前比例下曲线的点
        return temp[0];
    }
}
