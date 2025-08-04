using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using static Codice.Client.BaseCommands.Import.Commit;

public class RC25Auto : EditorWindow
{

    static string themeName = "Monster";
    static string basePath = "Assets/Animation/{0}/{1}/export/";
    static bool isSpine = true;

    [MenuItem("Auto/Custom Prefabs")]
    public static void ShowWindow()
    {
        GetWindow<RC25Auto>("Text Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Editor Window - Import nhân vật mới", EditorStyles.boldLabel);
        themeName = EditorGUILayout.TextField("Tên chủ đề mới:", themeName);
        basePath = EditorGUILayout.TextField("Template path:", basePath);
        isSpine = EditorGUILayout.Toggle("Có dùng spine không:", isSpine);

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
        if (GUILayout.Button("Save all animation Prefabs trong scene"))
        {
            SaveAllPrefabs();
        }
    }

    static void SaveAllPrefabs()
    {
        StaticVariable.ClearLog();

        //int loadedCount = 0;
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject obj in rootObjects)
        {
            if (obj.name.Contains("Camera")) continue;

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
                if (child.name.Equals(themeName, System.StringComparison.OrdinalIgnoreCase))
                {
                    hasMonsterChild = true;
                    monsterChild = child;
                    break;
                }
            }

            if (hasMonsterChild)
            {
                Debug.Log($"Object đã có child tên '{themeName}'.");
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

        rootObjects.SimpleForEach((obj, i) =>
        {
            if (obj.name.Contains("Camera")) return;

            var foundFolder = CheckFolderExists(obj.name);
            if (foundFolder == false) return;
            loadedCount++;

            bool hasMonsterChild = false;
            foreach (Transform child in obj.transform)
            {
                if (child.name.Equals(themeName, System.StringComparison.OrdinalIgnoreCase))
                {
                    hasMonsterChild = true;
                    break;
                }
            }

            if (hasMonsterChild)
            {
                Debug.Log($"Object đã có child tên '{themeName}'.");
                return;
            }

            if (isSpine) loadSpine(obj);
            else loadSprite(obj, i);
        });

        Debug.Log("Tổng số object active: " + loadedCount);
    }   

    private static void loadSprite(GameObject obj, int index)
    {
        string spriteAssetPath = $"{string.Format(basePath, themeName, index)}.png";
        //Debug.Log(spriteAssetPath);
        var spriteAsset = AssetDatabase.LoadAssetAtPath<Sprite>(spriteAssetPath);
        if (spriteAsset == null )
        {
            Debug.LogError("Không tìm thấy Spine asset tại: " + spriteAssetPath);
            return;
        }

        GameObject monsterChild = new GameObject(themeName);
        monsterChild.transform.SetParent(obj.transform);
        monsterChild.transform.localPosition = Vector3.zero.WithY(-0.4f);
        monsterChild.transform.localScale = VectorExtensions.Create3D(1.2f, 1.2f, 1.2f);

        // Thêm component SkeletonAnimation và gán Spine asset
        SpriteRenderer spriteAnimation = monsterChild.AddComponent<SpriteRenderer>();
        spriteAnimation.sprite = spriteAsset;
        spriteAnimation.sortingLayerName = "Character";
        spriteAnimation.sortingOrder = 3;

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
    
    private static void loadSpine(GameObject obj)
    {
        //string spineAssetPath = $"Assets/Animation/{themeName}/{obj.name}/export/skeleton_SkeletonData.asset";
        string spineAssetPath = $"{string.Format(basePath, themeName, obj.name)}/skeleton_SkeletonData.asset";
        SkeletonDataAsset spineAsset = AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(spineAssetPath);
        if (spineAsset == null)
        {
            Debug.LogError("Không tìm thấy Spine asset tại: " + spineAssetPath);
            return;
        }

        GameObject monsterChild = new GameObject(themeName);
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
                if (prefab.name.ToLower().Contains("admob") ||
                    prefab.name.ToLower().Contains("canvas") ||
                    prefab.name.ToLower().Contains("base") ||
                    prefab.name.ToLower().Contains("sound") ||
                    prefab.name.ToLower().Contains("black")) continue;

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
        if (isSpine == false) return true;

        string folderPath = Path.Combine($"Assets/Animation/{themeName}", objName);
        if (Directory.Exists(folderPath))
        {
            return true;
        }
        else
        {
            LogSystem.LogError("Không tìm thấy folder: " + folderPath);
            return false;
        }
    }
}
