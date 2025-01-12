using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.IO;

public class FindMissingScripts : EditorWindow
{
    [MenuItem("Window/_Package Tool")]
    public static void ShowWindow()
    {
        var _windows = EditorWindow.GetWindow(typeof(FindMissingScripts), true, "_Package Tool");
        _windows.minSize = new Vector2(400, 800);
        _windows.maxSize = new Vector2(400, 800);
    }

    private bool isSavePrefab = false;
    private string savePath = "";

    public void OnGUI()
    {
        if (GUILayout.Button("Remove Missing Script", GUILayout.Height(30)))
        {
            ClearLog();
            RemoveAllMissingScript();
        }
        GUILayout.BeginVertical();
        if (GUILayout.Button("Save Prefab On Path", GUILayout.Height(30)))
        {
            ClearLog();
            isSavePrefab = !isSavePrefab;
            savePath = "";
            //SavePrefabOnPath();
        }
        if(isSavePrefab)
        {
            GUILayout.BeginHorizontal(GUILayout.Height(20));
            EditorGUILayout.LabelField("  Save Path:", GUILayout.Width(80));
            savePath = EditorGUILayout.TextField("", savePath);
            GUILayout.EndHorizontal();
            if(GUILayout.Button("Save"))
            {
                ClearLog();
                SavePrefabOnPath(savePath);
            }
        }    
        GUILayout.EndVertical();
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Active Firebase", GUILayout.Height(30)))
        {
            ClearLog();
#if UNITY_ANDROID
            var _defineAndroid = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
            var _defineArray = _defineAndroid.Split(';');
            _defineArray = _defineArray.Add("FIREBASE_ENABLE");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, _defineArray);
            Debug.Log(PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android));
#endif
        }
        if (GUILayout.Button("Remove Firebase", GUILayout.Height(30)))
        {
            ClearLog();
#if UNITY_ANDROID
            var _defineAndroid = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
            if (_defineAndroid.Contains("FIREBASE_ENABLE"))
            {
                _defineAndroid = _defineAndroid.Replace("FIREBASE_ENABLE", "");
                var _defineArray = _defineAndroid.Split(';');
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, _defineArray);
                Debug.Log(PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android));
            }
#endif
            //var _defineIOS = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Active Applovin", GUILayout.Height(30)))
        {
            ClearLog();
#if UNITY_ANDROID
            var _defineAndroid = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
            var _defineArray = _defineAndroid.Split(';');
            _defineArray = _defineArray.Add("APPLOVIN_ENABLE");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, _defineArray);
            Debug.Log(PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android));
#endif
        }
        if (GUILayout.Button("Remove Applovin", GUILayout.Height(30)))
        {
            ClearLog();
#if UNITY_ANDROID
            var _defineAndroid = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
            if (_defineAndroid.Contains("APPLOVIN_ENABLE"))
            {
                _defineAndroid = _defineAndroid.Replace("APPLOVIN_ENABLE", "");
                var _defineArray = _defineAndroid.Split(';');
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, _defineArray);
                Debug.Log(PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android));
            }
#endif
            //var _defineIOS = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Active IAP", GUILayout.Height(30)))
        {
            ClearLog();
#if UNITY_ANDROID
            var _defineAndroid = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
            var _defineArray = _defineAndroid.Split(';');
            _defineArray = _defineArray.Add("IAP_ENABLE");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, _defineArray);
            Debug.Log(PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android));
#endif
        }
        if (GUILayout.Button("Remove IAP", GUILayout.Height(30)))
        {
            ClearLog();
#if UNITY_ANDROID
            var _defineAndroid = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
            if (_defineAndroid.Contains("IAP_ENABLE"))
            {
                _defineAndroid = _defineAndroid.Replace("IAP_ENABLE", "");
                var _defineArray = _defineAndroid.Split(';');
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, _defineArray);
                Debug.Log(PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android));
            }
#endif
            //var _defineIOS = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Get All Define", GUILayout.Height(30)))
        {
            ClearLog();
#if UNITY_ANDROID
            Debug.Log(PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android));
