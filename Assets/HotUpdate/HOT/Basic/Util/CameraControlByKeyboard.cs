// ***********************************************************************
// 作者:       TianJC
// 创建时间:   2018/01/18 11:03:37
// 模块描述:
// ***********************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraControlByKeyboard : CameraControl
{
    private Vector3 dir;
    private Vector3 mousePosition;
    public bool IsPointerOverGameObject;

    private void Start()
    {
        dir = targetPosition;
    }
    public override void Update()
    {
        base.Update();

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            mousePosition = Input.mousePosition;
            IsPointerOverGameObject = EventSystem.current.IsPointerOverGameObject();
        }

        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            IsPointerOverGameObject = false;
        }

        if (IsPointerOverGameObject) return;

        idleTimer = 0;
        targetObject.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Mathf.Abs(desiredDistance);
        if (Input.GetMouseButton(1))
        {
            xDeg += Input.GetAxis("Mouse X") * xSpeed;
            yDeg -= Input.GetAxis("Mouse Y") * ySpeed;
        }
        if (Input.GetMouseButton(0) && isMove)
        {
            //世界坐标转屏幕坐标
            dir = Camera.main.WorldToScreenPoint(targetPosition);
            //屏幕偏移值
            dir -= (Input.mousePosition - mousePosition);
            //屏幕最小移动距离(精度丢失造成的抖动)
            if (Vector3.Distance(Input.mousePosition, mousePosition) > 5)
            {
                //屏幕坐标转世界坐标
                dir = Camera.main.ScreenToWorldPoint(dir);
                if (!Util.Raycast(Vector3.down, ref dir,LayerMask.GetMask("Ground")))
                {
                    dir.y = targetObject.position.y;
                }
                targetPosition = dir;
                mousePosition = Input.mousePosition;
            }
        }

    }
}
