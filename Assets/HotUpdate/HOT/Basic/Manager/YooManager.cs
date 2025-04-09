using System.Collections.Generic;
using UnityEngine;
using YooAsset;

public class YooManager
{
    public static ResourcePackage package;
    private string packageName = "DefaultPackage";

    public YooManager()
    {
        package = YooAssets.TryGetPackage(packageName);
        if (package == null)
            package = YooAssets.CreatePackage(packageName);
    }
}