#endif
        }

        if (GUILayout.Button("Remove All Define", GUILayout.Height(30)))
        {
            ClearLog();
#if UNITY_ANDROID
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "");
#endif
        }
        GUILayout.EndHorizontal();
    }
    private static void RemoveAllMissingScript()
    {
        GameObject[] go = Selection.gameObjects;
        go_count = 0;
        components_count = 0;
        missing_count = 0;
        foreach (GameObject g in go)
        {
            FindInGOAndRemove(g);
            //PrefabUtility.ApplyObjectOverride(g, "Assets/_Asset/Assets/_Enemys", InteractionMode.AutomatedAction);
            EditorUtility.SetDirty(g);
        }
        AssetDatabase.Refresh();
        Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));
    }

    private static void UpdateSpriteFolderPath(string pathSprite, string pathPrefab, Material material)
    {
        if (!Directory.Exists(pathPrefab))
            return;
        if (!Directory.Exists(pathSprite))
            return;

        var folderSprites = Directory.GetFiles(pathSprite);
        var folderPrefabs = Directory.GetFiles(pathPrefab);

        //FindPrefabPathWithName(folderPrefabs, "");
        for(int i = 0; i < folderPrefabs.Length; i++)
        {
            var _datas = folderPrefabs[i].Split('.');
            if (_datas[_datas.Length - 1] == "meta")
                continue;
            var prefab = (GameObject)AssetDatabase.LoadAssetAtPath(folderPrefabs[i], typeof(GameObject));
            var spriteRenderers = prefab.GetComponentsInChildren<SpriteRenderer>();
            spriteRenderers.ForEach((sr) =>
            {
                var _spritePath = FindSpritePathWithName(folderSprites, sr.name);
                if(string.IsNullOrEmpty(_spritePath))
                {
                    Debug.Log($"name  {sr.name} prefab {prefab.name}");
                }
                else
                {
                    var texture = (Sprite)AssetDatabase.LoadAssetAtPath(_spritePath, typeof(Sprite));
                    sr.sprite = texture;
                }
                if (sr.material == null)
                    sr.material = material;
            });

            EditorUtility.SetDirty(prefab);
        }
        AssetDatabase.Refresh();
    }

    private static string FindSpritePathWithName(string[] lst, string name)
    {
        for(int i = 0; i < lst.Length; i++)
        {
            var _datas = lst[i].Split('.');
            if (_datas[_datas.Length - 1] == "meta")
                continue;
            var _data1s = lst[i].Split('/');
            if (_data1s[_data1s.Length - 1].Replace(".asset", "") == name)
                return lst[i];
        }
        return "";
    }

    private static string FindPrefabPathWithName(string[] lst, string name)
    {
        for (int i = 0; i < lst.Length; i++)
        {
            var _datas = lst[i].Split('.');
            if (_datas[_datas.Length - 1] == "meta")
                continue;
            Debug.Log(lst[i]);
        }
        return "";
    }

    private static void SavePrefabOnPath(string path)
    {
        if (!Directory.Exists(path))
        {
            var paths = path.Split('/');
            for(int index = 0; index < paths.Length; index++)
            {
                if (index > 0)
                {
                    string cachePath = "Assets/";
                    for (int i_index = 1; i_index <= index; i_index++)
                    {
                        cachePath += paths[i_index] + "/";
                    }
                    if(!Directory.Exists(cachePath))
                    {
                        var cachePaths = cachePath.Split('/');
                        //string _parentCreateFolder = "";
                        string _nameCreateFolder = "";
                        string _pathCreateFolder = "";
                        for (int i_cachePaths = 0; i_cachePaths < cachePaths.Length; i_cachePaths++)
                        {
                            if (!string.IsNullOrEmpty(cachePaths[i_cachePaths]))
                            {
                                if(i_cachePaths == cachePaths.Length - 2)
                                {
                                    _nameCreateFolder = cachePaths[i_cachePaths];
                                }
                                else
                                {
                                    _pathCreateFolder += cachePaths[i_cachePaths] + "/";
                                }
                                //if (i_cachePaths == cachePaths.Length - 3)
                                //{
                                //    _parentCreateFolder = cachePaths[i_cachePaths];
                                //}

                            }
                        }
                        _pathCreateFolder = _pathCreateFolder.Remove(_pathCreateFolder.Length - 1, 1);
                        AssetDatabase.CreateFolder(_pathCreateFolder, _nameCreateFolder);
                        AssetDatabase.Refresh(ImportAssetOptions.Default);
                    }
                }
            }
        }

        GameObject[] gameObjects = Selection.gameObjects;
        foreach (GameObject gameObject in gameObjects)
        {
            //Create folder Prefabs and set the path as within the Prefabs folder,
            // and name it as the GameObject's name with the .Prefab format
            string localPath = $"{path}/{gameObject.name}.prefab";

            // Make sure the file name is unique, in case an existing Prefab has the same name.
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

            // Create the new Prefab and log whether Prefab was saved successfully.
            bool prefabSuccess;
            PrefabUtility.SaveAsPrefabAsset(gameObject, localPath, out prefabSuccess);
            if (prefabSuccess == false)
                Debug.Log("Prefab failed to save" + prefabSuccess);
        }
    }    

    // change script
    private static void ChangeInSelected()
    {
        GameObject[] go = Selection.gameObjects;
        foreach (GameObject g in go)
        {
            Component[] components = g.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    DestroyImmediate(components[i]);
                    //g.AddComponent<ItemControl>();
                }
            }
        }
    }

    // find on selected
    private static void FindInSelected()
    {
        GameObject[] go = Selection.gameObjects;
        int go_count = 0, components_count = 0, missing_count = 0;
        foreach (GameObject g in go)
        {
            go_count++;
            Component[] components = g.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                components_count++;
                if (components[i] == null)
                {
                    missing_count++;
                    string s = g.name;
                    Transform t = g.transform;
                    while (t.parent != null)
                    {
                        s = t.parent.name + "/" + s;
                        t = t.parent;
                    }
                    Debug.Log(s + " has an empty script attached in position: " + i, g);
                }
            }
        }

        Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));
    }

    // find on selected and child
    private static int go_count = 0, components_count = 0, missing_count = 0;
    private static void FindInSelectedAndChil()
    {
        GameObject[] go = Selection.gameObjects;
        go_count = 0;
        components_count = 0;
        missing_count = 0;
        foreach (GameObject g in go)
        {
            FindInGO(g);
        }
        Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", go_count, components_count, missing_count));
    }

    private static void FindInGO(GameObject g)
    {
        go_count++;

        Component[] components = g.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            components_count++;
            if (components[i] == null)
            {
                missing_count++;
                string s = g.name;
                Transform t = g.transform;
                while (t.parent != null)
                {
                    s = t.parent.name + "/" + s;
                    t = t.parent;
                }
                Debug.Log(s + " has an empty script attached in position: " + i, g);
            }
        }
        // Now recurse through each child GO (if there are any):
        foreach (Transform childT in g.transform)
        {   
            FindInGO(childT.gameObject);
        }
    }

    private static void FindInGOAndRemove(GameObject g)
    {
        go_count++;

        int count = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(g);
        if (count != 0)
            Debug.Log($"Removed {count} missing scripts");

        foreach (Transform childT in g.transform)
        {
            FindInGOAndRemove(childT.gameObject);
        }

        Debug.Log(g.name);
    }

    public void ClearLog()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
}