using UnityEngine;

/// <summary>
/// UI跟随3D物体
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class Follow3DObject : MonoBehaviour
{
    public float followSpeed = 5f;     // 跟随平滑速度
    public bool rotateWithTarget = true;  // 是否旋转跟随目标
    public bool adjustSizeToScreen = true;  // 是否动态调整UI大小

    private Transform target;           // 目标物体
    private Vector3 offset;             // 偏移量
    private Canvas canvas;             // Canvas 组件
    private Vector2 screenResolution;  // 屏幕分辨率
    private Vector2 screenUIOffset;    // 屏幕偏移量
    private Camera mainCamera;         // 主相机缓存
    private CanvasGroup canvasGroup;   // CanvasGroup 组件
    private RectTransform rectTransform; // UI元素的 RectTransform

    private CanvasGroup CanvasGroup
    {
        get
        {
            if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
            return canvasGroup;
        }
    }

    private void Awake()
    {
        // 缓存 Canvas 和 Camera 组件
        canvas = GetComponentInParent<Canvas>();
        mainCamera = Camera.main;
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        // 初始化屏幕分辨率和偏移量
        screenResolution = new Vector2(Screen.width, Screen.height);
        ScreenZoomValue();
        AdjustSize();
    }

    private void Update()
    {
        // 如果屏幕分辨率发生变化
        if (Screen.width != screenResolution.x || Screen.height != screenResolution.y)
        {
            screenResolution = new Vector2(Screen.width, Screen.height);
            ScreenZoomValue();
            AdjustSize();
        }

        if (target != null)
        {
            TransformUIElement();
        }
    }

    /// <summary>
    /// 设置目标物体和偏移量
    /// </summary>
    /// <param name="targetTransform"></param>
    /// <param name="targetOffset"></param>
    /// <returns></returns>
    public Follow3DObject Set(Transform targetTransform, Vector3 targetOffset)
    {
        target = targetTransform;
        offset = targetOffset;
        TransformUIElement();
        return this;
    }

    private void TransformUIElement()
    {
        // 计算目标物体的屏幕位置
        Vector3 targetPosition = mainCamera.WorldToScreenPoint(target.position + offset);

        // 判断目标是否在屏幕内
        if (targetPosition.z > 0 && targetPosition.x >= 0 && targetPosition.x <= screenResolution.x && targetPosition.y >= 0 && targetPosition.y <= screenResolution.y)
        {
            // 屏幕坐标系中心化，并根据 Canvas 缩放调整位置
            targetPosition -= new Vector3(screenResolution.x / 2, screenResolution.y / 2, 0);
            targetPosition.x *= screenUIOffset.x;
            targetPosition.y *= screenUIOffset.y;

            // 使用平滑插值让 UI 元素移动
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, followSpeed * Time.deltaTime);

            // 可见性控制
            Visible();

            // 如果需要，旋转 UI 元素朝向目标
            if (rotateWithTarget)
            {
                transform.LookAt(target);
            }
        }
        else
        {
            // 如果目标不在屏幕内，隐藏 UI 元素
            Invisible();
        }
    }

    // 屏幕分辨率与 Canvas 缩放的转换
    private void ScreenZoomValue()
    {
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        screenUIOffset.x = canvasRect.sizeDelta.x / screenResolution.x;
        screenUIOffset.y = canvasRect.sizeDelta.y / screenResolution.y;
    }

    // 动态调整UI大小，适应不同分辨率
    private void AdjustSize()
    {
        if (adjustSizeToScreen)
        {
            // 假设我们需要一个根据屏幕分辨率调整的大小因子
            float screenWidthFactor = Screen.width / 1920f; // 假设1920为设计屏幕宽度
            float screenHeightFactor = Screen.height / 1080f; // 假设1080为设计屏幕高度

            // 比例因子，保证屏幕适配
            float scaleFactor = Mathf.Min(screenWidthFactor, screenHeightFactor);

            // 调整RectTransform的大小（保持比例）
            rectTransform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
        }
    }

    // 显示 UI 元素
    private void Visible()
    {
        if (CanvasGroup.alpha < 1)
        {
            CanvasGroup.alpha = Mathf.Lerp(CanvasGroup.alpha, 1, followSpeed * Time.deltaTime);
        }
    }

    // 隐藏 UI 元素
    private void Invisible()
    {
        if (CanvasGroup.alpha > 0)
        {
            CanvasGroup.alpha = Mathf.Lerp(CanvasGroup.alpha, 0, followSpeed * Time.deltaTime);
        }
    }
}
