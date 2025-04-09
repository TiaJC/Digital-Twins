using HybridCLR;
using System;
using UnityEngine;
using YooAsset;
using System.IO;

public class AOTAssemblies
{
    private static ResourcePackage package;

    public static void Start()
    {
        package = YooAssets.TryGetPackage("DefaultPackage");
        LoadMetadataForAOTAssemblies();
    }

    private static void LoadMetadataForAOTAssemblies()
    {
        /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
        /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
        HomologousImageMode mode = HomologousImageMode.SuperSet;

        if (!package.CheckLocationValid("AOTAssemblyList.txt")) return;
        byte[] listBytes = package.LoadRawFileSync("AOTAssemblyList.txt").GetRawFileData();
        string listStr = System.Text.Encoding.UTF8.GetString(listBytes);
        using (StringReader reader = new StringReader(listStr))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (package.CheckLocationValid(line))
                {
                    byte[] dllBytes = package.LoadRawFileSync(line).GetRawFileData();
                    // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
                    LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
                    Debug.Log($"LoadMetadataForAOTAssembly:{line}. mode:{mode} ret:{err}");
                }
                else
                {
                    Debug.LogError($"{line} not found!");
                }
            }
        }
    }
}