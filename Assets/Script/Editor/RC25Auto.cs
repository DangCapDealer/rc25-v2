using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Codice.Client.BaseCommands.Import.Commit;

public class RC25Auto : EditorWindow
{

    static string themeName = "Monster";

    [MenuItem("Auto/Custom Prefabs")]
    public static void ShowWindow()
    {
        GetWindow<RC25Auto>("Text Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Editor Window - Import nhân vật mới", EditorStyles.boldLabel);
        themeName = EditorGUILayout.TextField("Tên chủ đề mới:", themeName);

        GUILayout.Space(10);
        GUILayout.Label("Tên chủ đề vừa tạo:");
        EditorGUILayout.HelpBox(themeName, MessageType.Info);

        if (GUILayout.Button("Import Prefabs vào scene"))
        {
            LoadPrefabs();
        }

        if (GUILayout.Button("Tạo animation Prefabs trong scene"))
        {
            CheckingPrefabs();
        }

        if (GUILayout.Button("Xoá animation Prefabs trong scene"))
        {
            RemovePrefabs();
        }
    }

    static void RemovePrefabs()
    {
        StaticVariable.ClearLog();

        int loadedCount = 0;
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject obj in rootObjects)
        {
            if (obj.name.Contains("Camera")) continue;

            var foundFolder = CheckFolderExists(obj.name);
            if (foundFolder == false) continue;
            loadedCount++;

            bool hasMonsterChild = false;
            Transform monsterChild = null;
            foreach (Transform child in obj.transform)
            {
                if (child.name.Equals("Monster", System.StringComparison.OrdinalIgnoreCase))
                {
                    hasMonsterChild = true;
                    monsterChild = child;
                    break;
                }
            }

            if (hasMonsterChild)
            {
                Debug.Log("Object đã có child tên 'Monster'.");
                DestroyImmediate(monsterChild.gameObject);
                continue;
            }
        }
    }    

    static void CheckingPrefabs()
    {
        StaticVariable.ClearLog();

        int loadedCount = 0;
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject obj in rootObjects)
        {
            if (obj.name.Contains("Camera")) continue;

            var foundFolder = CheckFolderExists(obj.name);
            if (foundFolder == false) return;
            loadedCount++;

            bool hasMonsterChild = false;
            foreach (Transform child in obj.transform)
            {
                if (child.name.Equals("Monster", System.StringComparison.OrdinalIgnoreCase))
                {
                    hasMonsterChild = true;
                    break;
                }
            }

            if (hasMonsterChild)
            {
                Debug.Log("Object đã có child tên 'Monster'.");
                continue;
            }

            string spineAssetPath = $"Assets/Animation/Monster/{obj.name}/export/skeleton_SkeletonData.asset";
            SkeletonDataAsset spineAsset = AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(spineAssetPath);
            if (spineAsset == null)
            {
                Debug.LogError("Không tìm thấy Spine asset tại: " + spineAssetPath);
                continue;
            }

            GameObject monsterChild = new GameObject("Monster");
            monsterChild.transform.SetParent(obj.transform);
            monsterChild.transform.localPosition = Vector3.zero.WithY(-2.38f);
            monsterChild.transform.localScale = VectorExtensions.Create3D(1.2f, 1.2f, 1.2f);

            // Thêm component SkeletonAnimation và gán Spine asset
            SkeletonAnimation skeletonAnimation = monsterChild.AddComponent<SkeletonAnimation>();
            skeletonAnimation.skeletonDataAsset = spineAsset;
            skeletonAnimation.Initialize(true);

            var skeletonData = spineAsset.GetSkeletonData(true);
            if (skeletonData != null && skeletonData.Animations.Count > 0)
            {
                string defaultAnimation = skeletonData.Animations.Items[0].Name;
                skeletonAnimation.AnimationName = defaultAnimation;
                skeletonAnimation.loop = true;
                Debug.Log("Default Animation: " + defaultAnimation);
            }
            else
            {
                Debug.LogWarning("SkeletonData không chứa animation nào.");
            }

            Renderer rend = skeletonAnimation.GetComponent<Renderer>();
            rend.sortingLayerName = "Character";
            rend.sortingOrder = 3;
            monsterChild.Hide();

            if (PrefabUtility.IsPartOfPrefabInstance(obj))
            {
                PrefabUtility.ApplyPrefabInstance(obj, InteractionMode.UserAction);
                Debug.Log($"Đã lưu các thay đổi của prefab {obj.name} về prefab asset gốc.");
            }
            else
            {
                Debug.LogWarning($"Object được chọn không phải là prefab {obj.name}.");
            }
        }

        Debug.Log("Tổng số object active: " + loadedCount);
    }    

    static void LoadPrefabs()
    {
        string folderPath = "Assets/Prefabs"; // Thư mục chứa Prefabs
        string[] prefabFiles = Directory.GetFiles(folderPath, "*.prefab", SearchOption.AllDirectories);
        int loadedCount = 0;

        foreach (string prefabFile in prefabFiles)
        {
            string assetPath = prefabFile.Replace("\\", "/"); // Fix đường dẫn trên Windows
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (prefab != null)
            {
                if (prefab.name.Contains("Admob") ||
                    prefab.name.Contains("Canvas") ||
                    prefab.name.Contains("Base") ||
                    prefab.name.Contains("Sound") ||
                    prefab.name.Contains("Black")) continue;

                if (GameObject.Find(prefab.name) != null) continue;

                // Instantiate Prefab vào Scene
                PrefabUtility.InstantiatePrefab(prefab);
                loadedCount++;
            }
        }

        Debug.Log($"Đã load {loadedCount} prefabs vào scene.");
    }

    static bool CheckFolderExists(string objName)
    {
        string folderPath = Path.Combine($"Assets/Animation/{themeName}", objName);

        if (Directory.Exists(folderPath))
        {
            return true;
        }
        else
        {
            Debug.Log("Không tìm thấy folder: " + folderPath);
            return false;
        }
    }
}
