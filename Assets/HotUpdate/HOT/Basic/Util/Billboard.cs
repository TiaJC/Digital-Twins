// ***********************************************************************
// 作者:       TianJC
// 创建时间:   2018/01/18 11:03:37
// 模块描述:   公告板
// ***********************************************************************

using UnityEngine;

public class Billboard : MonoBehaviour
{
    void Update()
    {
        Vector3 v = Camera.main.transform.position - transform.position;
        v.x = v.z = 0.0f;
        transform.LookAt(Camera.main.transform.position - v);
    }
}