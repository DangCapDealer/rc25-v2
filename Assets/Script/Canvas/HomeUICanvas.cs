using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeUICanvas : MonoBehaviour
{
    public Transform _btnProductRemoveAd;

    private void Start()
    {
        OnIAPurechase("", "");
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
        Debug.Log($"Purchse complete ID: {productID}");

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
        MusicManager.Instance.PauseSound();
        GameManager.Instance.Style = GameManager.GameStyle.Normal;
        BackgroundDetection.Instance.SettingBackground();
        AdManager.Instance.ShowInterstitialHomeAd(() =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                GameManager.Instance.NumberOfCharacter = 7;
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
    }   

    public void BtnSingleHorror()
    {
        MusicManager.Instance.PauseSound();
        GameManager.Instance.Style = GameManager.GameStyle.Horror;
        BackgroundDetection.Instance.SettingBackground();
        AdManager.Instance.ShowInterstitialHomeAd(() =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                GameManager.Instance.NumberOfCharacter = 7;
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
    }

    public void BtnBatteBeat()
    {
        MusicManager.Instance.PauseSound();
        GameManager.Instance.Style = GameManager.GameStyle.Battle;
        BackgroundDetection.Instance.SettingBackground();
        AdManager.Instance.ShowInterstitialHomeAd(() =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                GameManager.Instance.NumberOfCharacter = 7;
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
    }    

    public void BtnSetting()
    {
        CanvasSystem.Instance._popupUICanvas.ShowPopup(Popup.Setting);
    }    

    public void BtnCheckin()
    {
        CanvasSystem.Instance._popupUICanvas.ShowPopup(Popup.Checkin);
    }    

    public void BtnNoAds()
    {
        CanvasSystem.Instance.ShowNoAd();
    }
    
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
