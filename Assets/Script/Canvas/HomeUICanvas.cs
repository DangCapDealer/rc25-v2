using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeUICanvas : MonoBehaviour
{
    private void OnEnable()
    {
        if (Manager.Instance != null)
            Manager.Instance.IngameScreenID = "HomeUICanvas";
    }


    public void BtnSingle()
    {
        if(AdManager.Instance == null)
        {
            GameManager.Instance.Style = GameManager.GameStyle.Normal;
            GameManager.Instance.NumberOfCharacter = 7;
            SoundSpawn.Instance.CreateSound();
            SoundSpawn.Instance.Reload();
            CanvasSystem.Instance.ChooseScreen("GameUICanvas");
            CanvasSystem.Instance._gameUICanvas.CreateGame();
            AdManager.Instance.HideBannerAd();
            AdManager.Instance.ShowBannerMERCAd();
        }   
        else
        {
            AdManager.Instance.ShowInterstitialHomeAd(() =>
            {
                GameManager.Instance.Style = GameManager.GameStyle.Normal;
                GameManager.Instance.NumberOfCharacter = 7;
                SoundSpawn.Instance.CreateSound();
                SoundSpawn.Instance.Reload();
                CanvasSystem.Instance.ChooseScreen("GameUICanvas");
                CanvasSystem.Instance._gameUICanvas.CreateGame();
                AdManager.Instance.HideBannerAd();
                AdManager.Instance.ShowBannerMERCAd();
            });
        }       
    }   

    public void BtnSingleHorror()
    {
        if (AdManager.Instance == null)
        {
            GameManager.Instance.Style = GameManager.GameStyle.Horror;
            GameManager.Instance.NumberOfCharacter = 7;
            SoundSpawn.Instance.CreateSound();
            SoundSpawn.Instance.Reload();
            CanvasSystem.Instance.ChooseScreen("GameUICanvas");
            CanvasSystem.Instance._gameUICanvas.CreateGame();
            AdManager.Instance.HideBannerAd();
            AdManager.Instance.ShowBannerMERCAd();
        }
        else
        {
            AdManager.Instance.ShowInterstitialHomeAd(() =>
            {
                GameManager.Instance.Style = GameManager.GameStyle.Horror;
                GameManager.Instance.NumberOfCharacter = 7;
                SoundSpawn.Instance.CreateSound();
                SoundSpawn.Instance.Reload();
                CanvasSystem.Instance.ChooseScreen("GameUICanvas");
                CanvasSystem.Instance._gameUICanvas.CreateGame();
                AdManager.Instance.HideBannerAd();
                AdManager.Instance.ShowBannerMERCAd();
            });
        }
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
