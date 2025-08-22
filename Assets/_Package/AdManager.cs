using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using EditorCools;
using DG.Tweening;
using PimDeWitte.UnityMainThreadDispatcher;

#if ADMOB
using GoogleMobileAds.Api;
#endif

[DefaultExecutionOrder(-7)]
public partial class AdManager : MonoSingletonGlobal<AdManager>
{
#if ADMOB
    public enum AdState { Loading, Ready, NotAvailable }
    public enum AdShowState { None, Pending }
    public enum AdBannerSize { Banner, FullWidth }

    public bool IsInitalized = false;
    public bool IsBannerShow = false;
    public bool IsBannerMREC = false;

    private bool IsCanUpdate = false;
    public bool IsBlockedAutoIntertitialAd = false;

    private void Start() => StartCoroutine(Bacon.UMP.Instance.DOGatherConsent(LoadAds()));

    private IEnumerator LoadAds()
    {
        IsCanUpdate = false;
        yield return new WaitUntil(() => Bacon.UMP.Instance.IsUMPReady);
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.SetiOSAppPauseOnBackground(true);
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Admob Initialized");
            Manager.Instance.IsAds = true;
            IsInitalized = true;
            PromiseShowInterstitialAd(() => {
                UnityMainThreadDispatcher.Instance().Enqueue(() => Manager.Instance.CompleteOpenAd());
            });
        });
        yield return new WaitUntil(() => IsInitalized);
        if (RuntimeStorageData.Player.IsLoadAds == true) LoadAppOpenAd();
#if !UNITY_EDITOR
        yield return WaitForSecondCache.WAIT_TIME_FIVE;
