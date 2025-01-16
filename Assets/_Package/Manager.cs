using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum Scene
{
    Loading,
    Ingame,
    MiniGame
}

public class Manager : MonoSingletonGlobal<Manager>
{
    public Scene Scene = Scene.Loading;

    [Header("Loading")]
    public bool IsAds = false;
    public bool IsFirebase = false;
    public bool IsLoading = false;
    public bool IsIngame = true;
    public string IngameScreenID = "";
    [SerializeField] private LoadingCanvas loadingCanvas;

    protected override void Awake()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;

        base.Awake();

        var IsRuntimeData = PlayerPrefsOverride.GetBool("Data", false);
        if (IsRuntimeData == false) RuntimeStorageData.CreateData();
        else RuntimeStorageData.ReadData();
        PlayerPrefsOverride.SetBool("Data", true);
    }

    public void Start()
    {
        IsLoading = true;
        loadingCanvas.Show(null, 0.0f, 0.6f);
    }

    public void CompleteOpenAd()
    {
        loadingCanvas.Show(() => 
        {
            LoadScene(Scene.Ingame);
            IsLoading = false; 
        }, 0.6f, 1.0f);
    }    


    public void LoadScene(Scene name, LoadSceneMode mode = LoadSceneMode.Single)
    {
        Scene = name;
        SceneManager.LoadScene((int)name, mode);
    }    

    public void ShowLoading()
    {
        IsLoading = true;
        loadingCanvas.Show();
        CoroutineUtils.PlayCoroutineOneSecond(() => LoadScene(Scene.Ingame));
    }  
    
    public void HideLoading()
    {
        CoroutineUtils.PlayCoroutineOneSecond(() =>
        {
            loadingCanvas.Hide();
        });
    }

    public void CorotineLoading(UnityAction CALLBACK)
    {
        loadingCanvas.Show();
        CoroutineUtils.PlayCoroutineOneSecond(() =>
        {
            CALLBACK?.Invoke();
            CoroutineUtils.PlayCoroutineHaftSecond(() => loadingCanvas.Hide());
        });
    }

    public void CaculateRCoin(int value, UnityAction<int> confirm, UnityAction reject)
    {
        var RCoin = RuntimeStorageData.Player.Gold;
        RCoin += value;
        if (RCoin >= 0)
        {
            RuntimeStorageData.Player.Gold = RCoin;
            confirm?.Invoke(RCoin);
        }    
        else
            reject?.Invoke();
    }

    public bool IsEnoughRCoin(int value)
    {
        if(RuntimeStorageData.Player.Gold >= value)
            return true;
        return false;
    }

    public void OnApplicationPause(bool pause)
    {
        Debug.Log("Application Pause");
        RuntimeStorageData.SaveAllData();
        if (pause == true)
            AdManager.Instance.CheckingOpenAd();
    }

    public void OnApplicationQuit()
    {
        RuntimeStorageData.SaveAllData();
    }
}
