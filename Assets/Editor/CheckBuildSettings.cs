using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

class CheckBuildSettings : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("Development Build is currently set to: " + EditorUserBuildSettings.development);
        if (EditorUserBuildSettings.development)
        {
            Debug.LogError("CẢNH BÁO: Development Build đang được bật! Có thể do một plugin hoặc script khác đã thay đổi giá trị này.");
        }
    }
}