using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeUICanvas : MonoBehaviour
{
    //public Transform _banner;

    private void Start()
    {
        //_banner.SetActive(RuntimeStorageData.Player.IsLoadAds);
    }

    private void OnEnable()
    {
        if (Manager.Instance != null)
            Manager.Instance.IngameScreenID = "HomeUICanvas";
    }


    public void BtnSingle()
    {
        MusicManager.Instance.PauseSound();
        GameManager.Instance.Style = GameManager.GameStyle.Normal;
        BackgroundDetection.Instance.SettingBackground();
        AdManager.Instance.ShowInterstitialHomeAd(() =>
        {
            //GameManager.Instance.NumberOfCharacter = 7;
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                GameManager.Instance.NumberOfCharacter = 7;
                SoundSpawn.Instance.CreateSound();
                SoundSpawn.Instance.Reload();
                CanvasSystem.Instance.ChooseScreen("GameUICanvas");
                CanvasSystem.Instance._gameUICanvas.CreateGame();
                CanvasSystem.Instance.ShowNativeCollapse();
            });
            //IsGotoGameAfterIntertitialAd = true;
        }, () =>
        {
            //IsShowNativeIntertitialAd = true; 
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
                //IsGotoGameAfterIntertitialAd = true;
                SoundSpawn.Instance.CreateSound();
                SoundSpawn.Instance.Reload();
                CanvasSystem.Instance.ChooseScreen("GameUICanvas");
                CanvasSystem.Instance._gameUICanvas.CreateGame();
                CanvasSystem.Instance.ShowNativeCollapse();
            });
            //GameManager.Instance.NumberOfCharacter = 7;
            ////IsGotoGameAfterIntertitialAd = true;
            //SoundSpawn.Instance.CreateSound();
            //SoundSpawn.Instance.Reload();
            //CanvasSystem.Instance.ChooseScreen("GameUICanvas");
            //CanvasSystem.Instance._gameUICanvas.CreateGame();
            //CanvasSystem.Instance.ShowNativeCollapse();
        }, () =>
        {
            //IsShowNativeIntertitialAd = true;
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                CanvasSystem.Instance.ShowNativeIntertitial();
            });
        });
    }

    //private bool IsGotoGameAfterIntertitialAd = false;
    //private bool IsShowNativeIntertitialAd = false;

    //private void AsyncIntertitialEvent()
    //{
    //    //if(IsGotoGameAfterIntertitialAd == true)
    //    //{
    //    //    IsGotoGameAfterIntertitialAd = false;
    //    //    SoundSpawn.Instance.CreateSound();
    //    //    SoundSpawn.Instance.Reload();
    //    //    CanvasSystem.Instance.ChooseScreen("GameUICanvas");
    //    //    CanvasSystem.Instance._gameUICanvas.CreateGame();
    //    //    CanvasSystem.Instance.ShowNativeCollapse();
    //    //}    

    //    //if(IsShowNativeIntertitialAd == true)
    //    //{
    //    //    IsShowNativeIntertitialAd = false;
    //    //    CanvasSystem.Instance.ShowNativeIntertitial();
    //    //}    
    //}    

    //private void Update()
    //{
    //    AsyncIntertitialEvent();
    //}

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
        CanvasSystem.Instance._popupUICanvas.ShowPopup(Popup.NoAds);
    }
    
    public void BtnRate()
    {
        // Thay bằng đường dẫn ứng dụng của bạn trên App Store hoặc Google Play
#if UNITY_ANDROID
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.sprank.horror.beats.studio.battle");
#elif UNITY_IOS
            Application.OpenURL("itms-apps://itunes.apple.com/app/id1234567890");
#else
            Debug.Log("Rate Us is not supported on this platform.");
#endif
    }
}
