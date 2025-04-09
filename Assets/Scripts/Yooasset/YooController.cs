using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using YooAsset;


public class YooController
{
    internal static ResourcePackage package;
    private static int downloadingMaxNumber = 10;
    private static int failedTryAgain = 3;
    private static int timeout = 60;

    internal static async Task UpdateAsset(string url)
    {
        YooAssets.Initialize();
        package = YooAssets.TryGetPackage("DefaultPackage");
        if (package == null)
            package = YooAssets.CreatePackage("DefaultPackage");

        Prompt("初始化资源更新");
        InitializationOperation initializationOperation = null;   
        var createParameters = new HostPlayModeParameters();
        createParameters.DecryptionServices = new GameDecryptionServices();
        createParameters.BuildinQueryServices = new GameQueryServices();
        createParameters.DeliveryQueryServices = new DefaultDeliveryQueryServices();
        createParameters.RemoteServices = new RemoteServices(url, url);
        initializationOperation = package.InitializeAsync(createParameters);
        await initializationOperation;


        if (string.IsNullOrEmpty(initializationOperation.PackageVersion))
        {
            //首次更新
            Debug.Log("首次更新");
        }
        else {
            Debug.Log("当前版本号："+ package.GetPackageVersion());
        }

        Prompt("获取资源版本号");
        UpdatePackageVersionOperation versionOperation = package.UpdatePackageVersionAsync();
        await versionOperation;
        if (versionOperation.Status != EOperationStatus.Succeed)
        {
            Debug.LogWarning(versionOperation.Error);
            Prompt("获取资源版本号失败");
            return;
        }
        Debug.Log("最新版本："+versionOperation.PackageVersion);
        Prompt("获取更新清单");
        UpdatePackageManifestOperation manifestOperation = package.UpdatePackageManifestAsync(versionOperation.PackageVersion);
        await manifestOperation;
        if (manifestOperation.Status != EOperationStatus.Succeed)
        {
            Debug.LogWarning(manifestOperation.Error);
            Prompt("获取更新清单失败");
            return;
        }

        ResourceDownloaderOperation downloader = package.CreateResourceDownloader("Patch", downloadingMaxNumber, failedTryAgain, timeout);
        if (downloader.TotalDownloadCount > 0)
        {
            //FsmDownloadFiles
            int totalDownloadCount = downloader.TotalDownloadCount;
            long totalDownloadBytes = downloader.TotalDownloadBytes;
            Debug.Log($"Update resource package size.  数量 : {totalDownloadCount} 大小 ; {totalDownloadBytes}");
            //下载前检测磁盘空间不足
        }

        downloader.OnStartDownloadFileCallback = OnStartDownloadFile;
        downloader.OnDownloadErrorCallback = OnDownloadError;
        downloader.OnDownloadOverCallback = OnDownloadOver;
        downloader.OnDownloadProgressCallback = OnDownloadProgress;
        downloader.BeginDownload();
        await downloader;
        if (downloader.Status != EOperationStatus.Succeed)
        {
            Debug.LogWarning(downloader.Error);
            Prompt("下载资源失败");
            return;
        }

        Prompt("清理缓存");
        var operation = package.ClearUnusedCacheFilesAsync();
        await operation;

        if (operation.Status != EOperationStatus.Succeed)
        {
            Debug.LogWarning(operation.Error);
        }

#if !UNITY_EDITOR
        RawFileOperationHandle rawFile = package.LoadRawFileAsync("HotUpdate.dll");
        await rawFile;
        Assembly hotUpdateAss = Assembly.Load(rawFile.GetRawFileData());
#else
        // Editor下无需加载，直接查找获得HotUpdate程序集
        Assembly hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "HotUpdate");
#endif
        Type type = hotUpdateAss.GetType("AOTAssemblies");
        type.GetMethod("Start").Invoke(null, null);

        Complete?.Invoke();
    }  

    public static event DownloaderOperation.OnDownloadError OnDownloadError;

    public static event DownloaderOperation.OnStartDownloadFile OnStartDownloadFile;

    public static event DownloaderOperation.OnDownloadOver OnDownloadOver;

    public static event DownloaderOperation.OnDownloadProgress OnDownloadProgress;

    public delegate void OnPrompt(string content);
    public static event OnPrompt Prompt;

    public delegate void OnComplete();
    public static event OnComplete Complete;
}
