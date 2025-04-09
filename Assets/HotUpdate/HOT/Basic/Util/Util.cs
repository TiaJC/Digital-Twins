using EPOOutline;
using Newtonsoft.Json;
using System;
using UnityEngine;

public class Util
{
    /// <summary>
    /// 射线检测
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public static bool Raycast(Vector3 direction, ref Vector3 position,LayerMask layerMask)
    {
        position += new Vector3(0, short.MaxValue, 0);
        RaycastHit[] hitInfo = Physics.RaycastAll(position, direction, int.MaxValue, layerMask);
        if (hitInfo != null & hitInfo.Length > 0)
        {
            position.y = hitInfo[0].point.y;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 设置描边效果
    /// </summary>
    /// <param name="go"></param>
    /// <param name="color"></param>
    public static void AddOutlinable(GameObject go, Color color)
    {
        if (!go.activeSelf) return;
        Outlinable outlinable;
        if ((outlinable = go.GetComponent<Outlinable>()) == null)
        {
            outlinable = go.AddComponent<Outlinable>();
        }
        outlinable.OutlineParameters.Color = color;
        outlinable.AddAllChildRenderersToRenderingList();
    }

    /// <summary>
    /// 获取时间戳
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static long GetTimeStamp(DateTime dateTime)
    {
        TimeSpan ts = dateTime - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds); // 精确到秒
    }

    /// <summary>
    /// 时间戳转DateTime
    /// </summary>
    /// <param name="unixTimeStamp"></param>
    /// <returns></returns>
    public static DateTime GetDateTime(long unixTimeStamp)
    {
        DateTime dtStart = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), TimeZoneInfo.Local);
        long lTime = long.Parse(unixTimeStamp + "0000");
        TimeSpan toNow = new TimeSpan(lTime);
        return dtStart.Add(toNow);
    }

    public static T DeserializeObject<T>(string content)
    {
        try
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(content, settings);
        }
        catch (Exception ex)
        {
            Debug.Log("序列化错误：" + ex.Message);
            return default(T);
        }
    }
}
