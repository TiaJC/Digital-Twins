using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class AssetBundleExpand
{
    /// <summary>
    /// 读取本地AssetBundle
    /// </summary>
    public static async Task<AssetBundle> GetLocalAssetBundleAsync(this GameObject _gameObject, string url)
    {
        return await _gameObject.AddComponent<AssetBundleHelper>().GetAssetBundle(url);
    }

    /// <summary>
    /// 加载在线AssetBundle
    /// </summary>
    public static IEnumerator GetAssetBundle(this GameObject _gameObject, string url, Action<AssetBundle, byte[]> action)
    {
        yield return _gameObject.AddComponent<AssetBundleHelper>().GetAssetBundle(url, action);
    }
}

public static class AssetBundleManager
{
    public static Dictionary<string, AssetBundle> _bundles = new Dictionary<string, AssetBundle>();
}

public class AssetBundleHelper : MonoBehaviour
{
    public async Task<AssetBundle> GetAssetBundle(string path,bool unloadAsync = false, bool unloadAllLoadedObjects = false)
    {
        if (AssetBundleManager._bundles.ContainsKey(path)) return AssetBundleManager._bundles[path];
        AssetBundle assetBundle = null;
        try
        {
            AssetBundleCreateRequest assetBundleCreate = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(path));
            assetBundleCreate.allowSceneActivation = false;
            await assetBundleCreate;
            assetBundle = assetBundleCreate.assetBundle;
            if(!unloadAsync) AssetBundleManager._bundles.Add(path, assetBundle);
            return assetBundle;
        }
        catch (Exception ex)
        {
            Log.Append("读取AB文件", path, ex.Message);
            return assetBundle;
        }
        finally
        {
            if (unloadAsync) await assetBundle.UnloadAsync(unloadAllLoadedObjects);
            DestroyImmediate(this);
        }
    }

    public IEnumerator GetAssetBundle(string url, Action<AssetBundle, byte[]> action)
    {
        using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(new Uri(new Uri(WebRequestParams.Url), url)))
        {
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Log.Append("下载AB文件", url, request.error);
                action?.Invoke(null, null);
            }
            try
            {
                action?.Invoke((request.downloadHandler as DownloadHandlerAssetBundle).assetBundle, request.downloadHandler.data);
            }
            catch (Exception ex)
            {
                Log.Append("读取AB文件", url, ex.Message);
                action?.Invoke(null, null);
            }
        }
    }
}
