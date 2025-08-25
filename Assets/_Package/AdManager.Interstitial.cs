using UnityEngine;
using UnityEngine.Events;
#if ADMOB
using DG.Tweening;
using GoogleMobileAds.Api;
using PimDeWitte.UnityMainThreadDispatcher;
#endif

public partial class AdManager
{
#if ADMOB
    public UnityAction ActionOnAfterInterstitalAd;
    public UnityAction ActionOnAfterInterstitalHomeAd;
    public UnityAction ActionOnAfterInterstitalOpenAd;

    [Header("AD INTERSTITIAL")]
    public GameObject _loadingInterstitalAd;

    public bool IsPreloadInterstitial = true;
    public AdState InterAdState = AdState.NotAvailable;
    public AdShowState InterAdShowState = AdShowState.None;
    public int _interstitalReloadCount = 0;
    [Tooltip("Tự động show ad theo thời gian")]
    public float InterAdSpaceTimeAutoCounter = 0;
    public string _adUnitInterId = "ca-app-pub-5904408074441373/8836093904";

    private InterstitialAd _interstitialAd;
    private InterstitialAd _interstitialHomeAd;
    private InterstitialAd _interstitialOpenAd;

    private void LoadInterstitialAd()
    {
        if (RuntimeStorageData.Player.IsLoadAds == false) return;
        if (InterAdState == AdState.Loading) return;
        InterAdState = AdState.Loading;

        if (_interstitialAd != null) { _interstitialAd.Destroy(); _interstitialAd = null; }

        Debug.Log($"[{GetType()}] Loading the interstitial ad.");
        var adRequest = new AdRequest();
        InterstitialAd.Load(_adUnitInterId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                InterAdState = AdState.NotAvailable;
                _interstitalReloadCount += 1;
                return;
            }
            _interstitialAd = ad;
            _interstitialAd.OnAdPaid += (revenue) => { AppflyerEventSender.Instance.logAdRevenue(revenue); };
            _interstitialAd.OnAdFullScreenContentClosed += () => { ActionOnAfterInterstitalAd?.Invoke(); };
            _interstitialAd.OnAdFullScreenContentFailed += (AdError error2) => { ActionOnAfterInterstitalAd?.Invoke(); };
            InterAdState = AdState.Ready;
            _interstitalReloadCount = 0;
        });
    }

    public void ShowInterstitialAd(UnityAction Callback)
    {
        if (RuntimeStorageData.Player.IsLoadAds == false) { Callback?.Invoke(); return; }
        if (InterAdShowState == AdShowState.Pending) return;

        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            Debug.Log($"[{GetType()}] Showing interstitial ad.");
            InterAdShowState = AdShowState.Pending;
            _loadingInterstitalAd?.SetActive(true);
            ResetOpenAdSpaceTime();

            ActionOnAfterInterstitalAd = () =>
            {
                InterAdShowState = AdShowState.None;
                InterAdState = AdState.NotAvailable;
                Callback?.Invoke();
            };

            DOVirtual.DelayedCall(1.0f, () =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => _loadingInterstitalAd?.SetActive(false));
                _interstitialAd.Show();
            });
            ResetInterstitialAdCounter();
        }
        else Callback?.Invoke();
    }

    [Header("AD HOME INTERSTITIAL")]
    public bool IsPreloadinterstitialHome = true;
    public AdState InterHomeAdState = AdState.NotAvailable;
    public AdShowState InterHomeAdShowState = AdShowState.None;
    public int _interstitialHomeAdReloadCount = 0;
    public float InterHomeAdSpaceTimeAutoCounter = 0;
    public string _adUnitInterHomeId = "ca-app-pub-5904408074441373/2893502086";

    private void LoadInterstitialHomeAd()
    {
        if (RuntimeStorageData.Player.IsLoadAds == false) return;
        if (InterHomeAdState == AdState.Loading) return;
        InterHomeAdState = AdState.Loading;

        if (_interstitialHomeAd != null) { _interstitialHomeAd.Destroy(); _interstitialHomeAd = null; }

        var adRequest = new AdRequest();
        InterstitialAd.Load(_adUnitInterHomeId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                InterHomeAdState = AdState.NotAvailable;
                _interstitialHomeAdReloadCount += 1;
                return;
            }

            _interstitialHomeAd = ad;
            _interstitialHomeAd.OnAdPaid += (revenue) => { AppflyerEventSender.Instance.logAdRevenue(revenue); };
            InterHomeAdState = AdState.Ready;
            _interstitialHomeAd.OnAdFullScreenContentClosed += () => { ActionOnAfterInterstitalHomeAd?.Invoke(); };
            _interstitialHomeAd.OnAdFullScreenContentFailed += (AdError error2) => { ActionOnAfterInterstitalHomeAd?.Invoke(); };
            _interstitialHomeAdReloadCount = 0;
        });
    }

    public void ShowInterstitialHomeAd(UnityAction Callback, UnityAction CallbackAfterInter)
    {
        if (RuntimeStorageData.Player.IsLoadAds == false) { Callback?.Invoke(); CallbackAfterInter?.Invoke(); return; }
        if (InterHomeAdShowState == AdShowState.Pending) return;
        if (InterHomeAdSpaceTimeAutoCounter < Manager.Instance.InterHomeReloadTimer) { Callback?.Invoke(); return; }

        if (_interstitialHomeAd != null && _interstitialHomeAd.CanShowAd())
        {
            Debug.Log($"[{GetType()}] Showing interstitial home ad.");
            InterHomeAdShowState = AdShowState.Pending;
            ResetOpenAdSpaceTime();

            ActionOnAfterInterstitalHomeAd = () =>
            {
                InterHomeAdShowState = AdShowState.None;
                InterHomeAdState = AdState.NotAvailable;
                Callback?.Invoke();
                CallbackAfterInter?.Invoke();
            };
            _interstitialHomeAd.Show();
            ResetInterstitialAdCounter();
        }
        else { Callback?.Invoke(); CallbackAfterInter?.Invoke(); }
    }

    [Header("AD OPEN INTERSTITIAL")]
    public string _adUnitInterOpenId = "ca-app-pub-5904408074441373/3310341039";
    public float InterOpenAdMaximusTimeCounter = 0.0f;
    public float InterOpenAdMaximusTime = 15.0f;

    private void PromiseShowInterstitialAd(UnityAction afterAd)
    {
        if (RuntimeStorageData.Player.IsLoadAds == false) { afterAd?.Invoke(); return; }

        Debug.Log($"[{GetType()}] Promise show interstitial open Ad");
        var adRequest = new AdRequest();
        InterstitialAd.Load(_adUnitInterOpenId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null) return;

            _interstitialOpenAd = ad;
            _interstitialOpenAd.OnAdPaid += (revenue) => { AppflyerEventSender.Instance.logAdRevenue(revenue); };
            _interstitialOpenAd.OnAdFullScreenContentClosed += () => { ActionOnAfterInterstitalOpenAd?.Invoke(); };
            _interstitialOpenAd.OnAdFullScreenContentFailed += (AdError error2) => { ActionOnAfterInterstitalOpenAd?.Invoke(); };

            ActionOnAfterInterstitalOpenAd = () => { afterAd?.Invoke(); };
            if (InterOpenAdMaximusTimeCounter <= InterOpenAdMaximusTime)
            {
                InterOpenAdMaximusTimeCounter = 999;
                ResetOpenAdSpaceTime();
                _interstitialOpenAd.Show();
            }
        });
    }

    private void CaculaterCounterInterOpenAd()
    {
        if (InterOpenAdMaximusTimeCounter <= InterOpenAdMaximusTime)
        {
            InterOpenAdMaximusTimeCounter += Time.deltaTime;
            if (InterOpenAdMaximusTimeCounter > InterOpenAdMaximusTime)
                Manager.Instance.CompleteOpenAd();
        }
    }

    private void CaculaterCounterInterAd()
    {
        if (IsBlockedAutoIntertitialAd) return;

        if (Manager.Instance.IsIngame == true && Manager.Instance.IngameScreenID == "GameUICanvas")
        {
            InterAdSpaceTimeAutoCounter += Time.deltaTime;
            if (InterAdSpaceTimeAutoCounter > Manager.Instance.InterAutoReloadTimer)
            {
                InterAdSpaceTimeAutoCounter = 0;
                ShowInterstitialAd(() => UnityMainThreadDispatcher.Instance().Enqueue(AdManager.Instance.ShowNativeOverlayAd));
            }
        }

        if (Manager.Instance.IsIngame == true)
            InterHomeAdSpaceTimeAutoCounter += Time.deltaTime;
    }
#endif
}
