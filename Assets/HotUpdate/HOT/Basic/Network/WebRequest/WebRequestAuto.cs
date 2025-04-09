// ***********************************************************************
// 作者:       TianJC
// 创建时间:   2019/08/21 14:24:03
// 模块描述:   自动间隔请求
// ***********************************************************************

using UnityEngine;
using System;

public abstract class WebRequestAuto<T> : WebRequestBase<T>
{
    private float interval = 60F;
    private float timer;
    protected override async void UpdateData()
    {
        while (true)
        {
            await new WaitUntil(() => { return interfaceStruct.interfaceString != null; });
            ApiResult<T> apiResult = await gameObject.ExecuteToObject<T>(interfaceStruct.interfaceString, interfaceStruct.upload, interfaceStruct.parameter);
            if (apiResult != null && apiResult.Success && apiResult.Data != null)
            {
                try
                {
                    await ShowData(apiResult.Data);
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            }
            timer = interval;
            await new WaitUntil(() => { timer -= Time.deltaTime; return timer <= 0; });
        }
    }
}
