using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class DownLoadHelper 
{
    /// <summary>
    /// 下载文件获取流
    /// </summary>
    public static async Task<byte[]> DownLoad(this GameObject _gameObject, string url, Action<float> progress = null)
    {
        return await _gameObject.AddComponent<DownLoad>()._DownLoad(url, progress);
    }

    /// <summary>
    /// 下载文件并保存本地
    /// </summary>
    public static async Task<string> DownLoadAndSave(this GameObject _gameObject, string url, string path, string fileName, Action<float> progress = null)
    {
        return await _gameObject.AddComponent<DownLoad>().DownLoadAndSave(url, path, fileName, progress);
    }
}

public class DownLoad : MonoBehaviour {
    public async Task<byte[]> _DownLoad(string url, Action<float> progress = null)
    {
        using (UnityWebRequest request = new UnityWebRequest(new Uri(new Uri(WebRequestParams.Url), url)))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SendWebRequest();

            while (!request.isDone) {
                progress?.Invoke(request.downloadProgress);
                await new WaitForEndOfFrame();
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                Log.Append("下载文件", url, request.error);
            }
            return request.downloadHandler.data;
        }
    }

    public async Task<string> DownLoadAndSave(string url, string path, string fileName, Action<float> progress = null)
    {
        string localPath = Path.Combine(path, fileName);
        byte[] bytes = await _DownLoad(url, progress);
        FileHelper.SaveFile(path, fileName, bytes);
        return localPath;
    }
}
