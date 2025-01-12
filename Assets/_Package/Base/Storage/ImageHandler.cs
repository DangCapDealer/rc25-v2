using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Networking;

public class ImageHandler : MonoSingleton<ImageHandler>
{
    public void downloadImage(string url, string fileName)
    {
        StartCoroutine(GetTextureRequest(url, fileName));
    }

    IEnumerator GetTextureRequest(string url, string fileName)
    {
        using (var www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                if (www.isDone)
                {
                    var texture = DownloadHandlerTexture.GetContent(www);
                    byte[] itemBGBytes = texture.EncodeToPNG();
                    File.WriteAllBytes(Application.dataPath + $"/{fileName}", itemBGBytes);
#if UNITY_EDITOR
                    AssetDatabase.Refresh();
#endif
                }
            }
        }
    }
}
