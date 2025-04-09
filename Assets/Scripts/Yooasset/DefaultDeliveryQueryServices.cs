using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

/// <summary>
/// 默认的分发资源查询服务类
/// </summary>
public class DefaultDeliveryQueryServices : IDeliveryQueryServices
{
    public DeliveryFileInfo GetDeliveryFileInfo(string packageName, string fileName)
    {
        throw new NotImplementedException();
    }
    public bool QueryDeliveryFiles(string packageName, string fileName)
    {
        return false;
    }
}
