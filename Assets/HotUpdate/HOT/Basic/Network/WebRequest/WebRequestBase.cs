using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WebRequestBase<T> : MonoBehaviour
{
    protected ApiResult<T> apiResult = null;
    public InterfaceStruct interfaceStruct;

    public abstract void Reset();

    public virtual void Awake()
    {
        Init();
    }

    protected virtual void OnEnable()
    {
        UpdateData();
    }

    protected virtual void OnDisable()
    {
        StopAllCoroutines();
    }
    protected abstract void UpdateData();

    protected abstract void Init();

    protected abstract IEnumerator ShowData(T data);

    public struct InterfaceStruct
    {
        public WebRequestInterface interfaceString;
        public object upload;
        public string[] parameter;
    }
}
