#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

public class ScenePrefabThumbnailGenerator : EditorWindow
{
    private List<GameObject> prefabs = new List<GameObject>();
    private int currentIndex = 0;

    private string rootOutputPath;

    [MenuItem("Tools/Generate Scene Prefab Thumbnails (JPG)")]
    public static void ShowWindow()
    {
        var window = GetWindow<ScenePrefabThumbnailGenerator>();
        window.titleContent = new GUIContent("Thumbnail Generator");
        window.StartProcess();
    }

    void StartProcess()
    {
        prefabs.Clear();
        currentIndex = 0;

        // 🔥 ROOT DIRECTORY (outside Assets)
        rootOutputPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "ScenePrefabThumbnails");

        if (!Directory.Exists(rootOutputPath))
        {
            Directory.CreateDirectory(rootOutputPath);
        }

        HashSet<GameObject> set = new HashSet<GameObject>();

        foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            CollectPrefabs(root.transform, set);
        }

        prefabs.AddRange(set);

        Debug.Log($"Found {prefabs.Count} prefabs. Saving to: {rootOutputPath}");

        EditorApplication.update += Process;
    }

    void Process()
    {
        if (currentIndex >= prefabs.Count)
        {
            EditorApplication.update -= Process;
            Debug.Log("✅ Thumbnail generation complete!");
            return;
        }

        GameObject prefab = prefabs[currentIndex];

        Texture2D preview = AssetPreview.GetAssetPreview(prefab);

        // Wait until Unity finishes generating preview
        if (preview == null)
            return;

        SaveAsJPG(prefab.name, preview);

        currentIndex++;
    }

    void SaveAsJPG(string name, Texture2D tex)
{
    // Convert to readable texture
    Texture2D readableTex = new Texture2D(tex.width, tex.height, TextureFormat.RGB24, false);
    readableTex.SetPixels(tex.GetPixels());
    readableTex.Apply();

    byte[] jpg = readableTex.EncodeToJPG(95);

    // 🔥 Generate unique file path
    string basePath = Path.Combine(rootOutputPath, name);
    string path = basePath + ".jpg";

    int counter = 1;

    while (File.Exists(path))
    {
        path = basePath + "_" + counter + ".jpg";
        counter++;
    }

    File.WriteAllBytes(path, jpg);

    Debug.Log($"Saved JPG: {path}");
}

    void CollectPrefabs(Transform t, HashSet<GameObject> set)
    {
        GameObject source = PrefabUtility.GetCorrespondingObjectFromSource(t.gameObject);

        if (source != null)
        {
            set.Add(source);
        }

        foreach (Transform child in t)
        {
            CollectPrefabs(child, set);
        }
    }
}
#endif