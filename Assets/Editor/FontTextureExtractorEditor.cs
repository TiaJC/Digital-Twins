using UnityEngine;
using UnityEditor;
using System.IO;
using TMPro;
using TMPro.EditorUtilities;

public class FontTextureExtractorEditor : EditorWindow
{
    private TMP_FontAsset fontAsset; // 统一的字段，表示字体资源

    [MenuItem("Tools/压缩TMP字体")]
    public static void ShowWindow()
    {
        // 打开窗口
        GetWindow<FontTextureExtractorEditor>("Font Texture Extractor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Font Texture Extractor", EditorStyles.boldLabel);

        // 选择字体资源的路径（通过字体对象来选择路径）
        fontAsset = (TMP_FontAsset)EditorGUILayout.ObjectField("Font Asset", fontAsset, typeof(TMP_FontAsset), false);

        // 提取纹理按钮
        if (GUILayout.Button("Extract Texture"))
        {
            if (fontAsset != null)
            {
                // 获取字体资源的路径
                string fontPath = AssetDatabase.GetAssetPath(fontAsset);
                ExtractTexture(fontPath);
            }
            else
            {
                Debug.LogError("请指定一个有效的字体资源！");
            }
        }
    }

    // 调用提取纹理的方法
    public void ExtractTexture(string fontPath)
    {
        try
        {
            // 生成纹理保存路径
            string texturePath = fontPath.Replace(".asset", ".png");

            // 加载字体资源
            TMP_FontAsset targetFontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fontPath);
            if (targetFontAsset == null)
            {
                Debug.LogError("无法加载字体资源，路径错误或资源不存在！");
                return;
            }

            // 获取图集纹理
            Texture2D atlasTexture = targetFontAsset.atlasTexture;
            if (atlasTexture == null)
            {
                Debug.LogError("字体资源中没有图集纹理！");
                return;
            }

            // 创建新纹理并复制图集纹理
            Texture2D texture2D = new Texture2D(atlasTexture.width, atlasTexture.height, TextureFormat.Alpha8, false);
            Graphics.CopyTexture(atlasTexture, texture2D);

            // 检查文件是否已存在
            if (File.Exists(texturePath))
            {
                Debug.LogWarning($"文件 {texturePath} 已存在，是否覆盖？");
            }

            // 使用 'using' 确保 FileStream 正常关闭
            byte[] dataBytes = texture2D.EncodeToPNG();
            using (FileStream fs = new FileStream(texturePath, FileMode.OpenOrCreate))
            {
                fs.Write(dataBytes, 0, dataBytes.Length);
                fs.Flush();
            }

            // 刷新资源数据库
            AssetDatabase.Refresh();

            // 重新加载新的纹理并更新字体资源
            Texture2D atlas = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath.Replace(Application.dataPath, "Assets"));
            if (atlas == null)
            {
                Debug.LogError("无法加载新保存的纹理！");
                return;
            }

            targetFontAsset.material.SetTexture(ShaderUtilities.ID_MainTex,atlas);
            Material[] matPresers = TMP_EditorUtility.FindMaterialReferences(targetFontAsset);
            foreach (var mat in matPresers)
            {
                mat.SetTexture(ShaderUtilities.ID_MainTex, atlas);
            }

            // 移除旧纹理并更新字体资源的纹理
            AssetDatabase.RemoveObjectFromAsset(targetFontAsset.atlasTexture);
            targetFontAsset.atlasTextures[0] = atlas;

            // 更新材质的主纹理
            targetFontAsset.material.mainTexture = atlas;

            // 保存并刷新资产数据库
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"纹理提取并成功保存为 {texturePath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"处理过程中发生错误: {ex.Message}");
        }
    }
}
