using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSystem : MonoSingleton<CanvasSystem>
{
    public HomeUICanvas _homeUICanvas;
    public GameUICanvas _gameUICanvas;
    public PopupUICanvas _popupUICanvas;
    public GameLoadingUICanvas _loadingUICanvas;

    public GameObject[] _screenUICanvas;

    //public Transform _bannerCollapse;
    public void ShowNativeCollapse()
    {
        //if (RuntimeStorageData.Player.IsLoadAds == false) return;
        //if (Manager.Instance.IsNativeMREC == false) return;
        //_bannerCollapse.SetActive(true);

        //_nativeCollapseAutomation = 0;
    }

    //public Transform _nativeInter;
    //public void ShowNativeIntertitial()
    //{
    //    if (RuntimeStorageData.Player.IsLoadAds == false) return;
    //    if (Manager.Instance.IsNativeInter == false) return;
    //    _nativeInter.SetActive(true);
    //}

    //public Transform _nativeBanner;

    [Header("Native Collapse")]
    public float _nativeCollapseAutomationTime = 45.0f;
    public float _nativeCollapseAutomation = 0;
    

    private IEnumerator Start()
    {
        ChooseScreen("HomeUICanvas");

        yield return new WaitUntil(() => RuntimeStorageData.IsReady);
        yield return new WaitUntil(() => FirebaseManager.Instance.IsInitialized);
        yield return WaitForSecondCache.WAIT_TIME_ZERO_POINT_ONE;
        OnIAPurchase("", "");
        AutoNoAd();
    }

    //private void Update()
    //{
    //    if (RuntimeStorageData.Player.IsLoadAds == false) return;
    //    if (Manager.Instance.IsNativeMREC == false) return;

    //    if (_bannerCollapse.IsActive() == true) return;

    //    _nativeCollapseAutomation += Time.deltaTime;
    //    if(_nativeCollapseAutomation > _nativeCollapseAutomationTime)
    //    {
    //        ShowNativeCollapse();
    //    }    
    //}


    [Header("SCREEN")]
    public string ScreenID = "";
    public void ChooseScreen(string name)
    {
        ScreenID = name;
        _screenUICanvas.SimpleForEach(x =>
        {
            if(x.name == name) x.SetActive(true);
            else x.SetActive(false);
        });
    }

    public void AutoNoAd()
    {
#if INAPP
        if (RuntimeStorageData.Player.IsProductId(InappController.Instance.GetProductIdByIndex(0)) &&
            RuntimeStorageData.Player.IsProductId(InappController.Instance.GetProductIdByIndex(1)))
        { }
        else ShowNoAd();
#endif
    }    

    public void ShowNoAd()
    {
        LogSystem.LogWarning("[CanvasSystem] Bật lại đoạn này khi có IAP");
        return;
        if(Manager.Instance.IsVIPMember == false)
        {
            var isProduct = RuntimeStorageData.Player.IsProductId(InappController.Instance.GetProductIdByIndex(1));
            if (isProduct == false)
            {
                _popupUICanvas.ShowPopup(Popup.NoAdsEvent);
            }
        }   
        else
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
            var numberOfProduct = InappController.Instance.GetProductIndexById(randomproductID);
            switch (numberOfProduct)
            {
                case 0:
                    _popupUICanvas.ShowPopup(Popup.NoAdsVipMember);
                    break;
                case 1:
                    _popupUICanvas.ShowPopup(Popup.NoAdsEvent);
                    break;
            }
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

    private void OnIAPurchase(string productId, string action)
    {
        if (RuntimeStorageData.Player.Packages.Contains(InappController.Instance.GetProductIdByIndex(0)) == false &&
            RuntimeStorageData.Player.Packages.Contains(InappController.Instance.GetProductIdByIndex(1)) == false)
        { RuntimeStorageData.Player.IsLoadAds = true; }
        else { RuntimeStorageData.Player.IsLoadAds = false; }
        //_nativeBanner.SetActive(RuntimeStorageData.Player.IsLoadAds);
        //if (_bannerCollapse.IsActive() == true) _bannerCollapse.SetActive(RuntimeStorageData.Player.IsLoadAds);
        //if (RuntimeStorageData.Player.IsLoadAds == true) { _nativeBanner.SetActive(Manager.Instance.IsNativeBanner); }
    }
}
