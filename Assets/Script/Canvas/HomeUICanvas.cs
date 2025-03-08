using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeUICanvas : MonoBehaviour
{
    public Transform _btnProductRemoveAd;
    public Transform[] _btnModes;

    private void Start()
    {
#if INAPP
        OnIAPurechase("", "");
#endif
    }

    private void OnEnable()
    {
        if (Manager.Instance != null)
            Manager.Instance.IngameScreenID = "HomeUICanvas";

        GameEvent.OnIAPurchase += OnIAPurechase;
    }

    private void OnDisable()
    {
        GameEvent.OnIAPurchase -= OnIAPurechase;
    }

    private void OnIAPurechase(string productID, string action)
    {
        if (RuntimeStorageData.Player.IsProductId(InappController.Instance.GetProductIdByIndex(0)) &&
            RuntimeStorageData.Player.IsProductId(InappController.Instance.GetProductIdByIndex(1)))
        {
            _btnProductRemoveAd.SetActive(false);
        }
        else
        {
            _btnProductRemoveAd.SetActive(true);
        }
    }


    public void BtnSingle()
    {
        StaticVariable.ClearLog();
        MusicManager.Instance.PauseSound();
        GameManager.Instance.Style = GameManager.GameStyle.Normal;
        BackgroundDetection.Instance.SettingBackground();
        AdManager.Instance.ShowInterstitialHomeAd(() =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                GameManager.Instance.NumberOfCharacter = 8;
                GameManager.Instance.GameCreate();
                SoundSpawn.Instance.CreateSound();
                SoundSpawn.Instance.Reload();
                CanvasSystem.Instance.ChooseScreen("GameUICanvas");
                CanvasSystem.Instance._gameUICanvas.CreateGame();
                CanvasSystem.Instance.ShowNativeCollapse();
            });
        }, () =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                CanvasSystem.Instance.ShowNativeIntertitial();
            });
        });

        FirebaseManager.Instance.LogEvent($"Mode_Beat_1");
        RuntimeStorageData.Player.Modes.Add("SingleNormalBeat - 1");
    }   

    public void BtnSingleHorror()
    {
        StaticVariable.ClearLog();
        MusicManager.Instance.PauseSound();
        GameManager.Instance.Style = GameManager.GameStyle.Horror;
        BackgroundDetection.Instance.SettingBackground();
        AdManager.Instance.ShowInterstitialHomeAd(() =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                GameManager.Instance.NumberOfCharacter = 8;
                GameManager.Instance.GameCreate();
                SoundSpawn.Instance.CreateSound();
                SoundSpawn.Instance.Reload();
                CanvasSystem.Instance.ChooseScreen("GameUICanvas");
                CanvasSystem.Instance._gameUICanvas.CreateGame();
                CanvasSystem.Instance.ShowNativeCollapse();
            });
        }, () =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                CanvasSystem.Instance.ShowNativeIntertitial();
            });
        });

        FirebaseManager.Instance.LogEvent($"Mode_Beat_2");
        RuntimeStorageData.Player.Modes.Add("SingleHorrorBeat - 2");
    }

    public void BtnSingleHuman()
    {
        StaticVariable.ClearLog();
        MusicManager.Instance.PauseSound();
        GameManager.Instance.Style = GameManager.GameStyle.Battle_Single;
        BackgroundDetection.Instance.SettingBackground();
        AdManager.Instance.ShowInterstitialHomeAd(() =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                GameManager.Instance.NumberOfCharacter = 8;
                GameManager.Instance.GameCreate();
                SoundSpawn.Instance.CreateSound();
                SoundSpawn.Instance.Reload();
                CanvasSystem.Instance.ChooseScreen("GameUICanvas");
                CanvasSystem.Instance._gameUICanvas.CreateGame();
                CanvasSystem.Instance.ShowNativeCollapse();
            });
        }, () =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                CanvasSystem.Instance.ShowNativeIntertitial();
            });
        });

        FirebaseManager.Instance.LogEvent($"Mode_Beat_3");
        RuntimeStorageData.Player.Modes.Add("SingleHumanBeat - 3");
    }

    public void BtnBatteBeat()
    {
        StaticVariable.ClearLog();
        MusicManager.Instance.PauseSound();
        GameManager.Instance.Style = GameManager.GameStyle.Battle;
        BackgroundDetection.Instance.SettingBackground();
        AdManager.Instance.ShowInterstitialHomeAd(() =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                GameManager.Instance.NumberOfCharacter = 12;
                GameManager.Instance.GameCreate();
                SoundSpawn.Instance.CreateSound();
                SoundSpawn.Instance.Reload();
                CanvasSystem.Instance.ChooseScreen("GameUICanvas");
                CanvasSystem.Instance._gameUICanvas.CreateGame();
                CanvasSystem.Instance.ShowNativeCollapse();
            });
        }, () =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                CanvasSystem.Instance.ShowNativeIntertitial();
            });
        });

        FirebaseManager.Instance.LogEvent($"Mode_Battle");
        RuntimeStorageData.Player.Modes.Add("BattleBeat_Solo");
    }    

    public void BtnSingleMonster()
    {
        StaticVariable.ClearLog();
        MusicManager.Instance.PauseSound();
        GameManager.Instance.Style = GameManager.GameStyle.Monster;
        BackgroundDetection.Instance.SettingBackground();
        AdManager.Instance.ShowInterstitialHomeAd(() =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                GameManager.Instance.NumberOfCharacter = 8;
                GameManager.Instance.GameCreate();
                SoundSpawn.Instance.CreateSound();
                SoundSpawn.Instance.Reload();
                CanvasSystem.Instance.ChooseScreen("GameUICanvas");
                CanvasSystem.Instance._gameUICanvas.CreateGame();
                CanvasSystem.Instance.ShowNativeCollapse();
            });
        }, () =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                CanvasSystem.Instance.ShowNativeIntertitial();
            });
        });

        FirebaseManager.Instance.LogEvent($"Mode_Beat_4");
        RuntimeStorageData.Player.Modes.Add("SingleMonsterBeat - 4");
    }

    public void BtnSetting() { CanvasSystem.Instance._popupUICanvas.ShowPopup(Popup.Setting); }    
    public void BtnCheckin() { CanvasSystem.Instance._popupUICanvas.ShowPopup(Popup.Checkin); }    
    public void BtnNoAds() { CanvasSystem.Instance.ShowNoAd(); }

    public void BtnRate()
    {
#if UNITY_ANDROID
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.sprank.horror.beats.studio.battle");
#elif UNITY_IOS
            Application.OpenURL("itms-apps://itunes.apple.com/app/id1234567890");
#else
            Debug.Log("Rate Us is not supported on this platform.");
#endif
    }
}
