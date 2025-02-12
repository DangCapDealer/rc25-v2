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

        yield return new WaitUntil(() => RuntimeStorageData.IsReady);
        yield return new WaitUntil(() => FirebaseManager.Instance.IsInitialized);
        yield return WaitForSecondCache.WAIT_TIME_ZERO_POINT_ONE;
        _nativeBanner.SetActive(RuntimeStorageData.Player.IsLoadAds);
        if (RuntimeStorageData.Player.IsLoadAds == true)
        {
            _nativeBanner.SetActive(Manager.Instance.IsNativeBanner);
        }

        AutoNoAd();
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

    public void AutoNoAd()
    {
        if (RuntimeStorageData.Player.IsProductId(InappController.Instance.GetProductIdByIndex(0)) &&
            RuntimeStorageData.Player.IsProductId(InappController.Instance.GetProductIdByIndex(1)))
        {

        }
        else
        {
            ShowNoAd();
        }
    }    

    public void ShowNoAd()
    {
        int numerOfRandom = 0;
        string randomproductID = "";
        do
        {
            int randomNum = Random.Range(0, 2);
            randomproductID = InappController.Instance.GetProductIdByIndex(randomNum);
            if (numerOfRandom > 200)
                break;
            numerOfRandom += 1;
        }
        while (RuntimeStorageData.Player.Packages.Contains(randomproductID));
        Debug.Log(randomproductID);
        var numberOfProduct = InappController.Instance.GetProductIndexById(randomproductID);
        switch(numberOfProduct)
        {
            case 0:
                _popupUICanvas.ShowPopup(Popup.NoAdsEvent);
                break;
            case 1:
                _popupUICanvas.ShowPopup(Popup.NoAdsVipMember);
                break;
        }
    }

    private void OnEnable()
    {
        GameEvent.OnIAPurchase += OnIAPurchase; 
    }

    private void OnDisable()
    {
        GameEvent.OnIAPurchase -= OnIAPurchase;
    }

    private void OnIAPurchase(string productId)
    {
        _nativeBanner.SetActive(RuntimeStorageData.Player.IsLoadAds);
    }
}
