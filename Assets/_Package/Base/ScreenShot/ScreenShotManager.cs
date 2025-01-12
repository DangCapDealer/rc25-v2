using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScreenShotManager : MonoSingleton<ScreenShotManager>
{
#if UNITY_EDITOR
    void Update ()
	{
	    if (Input.GetKeyDown(KeyCode.C))
	    {
	        StartCoroutine(TakeScreenShotCo());
	    }
	}
#endif

    private IEnumerator TakeScreenShotCo()
    {
        yield return new WaitForEndOfFrame();

        var directory = new DirectoryInfo(Application.persistentDataPath);
        var path = Path.Combine(directory.Parent.FullName, $"Screenshot_{DateTime.Now:yyyyMMdd_Hmmss}.png");
     
        ScreenCapture.CaptureScreenshot(path,1);
        Debug.Log($"Screen Shot Captured");
        Debug.Log(Application.persistentDataPath);
    }
}
