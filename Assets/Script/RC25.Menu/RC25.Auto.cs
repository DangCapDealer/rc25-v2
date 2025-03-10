using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class RC25Auto : EditorWindow
{
    [MenuItem("Tools/Load Prefabs To Scene")]
    static void LoadPrefabs()
    {
        string folderPath = "Assets/Prefabs"; // Đường dẫn thư mục chứa Prefabs
        string[] prefabFiles = Directory.GetFiles(folderPath, "*.prefab", SearchOption.AllDirectories);

        foreach (string prefabFile in prefabFiles)
        {
            string assetPath = prefabFile.Replace("\\", "/"); // Fix đường dẫn trên Windows
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (prefab != null)
            {
                // Instantiate Prefab vào Scene
                PrefabUtility.InstantiatePrefab(prefab);
            }
        }

        Debug.Log($"Loaded {prefabFiles.Length} prefabs into the scene.");
    }
}
