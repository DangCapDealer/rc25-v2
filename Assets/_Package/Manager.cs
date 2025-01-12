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
    public int NumberOfReplay = 0;
    public int NumberOfOpen = 0;
    public int NumberOfDead = 0;


    public string Mode = "Hard";

    [Header("Loading")]
    public bool IsAds = false;
    public bool IsFirebase = false;
    public bool IsIngame = false;
    public bool IsLoading = false;
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

        if(RuntimeStorageData.Player.LastDayLogin != DateTime.Now.Day)
        {
            RuntimeStorageData.Player.LastDayLogin = DateTime.Now.Day;
            RuntimeStorageData.Player.NumberOfDay += 1;
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
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


    public string GameId;
    public void ShowLoading(string GameId)
    {
        this.GameId = GameId;
        IsLoading = true;
        loadingCanvas.Show();
        CoroutineUtils.PlayCoroutineOneSecond(() => LoadScene(Scene.MiniGame));
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
