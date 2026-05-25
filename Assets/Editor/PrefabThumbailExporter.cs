#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class PrefabThumbnailExporter
{
    [MenuItem("Assets/Export Prefab Thumbnail", true)]
    static bool ValidateExport()
    {
        return Selection.activeObject is GameObject;
    }

    [MenuItem("Assets/Export Prefab Thumbnail")]
    static void ExportThumbnail()
    {
        GameObject prefab = Selection.activeObject as GameObject;

        Texture2D preview = AssetPreview.GetAssetPreview(prefab);

        if (preview == null)
        {
            Debug.LogWarning("Preview not ready. Try again in a second.");
            return;
        }

        // Save dialog
        string path = EditorUtility.SaveFilePanel(
            "Save Prefab Thumbnail",
            "",
            prefab.name,
            "jpg"
        );

        if (string.IsNullOrEmpty(path)) return;

        Texture2D readable = new Texture2D(preview.width, preview.height, TextureFormat.RGB24, false);
        readable.SetPixels(preview.GetPixels());
        readable.Apply();

        File.WriteAllBytes(path, readable.EncodeToJPG(95));

        Debug.Log("Thumbnail exported: " + path);
    }
}
#endif