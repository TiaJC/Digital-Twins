using System;
using System.IO;
using System.Text;

public class FileHelper
{
    /// <summary>
    /// 创建文件夹
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    public static bool CreateDirectory(string directory)
    {
        if (Directory.Exists(directory)) return true;
        try
        {
            Directory.CreateDirectory(directory);
            return true;
        }
        catch (Exception ex)
        {
            Log.Append("创建文件夹", directory, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 删除文件夹
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    public static bool DeleteDirectory(string directory)
    {
        if (!Directory.Exists(directory)) return true;
        try
        {
            Directory.Delete(directory,true);
            return true;
        }
        catch (Exception ex)
        {
            Log.Append("删除文件夹", directory, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 判断文件是否存在
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="fileName"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool Exists(string directory, string fileName, out string path)
    {
        path = Path.Combine(directory, fileName);
        return File.Exists(path);
    }

    /// <summary>
    /// 创建文件
    /// </summary>
    /// <param name="directory">文件夹路径</param>
    /// <param name="fileName">文件名(带后缀)</param>
    /// <param name="path">完整路径</param>
    /// <returns></returns>
    public static bool CreateFile(string directory, string fileName, out string path)
    {

        path = Path.Combine(directory, fileName);
        if (!CreateDirectory(directory)) return false;
        if (File.Exists(path)) return true;
        try
        {
            File.Create(path).Dispose();
            return true;
        }
        catch (Exception ex)
        {
            Log.Append("创建文件", path, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 读取文本信息
    /// </summary>
    /// <param name="directory">文件夹路径</param>
    /// <param name="fileName">文件名(带后缀)</param>
    /// <returns></returns>
    public static string ReadAllText(string directory, string fileName)
    {
        string path = Path.Combine(directory, fileName);
        try
        {
            if (!File.Exists(path)) throw new Exception("文件不存在");
            return File.ReadAllText(path, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            Log.Append("读取文本内容", path, ex.Message);
            return null;
        }
    }

    /// <summary>
    /// 写入文本信息
    /// </summary>
    /// <param name="directory">文件夹路径</param>
    /// <param name="fileName">文件名(带后缀)</param>
    /// <param name="content">内容</param>
    public static void AppendAllText(string directory, string fileName, string content)
    {
        if (!CreateFile(directory, fileName, out string path)) return;
        try
        {
            File.AppendAllText(path, content, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            Log.Append("写入文本内容", path, ex.Message);
        }
    }

    /// <summary>
    /// 保存文件
    /// </summary>
    /// <param name="directory">文件夹路径</param>
    /// <param name="fileName">文件名(带后缀)</param>
    /// <param name="bytes"></param>
    public static void SaveFile(string directory, string fileName, byte[] bytes)
    {
        if (!CreateFile(directory, fileName, out string path)) return;
        FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate);
        try
        {
            fileStream.Write(bytes, 0, bytes.Length);
        }
        catch (Exception ex)
        {
            Log.Append("保存文件", path, ex.Message);
        }
        finally
        {
            fileStream.Close();
            fileStream.Dispose();
        }
    }
}
