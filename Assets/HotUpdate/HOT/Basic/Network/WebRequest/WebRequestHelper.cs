// ***********************************************************************
// 作者:       TianJC
// 创建时间:   2018/01/18 11:03:37
// 模块描述:   网络请求
// ***********************************************************************

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

#region 请求配置
public class WebRequestParams
{
    public static string Url
    {

        get =>
#if UNITY_EDITOR
            "url";
#else
        "";
#endif
    }

    public static string ContentType
    {
        get => "application/json;charset=UTF-8";
    }
    public static int Timeout { get => 60; }
    public static int Retry { get => 3; }
    public static Dictionary<string, string> Header { get => header; }


    private static Dictionary<string, string> header = new Dictionary<string, string>()
    {
#if UNITY_EDITOR
        { "X-Auth-Token","335795c881034745b408fcdad46e8f02"}
#endif
    };
    public static void SetHeader(string key, string value)
    {
        if (!header.ContainsKey(key))
        {
            header.Add(key, null);
        }
        header[key] = value;
    }

    public static void ClearHeaderAll()
    {
        header = new Dictionary<string, string>();
    }

    public static void ClearHeader(string key)
    {
        if (header.ContainsKey(key)) header.Remove(key);
    }
}
#endregion

#region 接口列表
public class WebRequestInterface
{
    public string name;
    public string url;
    public string[] parameter;

    public WebRequestInterface(string name, string url, params string[] parameter)
    {
        this.name = name;
        this.url = url;
        this.parameter = parameter;
    }

    public WebRequestInterface(string name, string url)
    {
        this.name = name;
        this.url = url;
        parameter = null;
    }
}
#endregion

#region 请求调用
public static class WebRequestHelper
{


    public static ApiResult<T> DeserializeObject<T>(string value)
    {
        try
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            return JsonConvert.DeserializeObject<ApiResult<T>>(value, settings);
        }
        catch (Exception ex)
        {
            return new ApiResult<T>(false, ex.Message);
        }
    }

    public static async Task<ApiResult<string>> Execute(this GameObject _gameObject, WebRequestInterface requestInterface, object upload = null, params string[] parameter)
    {
        string url = requestInterface.url;
        if (parameter != null && requestInterface.parameter != null)
        {
            for (int i = 0; i < requestInterface.parameter.Length && i < parameter.Length; i++)
            {
                if (i == 0) url += "?";
                url += requestInterface.parameter[i] + "=" + parameter[i];
                if (i + 1 < requestInterface.parameter.Length && i < parameter.Length) url += "&";
            }
        }
        return await _gameObject.AddComponent<WebRequest>().Execute(requestInterface.name, url, (upload == null) ? UnityWebRequest.kHttpVerbGET : UnityWebRequest.kHttpVerbPOST, upload);
    }

    public static async Task<ApiResult<T>> ExecuteToObject<T>(this GameObject _gameObject, WebRequestInterface requestInterface, object upload = null, params string[] parameter)
    {
        ApiResult<string> apiResult = await Execute(_gameObject, requestInterface, upload, parameter);
        if (!apiResult.Success) return new ApiResult<T>() { Success = false, Message = apiResult.Message };
        ApiResult<T> result = DeserializeObject<T>(apiResult.Data);
        if (result.Data == null) return new ApiResult<T>() { Success = false, Message = "数据为空" };
        return result;
    }
}
#endregion

#region 请求实现
public class WebRequest : MonoBehaviour
{
    int retryIndex = 0;
    public async Task<ApiResult<string>> Execute(string name, string url, string method, object upload)
    {
        try
        {
            ApiResult<string> apiResult = await _Execute(name, url, method, upload);
            if (apiResult.Success) return apiResult;
            if (retryIndex < WebRequestParams.Retry)
            {
                retryIndex++;
                Log.Append($"{name}重试请求:{retryIndex}", url, apiResult.Message);
                return await Execute(name, url, method, upload);
            }
            return new ApiResult<string>(false, apiResult.Message);
        }
        finally
        {
            DestroyImmediate(this);
        }
    }

    private async Task<ApiResult<string>> _Execute(string name, string url, string method, object data)
    {
        using (UnityWebRequest request = new UnityWebRequest(new Uri(new Uri(WebRequestParams.Url), url), method))
        {
            foreach (var key in WebRequestParams.Header.Keys)
            {
                if (!string.IsNullOrEmpty(WebRequestParams.Header[key]))
                {
                    request.SetRequestHeader(key, WebRequestParams.Header[key]);
                }
            }

            request.timeout = WebRequestParams.Timeout;

            if ((method.Equals(UnityWebRequest.kHttpVerbPOST, StringComparison.CurrentCultureIgnoreCase) ||
                method.Equals(UnityWebRequest.kHttpVerbPUT, StringComparison.CurrentCultureIgnoreCase) ||
                method.Equals(UnityWebRequest.kHttpVerbDELETE, StringComparison.CurrentCultureIgnoreCase) &&
                data != null))
            {
                request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)));
                request.uploadHandler.contentType = WebRequestParams.ContentType;
            }
            request.downloadHandler = new DownloadHandlerBuffer();
            await request.SendWebRequest();
            Log.Append(name, request.url, JsonConvert.SerializeObject(data), request.error, request.downloadHandler.text);
            return new ApiResult<string>(string.IsNullOrEmpty(request.error), request.downloadHandler.text, request.error);
        }
    }
}
#endregion

#region 请求返回
[System.Serializable]
public class ApiResult<T>
{
    [JsonProperty("code")] public int Code { get; set; }
    [JsonProperty("data")] public T Data { get; set; }
    [JsonProperty("message")] public string Message { get; set; }
    [JsonProperty("success")] public bool Success { get; set; }

    public ApiResult(bool success, T data, string message)
    {
        Message = message;
        Success = success;
        Data = data;
    }

    public ApiResult(bool success, string message)
    {
        Message = message;
        Success = success;
    }
    public ApiResult() { }
}

[System.Serializable]
public class Records<T>
{
    public T items;

    public Records() { }
}
#endregion
