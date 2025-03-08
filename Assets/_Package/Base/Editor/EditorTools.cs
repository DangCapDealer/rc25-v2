using UnityEngine;
using System.Diagnostics;

using UnityEditor.Build.Reporting;
using System.Linq;
using System.IO;



#if UNITY_EDITOR
using UnityEditor;
#endif
public static class EditorTools
{
#if UNITY_EDITOR
    [MenuItem("Tools/Quick Screenshot")]
    private static void QuickScreenshot()
    {
        ScreenCapture.CaptureScreenshot(Application.dataPath + "/Screenshot.png");
        AssetDatabase.Refresh();
    }

    private const string PRODUCT_NAME = "RC25_SPRONKY_BEAT";
    private const string CORRECT_PASSWORD = "123456";


    [MenuItem("Build/Build And Run Android with Password")]
    public static void BuildAndRunAndroidWithPassword()
    {
        PlayerSettings.Android.keystorePass = CORRECT_PASSWORD;
        PlayerSettings.Android.keyaliasPass = CORRECT_PASSWORD;

        string path = $"Builds/{PRODUCT_NAME}.apk";
        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = GetEnabledScenes(),
            locationPathName = path,
            target = BuildTarget.Android,
            options = BuildOptions.AutoRunPlayer
        };

        BuildPipeline.BuildPlayer(buildOptions);
        OpenFolder();
    }

    [MenuItem("Build/Build Android with Password")]
    public static void BuildAndroidWithPassword()
    {
        PlayerSettings.Android.keystorePass = CORRECT_PASSWORD;
        PlayerSettings.Android.keyaliasPass = CORRECT_PASSWORD;

        string path = $"Builds/{PRODUCT_NAME}.apk";
        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = GetEnabledScenes(),
            locationPathName = path,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        BuildPipeline.BuildPlayer(buildOptions);
        OpenFolder();
    }

    static string[] GetEnabledScenes()
    {
        return EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();
    }

    [MenuItem("Build/Open Specific Folder in Assets")]
    public static void OpenFolder()
    {
        string folderPath = $"Builds/";
        string fullPath = Path.GetFullPath(folderPath);
        if (Directory.Exists(fullPath))
        {
            EditorUtility.RevealInFinder(fullPath); // Mở trong Explorer/Finder
        }
    }
#endif
}