#endif
        LoadRewardedAd();
        //LoadRewardedSecondAd();
        LoadRewardedThridAd();
        if (RuntimeStorageData.Player.IsLoadAds == true)
        {
            LoadNativeOverlayBannerAd();
            LoadNativeOverlayAd();
            LoadInterstitialAd();
            LoadInterstitialHomeAd();
        }

        IsCanUpdate = true;
    }


    private float timerReload = 10.0f;

    private void Update()
    {
        if (IsInitalized == false) return;
        if (IsCanUpdate == false) return;
        if (IsPreloadReward && RewardAdState == AdState.NotAvailable) LoadRewardedAd();
        //if (IsPreloadRewardSecond) if (RewardSecondAdState == AdState.NotAvailable) LoadRewardedSecondAd();
        if (IsPreloadRewardThrid && RewardThridAdState == AdState.NotAvailable) LoadRewardedThridAd();

        CaculaterCounterInterAd();
        CaculaterCounterInterOpenAd();
        CaculaterCounterOpenAd();

        CaculaterCounterCollapseBannerAd();

        if (RuntimeStorageData.Player.IsLoadAds == false) return;
        timerReload += Time.deltaTime;
        if (timerReload < 5.0f) return;

        if (IsPreloadinterstitialHome) if (InterHomeAdState == AdState.NotAvailable) LoadInterstitialHomeAd();
        if (IsPreloadInterstitial) if (InterAdState == AdState.NotAvailable) LoadInterstitialAd();
        if (IsPreloadOpen) if (OpenAdState == AdState.NotAvailable) LoadAppOpenAd();
        if (IsPreloadNativeOverlayAd) if (NativeOverlayAdState == AdState.NotAvailable) LoadNativeOverlayAd();
        if (IsPreloadNativeOverlayBannerAd) if (NativeOverlayBannerAdState == AdState.NotAvailable) LoadNativeOverlayBannerAd();

        timerReload = 0;
    }

    public UnityAction ActionOnAfterInterstitalAd;
    public UnityAction ActionOnAfterInterstitalHomeAd;
    public UnityAction ActionOnAfterInterstitalOpenAd;
    [Header("AD INTERSTITIAL")]
    public GameObject _loadingInterstitalAd;

    private void ResetInterstitialAdCounter()
    {
        InterAdSpaceTimeAutoCounter = 0;
        InterHomeAdSpaceTimeAutoCounter = 0;
    }    


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
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log($"[{this.GetType().ToString()}] Loading the interstitial ad.");
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
            _interstitialAd.OnAdFullScreenContentFailed += (AdError error) => { ActionOnAfterInterstitalAd?.Invoke(); };
            InterAdState = AdState.Ready;
            _interstitalReloadCount = 0;
        });
    }

    public void ShowInterstitialAd(UnityAction Callback)
    {
        if (RuntimeStorageData.Player.IsLoadAds == false)
        {
            if (Callback != null) Callback?.Invoke();
            return;
        }
        if (InterAdShowState == AdShowState.Pending) return;
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            Debug.Log($"[{this.GetType().ToString()}] Showing interstitial ad.");
            InterAdShowState = AdShowState.Pending;
            _loadingInterstitalAd?.SetActive(true);
            OpenAdSpaceTimeCounter = 0;

            ActionOnAfterInterstitalAd = () =>
            {
                InterAdShowState = AdShowState.None;
                InterAdState = AdState.NotAvailable;
                if (Callback != null) Callback?.Invoke();
            };

            DOVirtual.DelayedCall(1.0f, () =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() => _loadingInterstitalAd?.SetActive(false));
                _interstitialAd.Show();
            });
            ResetInterstitialAdCounter();
        }
        else if (Callback != null) Callback?.Invoke();
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

        if (_interstitialHomeAd != null)
        {
            _interstitialHomeAd.Destroy();
            _interstitialHomeAd = null;
        }

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
            _interstitialHomeAd.OnAdFullScreenContentFailed += (AdError error) => { ActionOnAfterInterstitalHomeAd?.Invoke(); };
            _interstitialHomeAdReloadCount = 0;
        });
    }

    public void ShowInterstitialHomeAd(UnityAction Callback, UnityAction CallbackAfterInter)
    {
        if (RuntimeStorageData.Player.IsLoadAds == false)
        {
            if (Callback != null) Callback?.Invoke();
            if (CallbackAfterInter != null) CallbackAfterInter?.Invoke();
            return;
        }    

        if (InterHomeAdShowState == AdShowState.Pending) return;
        if (InterHomeAdSpaceTimeAutoCounter < Manager.Instance.InterHomeReloadTimer)
        {
            if (Callback != null) Callback?.Invoke();
            return;
        }    

        if (_interstitialHomeAd != null && _interstitialHomeAd.CanShowAd())
        {
            Debug.Log($"[{this.GetType().ToString()}] Showing interstitial home ad.");
            InterHomeAdShowState = AdShowState.Pending;
            OpenAdSpaceTimeCounter = 0;

            ActionOnAfterInterstitalHomeAd = () =>
            {
                InterHomeAdShowState = AdShowState.None;
                InterHomeAdState = AdState.NotAvailable;
                if (Callback != null) Callback?.Invoke();
                if (CallbackAfterInter != null) CallbackAfterInter?.Invoke();
            };
            _interstitialHomeAd.Show();
            ResetInterstitialAdCounter();
        }
        else
        {
            if (Callback != null) Callback?.Invoke();
            if (CallbackAfterInter != null) CallbackAfterInter?.Invoke();
        }
    }

    [Header("AD OPEN INTERSTITIAL")]
    public string _adUnitInterOpenId = "ca-app-pub-5904408074441373/3310341039";
    public float InterOpenAdMaximusTimeCounter = 0.0f;
    public float InterOpenAdMaximusTime = 15.0f;

    private void PromiseShowInterstitialAd(UnityAction afterAd)
    {
        if (RuntimeStorageData.Player.IsLoadAds == false)
        {
            if (afterAd != null) afterAd?.Invoke();
            return;
        } 
        Debug.Log($"[{this.GetType().ToString()}] Promise show interstitial open Ad");
        var adRequest = new AdRequest();
        InterstitialAd.Load(_adUnitInterOpenId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null) return;
            _interstitialOpenAd = ad;
            _interstitialOpenAd.OnAdPaid += (revenue) => { AppflyerEventSender.Instance.logAdRevenue(revenue); };
            _interstitialOpenAd.OnAdFullScreenContentClosed += () => { ActionOnAfterInterstitalOpenAd?.Invoke(); };
            _interstitialOpenAd.OnAdFullScreenContentFailed += (AdError error) => { ActionOnAfterInterstitalOpenAd?.Invoke(); };

            ActionOnAfterInterstitalOpenAd = () => { afterAd?.Invoke(); };
            if (InterOpenAdMaximusTimeCounter <= InterOpenAdMaximusTime)
            {
                InterOpenAdMaximusTimeCounter = 999;
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
        if (IsBlockedAutoIntertitialAd == true) return;

        if (Manager.Instance.IsIngame == true && Manager.Instance.IngameScreenID == "GameUICanvas")
        {
            InterAdSpaceTimeAutoCounter += Time.deltaTime;
            if (InterAdSpaceTimeAutoCounter > Manager.Instance.InterAutoReloadTimer)
            {
                InterAdSpaceTimeAutoCounter = 0;
                ShowInterstitialAd(() => UnityMainThreadDispatcher.Instance().Enqueue(AdManager.Instance.ShowNativeOverlayAd));
            }
        }

        if (Manager.Instance.IsIngame == true) InterHomeAdSpaceTimeAutoCounter += Time.deltaTime;
    }



    [Header("AD REWARD")]
    public bool UseReward = true;
    public bool IsPreloadReward = true;
    public AdState RewardAdState = AdState.NotAvailable;
    public int _rewardReloadCount = 0;
    public string _adUnitRewardId = "ca-app-pub-5904408074441373/1580420414";

    private RewardedAd _rewardedAd;

    private void LoadRewardedAd()
    {
        if (RewardAdState == AdState.Loading) return;
        RewardAdState = AdState.Loading;

        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }
        var adRequest = new AdRequest();
        RewardedAd.Load(_adUnitRewardId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                RewardAdState = AdState.NotAvailable;
                _rewardReloadCount += 1;
                return;
            }

            _rewardedAd = ad;
            _rewardedAd.OnAdPaid += (revenue) => { AppflyerEventSender.Instance.logAdRevenue(revenue); };
            _rewardedAd.OnAdFullScreenContentClosed += () => { RewardAdState = AdState.NotAvailable; };
            _rewardedAd.OnAdFullScreenContentFailed += (AdError error) => { RewardAdState = AdState.NotAvailable; };
            RewardAdState = AdState.Ready;
            _rewardReloadCount = 0;
        });
    }

    public void ShowRewardedAd(UnityAction RewardComplete)
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            OpenAdSpaceTimeCounter = 0;
            _rewardedAd.Show((Reward reward) => UnityMainThreadDispatcher.Instance().Enqueue(() => RewardComplete?.Invoke()));
        }
    }

    //[Header("AD REWARD SECOND")]
    //public bool IsPreloadRewardSecond = true;
    //public AdState RewardSecondAdState = AdState.NotAvailable;
    //public int _rewardSecondLoadCount = 0;
    //public string _adUnitRewardSecondId = "ca-app-pub-5904408074441373/9267338743";

    //private RewardedAd _rewardedSecondAd;
    //private void LoadRewardedSecondAd()
    //{
    //    if (RewardSecondAdState == AdState.Loading) return;
    //    RewardSecondAdState = AdState.Loading;

    //    if (_rewardedSecondAd != null)
    //    {
    //        _rewardedSecondAd.Destroy();
    //        _rewardedSecondAd = null;
    //    }
    //    var adRequest = new AdRequest();

    //    RewardedAd.Load(_adUnitRewardSecondId, adRequest, (RewardedAd ad, LoadAdError error) =>
    //    {
    //        if (error != null || ad == null)
    //        {
    //            RewardSecondAdState = AdState.NotAvailable;
    //            _rewardSecondLoadCount += 1;
    //            return;
    //        }

    //        _rewardedSecondAd = ad;
    //        _rewardedSecondAd.OnAdFullScreenContentClosed += () => { RewardSecondAdState = AdState.NotAvailable; };
    //        _rewardedSecondAd.OnAdFullScreenContentFailed += (AdError error) => { RewardSecondAdState = AdState.NotAvailable; };
    //        RewardSecondAdState = AdState.Ready;
    //        _rewardSecondLoadCount = 0;
    //    });
    //}

    //public void ShowRewardedSecondAd(UnityAction RewardComplete)
    //{
    //    if (_rewardedSecondAd != null && _rewardedSecondAd.CanShowAd())
    //    {
    //        OpenAdSpaceTimeCounter = 0;
    //        _rewardedSecondAd.Show((Reward reward) => UnityMainThreadDispatcher.Instance().Enqueue(() => RewardComplete?.Invoke()));
    //    }
    //}

    [Header("AD REWARD THIRD")]
    public bool IsPreloadRewardThrid = true;
    public AdState RewardThridAdState = AdState.NotAvailable;
    public int _rewardThridLoadCount = 0;
    public string _adUnitRewardThriddId = "ca-app-pub-5904408074441373/7387280867";

    private RewardedAd _rewardedThridAd;

    private void LoadRewardedThridAd()
    {
        if (RewardThridAdState == AdState.Loading) return;
        RewardThridAdState = AdState.Loading;

        if (_rewardedThridAd != null)
        {
            _rewardedThridAd.Destroy();
            _rewardedThridAd = null;
        }

        var adRequest = new AdRequest();
        RewardedAd.Load(_adUnitRewardThriddId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                RewardThridAdState = AdState.NotAvailable;
                _rewardThridLoadCount += 1;
                return;
            }

            _rewardedThridAd = ad;
            _rewardedThridAd.OnAdPaid += (revenue) => { AppflyerEventSender.Instance.logAdRevenue(revenue); };
            _rewardedThridAd.OnAdFullScreenContentClosed += () => { RewardThridAdState = AdState.NotAvailable; };
            _rewardedThridAd.OnAdFullScreenContentFailed += (AdError error) => { RewardThridAdState = AdState.NotAvailable; };
            RewardThridAdState = AdState.Ready;
            _rewardThridLoadCount = 0;
        });
    }

    public void ShowRewardedThridAd(UnityAction RewardComplete)
    {
        if (_rewardedThridAd != null && _rewardedThridAd.CanShowAd())
        {
            OpenAdSpaceTimeCounter = 0;
            _rewardedThridAd.Show((Reward reward) => UnityMainThreadDispatcher.Instance().Enqueue(() => RewardComplete?.Invoke()));
        }
    }

    [Header("AD OPEN")]
    public GameObject LoadingOpenAdCanvas;
    public bool IsPreloadOpen = true;
    public AdState OpenAdState = AdState.NotAvailable;
    public int _openReloadCount = 0;
    public string _adUnitOpenId = "ca-app-pub-5904408074441373/7523012234";
    private AppOpenAd appOpenAd;

    public float OpenAdSpaceTimeCounter = 0.0f;
    public float OpenAdSpaceTime = 5.0f;

    private void CaculaterCounterOpenAd()
    {
        if (RuntimeStorageData.Player.IsLoadAds == false) return;
        OpenAdSpaceTimeCounter += Time.deltaTime;
    }    

    private void LoadAppOpenAd()
    {
        if (RuntimeStorageData.Player.IsLoadAds == false) return;
        if (OpenAdState == AdState.Loading) return;
        OpenAdState = AdState.Loading;

        if (appOpenAd != null)
        {
            appOpenAd.Destroy();
            appOpenAd = null;
        }

        var adRequest = new AdRequest();
        AppOpenAd.Load(_adUnitOpenId, adRequest,
            (AppOpenAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    OpenAdState = AdState.NotAvailable;
                    _openReloadCount += 1;
                    return;
                }

                appOpenAd = ad;
                appOpenAd.OnAdPaid += (revenue) => { AppflyerEventSender.Instance.logAdRevenue(revenue); };

                appOpenAd.OnAdFullScreenContentClosed += () => 
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => LoadingOpenAdCanvas.Hide());
                    OpenAdState = AdState.NotAvailable; 
                };

                appOpenAd.OnAdFullScreenContentFailed += (AdError error) => 
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => LoadingOpenAdCanvas.Hide());
                    OpenAdState = AdState.NotAvailable; 
                };
                OpenAdState = appOpenAd.CanShowAd() == true ? AdState.Ready : AdState.NotAvailable;
                _openReloadCount = 0;
            });
    }

    public void CheckingOpenAd()
    {
        if (OpenAdSpaceTimeCounter < OpenAdSpaceTime) return;
        Debug.Log($"[{this.GetType().ToString()}] Checking Open Ad");
        OpenAdSpaceTimeCounter = 0;
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            if (RuntimeStorageData.Player.IsLoadAds == false) return;
            ShowAppOpenAd();
        });
    }

    private bool IsAdAvailable
    {
        get { return appOpenAd != null && appOpenAd.CanShowAd() == true; }
    }

    private void ShowAppOpenAd()
    {
        if (IsAdAvailable)
        {
            Debug.Log($"[{this.GetType().ToString()}] Showing app open ad.");
            UnityMainThreadDispatcher.Instance().Enqueue(() => LoadingOpenAdCanvas.Show());
            DOVirtual.DelayedCall(0.1f, () => appOpenAd.Show());
            //appOpenAd.Show();
        }
    }
