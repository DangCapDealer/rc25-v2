using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;


public class MenuEditor
{
    [UnityEditor.MenuItem("Tools/Button Event Finder")]
    static void ButtonEventFinder()
    {
        var currentScene = SceneManager.GetActiveScene();
        for (int i = 0; i < currentScene.rootCount; i++)
        {
            GameObject obj = currentScene.GetRootGameObjects()[i];
            if (obj.name == "Canvas")
                ButtonFinder(obj.transform);
        }
    }

    static void ButtonFinder(Transform parent)
    {
        parent.ForChild((child) =>
        {
            var button = child.GetComponent<Button>();
            if (button != null)
                GetButtonEvents(button);
            ButtonFinder(child);
        });
    }

    static void GetButtonEvents(Button button)
    {
        int onClickEventsCount = button.onClick.GetPersistentEventCount();
        for (int i = 0; i < onClickEventsCount; i++)
        {
            string methodName = button.onClick.GetPersistentMethodName(i);
            Object targetObject = button.onClick.GetPersistentTarget(i);
            Debug.Log($"Button in {button.gameObject.name}, Target: {targetObject}.{methodName}");
        }
    }

    [UnityEditor.MenuItem("Tools/Remove all momobehavier")]
    private static void RemoveAllMonobehavier()
    {
        GameObject[] allObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var _object in allObjects)
        {
            FindInGO(_object);
        }
    }

    private static void FindInGO(GameObject g)
    {
        MonoBehaviour[] scripts = g.GetComponents<MonoBehaviour>(); 
        foreach (MonoBehaviour script in scripts) 
        {
            try
            {
                Object.DestroyImmediate(script);
            }
            catch (System.Exception)
            {
                Debug.Log(script.GetType());
            }

        }

        foreach (Transform childT in g.transform)
        {
            FindInGO(childT.gameObject);
        }
    }

    [UnityEditor.MenuItem("Tools/Rename Object")]
    static void RenameObject()
    {
        var selectedObjects = Selection.transforms;

        if (selectedObjects.Length > 0)
        {
            Debug.Log("Selected Objects:");
            foreach (Transform _obj in selectedObjects)
            {
                FindChildAndRename(_obj);
            }
        }
    }

    private static void FindChildAndRename(Transform _obj)
    {
        for(int i = 0; i < _obj.childCount; i++)
        {
            var child = _obj.GetChild(i);
            var baseNames = child.name.Split(':');
            child.name = baseNames[baseNames.Length - 1];
            FindChildAndRename (child);
        }
    }

    [MenuItem("Tools/Scene/Loading %s1")]
    static void OpenLoadingScene()
    {
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        EditorSceneManager.OpenScene("Assets/Scenes/Loading.unity");
    }

    [MenuItem("Tools/Scene/Game %s2")]
    static void OpenGameScene()
    {
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        EditorSceneManager.OpenScene("Assets/Scenes/Game.unity");
    }

    [MenuItem("Tools/Scene/Change %`")]
    static void ChangeScene()
    {
        var scene = SceneManager.GetActiveScene();
        switch(scene.name)
        {
            case "Loading":
                OpenGameScene();
                break;
            case "Game":
                OpenLoadingScene();
                break;
        }    

        Debug.Log("Active Scene is '" + scene.name + "'.");
    }

    [MenuItem("Tools/Checking Material")]
    static void ModifyMaterialName()
    {
        StaticVariable.ClearLog();
        string[] guids = AssetDatabase.FindAssets("t:Material");

        List<Material> materials = new List<Material>();
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (path.EndsWith(".mat") == true)
                materials.Add(mat);
        }

        HashSet<string> uniqueNames = new HashSet<string>();
        List<string> duplicateNames = new List<string>();

        // Duyệt qua tất cả các Material trong mảng
        foreach (Material mat in materials)
        {
            if (!uniqueNames.Add(mat.name)) // Nếu tên đã tồn tại trong HashSet
            {
                duplicateNames.Add(mat.name); // Thêm vào danh sách tên trùng
                string assetPath = AssetDatabase.GetAssetPath(mat);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    Debug.Log("Material: " + mat.name + " - Path: " + assetPath);
                }
            }
        }
    }
    static void ReadChildObjects(Transform parentTransform)
    {
        foreach (Transform childTransform in parentTransform)
        {
            GameObject childObject = childTransform.gameObject;
            var tmp = childObject.GetComponent<TextMeshProUGUI>();
            if (tmp != null)
            {
                var _tex = tmp.text;
                var _color = tmp.color;
                var _fontSize = tmp.fontSize;

                Undo.DestroyObjectImmediate(tmp);
                Debug.Log("Component removed");

                var text = childObject.AddComponent<Text>();
                text.text = _tex;
                text.color = _color;
                text.fontSize = (int)_fontSize;

                EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                Debug.Log("Scene saved.");
            }



            ReadChildObjects(childTransform);
        }
    }

    [MenuItem("Tools/Set Keystore Password")]
    public static void SetPassword()
    {
        //PlayerSettings.Android.useCustomKeystore = true;
        PlayerSettings.Android.keystorePass = "123456";
        PlayerSettings.Android.keyaliasPass = "123456";

        //AssetDatabase.SaveAssets();

        UnityEngine.Debug.Log("Keystore settings updated successfully!");
    }
}
