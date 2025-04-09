/*
***********************************
作者:       TianJC
创建时间:   2021/09/07 17:05:43
模块描述:
***********************************
*/

#region 修改日志
/*
***********************************
版本号:
修改者:
修改时间:
修改描述:
***********************************
*/
#endregion

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;
using System.Drawing;
using Newtonsoft.Json.Linq;

public class CameraControl : MonoBehaviour
{
    public Transform targetObject;
    public Vector3 targetPosition;
    [Header("转动")]
    [SerializeField] private float rotateOnOff = 60;
    [SerializeField] internal float xSpeed = 1;
    [SerializeField] internal float ySpeed = 1;
    [SerializeField] private int yMinLimit = -5;
    [SerializeField] private int yMaxLimit = 80;
    [SerializeField] private float dampening = 4;
    [Header("拉伸")]
    [SerializeField] internal float zoomSpeed = 1;
    [SerializeField] internal float maxDistance = 400;
    [SerializeField] private float minDistance = 10;
    [Header("范围")]
    [SerializeField] public bool isMove = true;
    [SerializeField] internal Vector2 rangeX;
    [SerializeField] internal Vector2 rangeZ;
    [Header("状态")]
    public float xDeg = 0;
    public float yDeg = 30;
    public float desiredDistance = 200;

    private float currentDistance;
    private Vector3 currentTargrtPosition;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;
    private Vector3 desiredTargetPosition;
    private Vector3 defalutTargetPosition;
    private Vector2 defalutTargetAngle;
    private Sequence sequence;
    internal float idleTimer = 0;
    public static CameraControl inctance;

    private void Awake()
    {
        inctance = this;
        Init();
    }

    public virtual void Update()
    {
        if (rotateOnOff > 0 && idleTimer > rotateOnOff)
        {
            xDeg += xSpeed * 0.0001f;
        }
        else
        {
            idleTimer += Time.deltaTime;
        }

        xDeg = ClampAngle(xDeg, -180, 180);
        yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
        desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
        currentRotation = transform.rotation;
        rotation = Quaternion.Lerp(currentRotation, desiredRotation, 0.02f * dampening * 2);
        transform.rotation = rotation;

        currentTargrtPosition = targetObject.position;
        desiredTargetPosition = Vector3.Lerp(currentTargrtPosition, targetPosition, 0.02f * dampening * 2);
        targetObject.position = desiredTargetPosition;

        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, 0.02f * dampening);
        if (GetComponent<Camera>().orthographic)
        {
            GetComponent<Camera>().orthographicSize = currentDistance;
        }
        position = targetObject.position - (rotation * Vector3.forward * currentDistance);
        transform.position = position;
    }


    public void Init()
    {
        if (!targetObject)
        {
            GameObject go = new GameObject();
            go.name = "Target";
            go.transform.position = new Vector3(0, 0, 0);
            targetObject = go.transform;
        }

        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;
        currentDistance = desiredDistance;
        targetPosition = targetObject.position;
        currentTargrtPosition = targetObject.position;
        defalutTargetPosition = targetObject.position;
        defalutTargetAngle = new Vector2(xDeg,yDeg);
        position = targetObject.position - (rotation * Vector3.forward * currentDistance);
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -180)
            angle += 360;
        if (angle > 180)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    public bool IsOverUi_Standalone()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public bool IsOverUi_Android(Touch touch)
    {
        return EventSystem.current.IsPointerOverGameObject(touch.fingerId);
    }

    public void Move(float timer = 0.5F)
    {
        MoveAndRotate(defalutTargetPosition,defalutTargetAngle, maxDistance, timer);
        SetPhysicalCamera(Vector2.zero);
    }

    public void Move(Vector3 point, float timer = 0.5F)
    {
        if (sequence != null)
        {
            sequence.Pause();
            sequence = null;
        }
        sequence.Append(DOTween.To(() => targetPosition, z => targetPosition = z, point, timer));
    }

    public void Move(Vector3 point, float zValue,float timer = 0.5F)
    {
        if (sequence != null)
        {
            sequence.Pause();
            sequence = null;
        }
        sequence.Append(DOTween.To(() => desiredDistance, z => desiredDistance = z, zValue, timer));
        sequence.Append(DOTween.To(() => targetPosition, z => targetPosition = z, point, timer));
    }

    public void Rotate(Vector3 angle,float timer = 0.5F) {
        DOTween.To(() => xDeg, z => xDeg = z, angle.x, timer);
        DOTween.To(() => yDeg, z => yDeg = z, angle.y, timer);
        DOTween.To(() => desiredDistance, z => desiredDistance = z, angle.z, timer);
    }

    public void MoveAndRotate(Vector3 point, Vector2 angle, float zValue, float timer = 0.5F) {
        if (sequence != null)
        {
            sequence.Pause();
            sequence = null;
        }

        sequence.Append(DOTween.To(() => desiredDistance, x => desiredDistance = x, zValue, timer));
        sequence.Append(DOTween.To(() => targetPosition, x => targetPosition = x, point, timer));
        sequence.Append(DOTween.To(() => xDeg, x => xDeg = x, angle.x, timer));
        sequence.Append(DOTween.To(() => yDeg, x => yDeg = x, angle.y, timer));
    }

    public void SetPhysicalCamera(Vector2 offset,float timer = 0.5F) {
        DOTween.To(() => Camera.main.lensShift, z => Camera.main.lensShift = z, offset, timer);
    }
}
