using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSystem : MonoSingleton<CanvasSystem>
{
    public HomeUICanvas _homeUICanvas;
    public GameUICanvas _gameUICanvas;
    public PopupUICanvas _popupUICanvas;

    public GameObject[] _screenUICanvas;

    public Transform _bannerCollapse;
    public void ShowNativeCollapse()
    {
        if (RuntimeStorageData.Player.IsLoadAds == false)
            return;
        if (Manager.Instance.IsNativeMREC == false)
            return;
        _bannerCollapse.SetActive(true);
    }

    public Transform _nativeInter;
    public void ShowNativeIntertitial()
    {
        if (RuntimeStorageData.Player.IsLoadAds == false)
            return;
        if (Manager.Instance.IsNativeInter == false)
            return;
        _nativeInter.SetActive(true);
    }

    public Transform _nativeBanner;
    

    private IEnumerator Start()
    {
        ChooseScreen("HomeUICanvas");

        yield return new WaitUntil(() => Manager.IsReady);
        yield return new WaitUntil(() => RuntimeStorageData.IsReady);
        yield return new WaitUntil(() => FirebaseManager.Instance.IsInitialized);
        yield return WaitForSecondCache.WAIT_TIME_ONE;
        _nativeBanner.SetActive(RuntimeStorageData.Player.IsLoadAds);
        if (RuntimeStorageData.Player.IsLoadAds == true)
        {
            _nativeBanner.SetActive(Manager.Instance.IsNativeBanner);
        }
    }

    public void ChooseScreen(string name)
    {
        _screenUICanvas.SimpleForEach(x =>
        {
            if(x.name == name)
            {
                x.SetActive(true);
            }
            else
            {
                x.SetActive(false);
            } 
               
        });
    }      
}