#else

    [HideInInspector] public float InterAdSpaceTimeAutoCounter = 0;
    [HideInInspector] public float OpenAdSpaceTimeCounter = 0;

    private void Start()
    {
        Manager.Instance.CompleteOpenAd();
    }

    public void ShowRewardedAd(UnityAction Callback)
    {
        Debug.Log($"[{this.GetType().ToString()}] Show Rewarded Ad.");
        Callback?.Invoke();
    }

    public void ShowRewardedSecondAd(UnityAction Callback)
    {
        Callback?.Invoke();
    }

    public void ShowRewardedThridAd(UnityAction Callback)
    {
        Callback?.Invoke();
    }

    public void ShowInterstitialAdWithSpaceTime(UnityAction Callback)
    {
        Debug.Log($"[{this.GetType().ToString()}] Show Interstitial Ad With Space Time.");
        Callback?.Invoke();
    }

    public void ShowInterstitialAd(UnityAction Callback)
    {
        Debug.Log($"[{this.GetType().ToString()}] Show Interstitial Ad.");
        Callback?.Invoke();
    }

    public void ShowInterstitialHomeAd(UnityAction Callback, UnityAction CallbackAfterInter)
    {
        if (Callback != null)
            Callback?.Invoke();
    }

    public void CheckingOpenAd()
    {
        Debug.Log($"[{this.GetType().ToString()}] Checking Open Ad.");
    }
#endif
        }
