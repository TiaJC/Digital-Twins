using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Log
{
    public static string fileName
    {
        get => DateTime.Now.ToString("yyyy-MM-dd") + ".txt";

    }
    public static string localPath
    {
#if UNITY_EDITOR
        get => Application.streamingAssetsPath + "/Log";
#else
        get => Application.dataPath+"/Log";
#endif
    }


    public static void Append(params string[] contents)
    {
        string str = "";
        foreach (var item in contents)
        {
            if (string.IsNullOrEmpty(item)) continue;
            str += item + "\n";
        }
        Debug.LogWarning(str);
        FileHelper.AppendAllText(localPath, fileName, str);
    }
}
