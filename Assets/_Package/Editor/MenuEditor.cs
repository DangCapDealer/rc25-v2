using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEditor.Build.Reporting;


public class MenuEditor
{
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
    [MenuItem("Tools/Import Keystore Password")]
    public static void SetPassword()
    {
        PlayerSettings.Android.keystorePass = "123456";
        PlayerSettings.Android.keyaliasPass = "123456";
        UnityEngine.Debug.Log("Keystore settings updated successfully!");
    }

        [MenuItem("Tools/Quick Screenshot")]
    private static void QuickScreenshot()
    {
        ScreenCapture.CaptureScreenshot(Application.dataPath + "/Screenshot.png");
        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/Version/Version and Version Code")]
    public static void LogVersionAndVersionCode()
    {
        string currentDateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        Debug.Log($"📱 Product: {PlayerSettings.productName}");
        Debug.Log($"📅 DateTime: {currentDateTime}");
        Debug.Log($"🔢 App Version: {PlayerSettings.bundleVersion}");
        Debug.Log($"⚡ Version Code: {PlayerSettings.Android.bundleVersionCode}");
        Debug.Log($"📦 Bundle ID: {PlayerSettings.applicationIdentifier}");
    }   

    [MenuItem("Tools/Version/Increment Version and Version Code")]
    public static void ChangeVersionAndVersionCode()
    {
        var vCode = PlayerSettings.Android.bundleVersionCode;
        vCode += 1;
        
        string newVersion = "";
        
        if (vCode < 10)
        {
            // 0.0.1 đến 0.0.9
            newVersion = $"0.0.{vCode}";
        }
        else if (vCode < 100)
        {
            // 0.1.0 đến 0.9.9
            int tens = vCode / 10;
            int ones = vCode % 10;
            newVersion = $"0.{tens}.{ones}";
        }
        else if (vCode < 1000)
        {
            // 1.0.0 đến 9.9.9
            int hundreds = vCode / 100;
            int remainder = vCode % 100;
            int tens = remainder / 10;
            int ones = remainder % 10;
            newVersion = $"{hundreds}.{tens}.{ones}";
        }
        else
        {
            // Reset về 1.0.0 nếu vượt quá 999
            vCode = 100;
            newVersion = "1.0.0";
            Debug.LogWarning("Version code exceeded 999, reset to 1.0.0");
        }
        
        // Cập nhật PlayerSettings
        PlayerSettings.Android.bundleVersionCode = vCode;
        PlayerSettings.bundleVersion = newVersion;
        
        Debug.Log($"✅ Version updated: {newVersion} (Code: {vCode})");
    }
    private const string CORRECT_PASSWORD = "123456";


    [MenuItem("Tools/Build And Run Android")]
    public static void BuildAndRunAndroidWithPassword()
    {
        OnBuild(BuildOptions.AutoRunPlayer);
    }

    [MenuItem("Tools/Build Android")]
    public static void BuildAndroidWithPassword()
    {
        OnBuild(BuildOptions.None);
    }

    [MenuItem("Tools/Clear Cache and Build Android")]
    public static void ClearCacheAndBuildAndroid()
    {
        OnBuild(BuildOptions.None);
    }

    [MenuItem("Tools/Build Android App Bundle")]
    public static void BuildAndroidAppBundle()
    {
        ChangeVersionAndVersionCode();
        OnBuild(BuildOptions.None, true);
    }

    private static void ClearAllCache()
    {
        Debug.Log("🧹 Clearing all Unity cache...");
        
        // Clear Asset Database
        AssetDatabase.DeleteAsset("Library/ArtifactDB");
        AssetDatabase.DeleteAsset("Library/SourceAssetDB");
        
        // Clear GI Cache
        Lightmapping.Clear();
        
        // Clear Shader Cache - SỬA LỖI
        // Xóa tất cả shader cache
        Shader[] allShaders = Resources.FindObjectsOfTypeAll<Shader>();
        foreach (Shader shader in allShaders)
        {
            ShaderUtil.ClearCachedData(shader);
        }
        
        // Force refresh
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        
        Debug.Log("✅ Cache cleared!");
    }

    private static void OnBuild(BuildOptions _options, bool _buildAppBundle = false)
    {
        try
        {
            // Save current scenes trước khi build
            EditorSceneManager.SaveOpenScenes();
            
            // Kiểm tra và tạo thư mục Builds
            string buildsFolder = "Builds";
            if (!Directory.Exists(buildsFolder))
            {
                Directory.CreateDirectory(buildsFolder);
                Debug.Log("Created Builds directory");
            }

            // Đảm bảo Android platform được chọn - CHỜ ĐỦ THỜI GIAN
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                Debug.Log("Switching to Android platform...");
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                
                // CHỜ PLATFORM SWITCH HOÀN TẤT
                while (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
                {
                    System.Threading.Thread.Sleep(100);
                }
                Debug.Log("Platform switched successfully!");
            }

            // FORCE BUILD APK THAY VÌ AAB
            EditorUserBuildSettings.buildAppBundle = _buildAppBundle;
            Debug.Log("Forced APK build (not AAB)");

            // Set keystore configuration
            PlayerSettings.Android.keystorePass = CORRECT_PASSWORD;
            PlayerSettings.Android.keyaliasPass = CORRECT_PASSWORD;
            
            // Xóa APK cũ và ĐẢM BẢO XÓA HOÀN TẤT
            string app = $"{PlayerSettings.productName}_{PlayerSettings.bundleVersion}_{PlayerSettings.Android.bundleVersionCode}_{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}";
            app = app.Replace(" ", "_").Replace(":", "_").Replace("-", "_");
            string fileExtension = _buildAppBundle ? ".aab" : ".apk";
            string path = $"Builds/{app}{fileExtension}";
            if (File.Exists(path))
            {
                long oldSize = new FileInfo(path).Length;
                Debug.Log($"Old APK size: {oldSize / (1024f * 1024f):F2} MB");
                
                File.Delete(path);
                // Chờ file bị xóa hoàn toàn
                while (File.Exists(path))
                {
                    System.Threading.Thread.Sleep(50);
                }
                Debug.Log("Deleted old APK file");
            }
            
            // Kiểm tra scenes
            string[] scenes = GetEnabledScenes();
            if (scenes.Length == 0)
            {
                Debug.LogError("No enabled scenes found in Build Settings!");
                return;
            }
            
            Debug.Log($"Building with scenes: {string.Join(", ", scenes)}");

            BuildPlayerOptions buildOptions = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = path,
                target = BuildTarget.Android,
                options = _options | BuildOptions.CleanBuildCache | BuildOptions.StrictMode // FORCE CLEAN BUILD
            };

            Debug.Log($"Starting CLEAN APK build to: {path}");
            var startTime = System.DateTime.Now;
            BuildReport buildReport = BuildPipeline.BuildPlayer(buildOptions);
            var buildTime = System.DateTime.Now - startTime;
            
            // KIỂM TRA FILE APK THỰC SỰ TỒN TẠI VÀ KHÔNG BỊ CORRUPT
            if (buildReport.summary.result == BuildResult.Succeeded)
            {
                // Chờ file được tạo hoàn toàn
                int waitCount = 0;
                while (!File.Exists(path) && waitCount < 50)
                {
                    System.Threading.Thread.Sleep(100);
                    waitCount++;
                }
                
                if (File.Exists(path))
                {
                    long fileSizeBytes = new FileInfo(path).Length;
                    float fileSizeMB = fileSizeBytes / (1024f * 1024f);
                    
                    // CẢNH BÁO NẾU BUILD QUÁ NHANH (CÓ THỂ BỊ CACHE)
                    if (buildTime.TotalSeconds < 30)
                    {
                        LogSystem.LogError($"⚠️ Build finished very quickly ({buildTime.TotalSeconds:F1}s) - might be using cached build!");
                        LogSystem.LogError($"Try using 'Clear Cache and Build Android' for a clean build!");
                    }
                    
                    Debug.Log($"✅ APK Build succeeded! APK Size: {fileSizeMB:F2} MB ({fileSizeBytes} bytes)");
                    Debug.Log($"Build time: {buildTime.TotalSeconds:F1} seconds");
                    Debug.Log($"Unity reported build time: {buildReport.summary.totalTime.TotalSeconds:F1} seconds");
                    Debug.Log($"Warnings: {buildReport.summary.totalWarnings}");
                    Debug.Log($"Errors: {buildReport.summary.totalErrors}");
                    
                    OpenFolder();
                }
                else
                {
                    Debug.LogError("❌ Build reported success but APK file not found!");
                }
            }
            else
            {
                Debug.LogError($"❌ Build failed with result: {buildReport.summary.result}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Build error: {e.Message}\n{e.StackTrace}");
        }
    }

    static string[] GetEnabledScenes()
    {
        return EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();
    }

    [MenuItem("Tools/Open Specific Folder in Assets")]
    public static void OpenFolder()
    {
        string folderPath = $"Builds/";
        string fullPath = Path.GetFullPath(folderPath);
        if (Directory.Exists(fullPath))
        {
            EditorUtility.RevealInFinder(fullPath); // Mở trong Explorer/Finder
        }
    }
}
