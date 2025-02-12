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
public class AdManager : MonoSingletonGlobal<AdManager>
{
#if ADMOB
    public enum AdState
    {
        Loading, Ready, NotAvailable
    }

    public enum AdBannerSize
    {
        Banner,
        FullWidth
    }
    public bool IsInitalized = false;
    public bool IsBannerShow = false;
    public bool IsBannerMREC = false;

    [Header("Ad Manager")]
#if UNITY_ANDROID
    public string _adUnitId = "ca-app-pub-5904408074441373~6318315191";
#elif UNITY_IPHONE
    public string _adUnitId = "ca-app-pub-5904408074441373~6917230354";
#else
    public string _adUnitId = "unused";
#endif


    void Start()
    {
        StartCoroutine(Bacon.UMP.Instance.DOGatherConsent(LoadAds()));
    }   

    private IEnumerator LoadAds()
    {
        yield return new WaitUntil(() => Bacon.UMP.Instance.IsUMPReady);
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
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
#if !UNITY_EDITOR
        yield return WaitForSecondCache.WAIT_TIME_EIGHT;
#endif
        if (UseReward == true)
        {
            LoadRewardedAd();
            LoadRewardedSecondAd();
            LoadRewardedThridAd();
        }      
        if (UseInterstitial == true)
        {
            if (RuntimeStorageData.Player.IsLoadAds == true)
            {
                LoadInterstitialAd();
                LoadInterstitialHomeAd();
            }
        }
        if (UseOpen == true)
        {
            if (RuntimeStorageData.Player.IsLoadAds == true)
            {
                LoadAppOpenAd();
            }
        }    
    }


    private float AfterAdReload = 10.0f;
    private float TimerBannerAutoReload = 0.0f;
    private float TimerBannerAdReload = 0.0f;
    private float TimerBannerMERCAdReload = 0.0f;
    private float TimerInterstitialAdReload = 0.0f;
    private float TimerInterstitialHomeAdReload = 0.0f;
    private float TimerRewardedAdReload = 0.0f;
    private float TimerRewardedSecondAdReload = 0.0f;
    private float TimerRewardedThirdAdReload = 0.0f;
    private float TimerOpenAdReload = 0.0f;


    private float timerLogUMP = 0;

    private void Update()
    {
        if (IsInitalized == false)
            return;

        if (IsPreloadInterstitial)
        {
            if (RuntimeStorageData.Player.IsLoadAds == false)
                return;
            TimerInterstitialAdReload += Time.deltaTime;
            if (TimerInterstitialAdReload > AfterAdReload)
            {
                if (InterHomeAdState == AdState.NotAvailable /*&& _interstitalReloadCount <= 10*/)
                {
                    TimerInterstitialHomeAdReload = 0;
                    LoadInterstitialHomeAd();
                }    
            }
        }

        if (IsPreloadinterstitialHome)
        {
            if (RuntimeStorageData.Player.IsLoadAds == false)
                return;
            TimerInterstitialHomeAdReload += Time.deltaTime;
            if (TimerInterstitialHomeAdReload > AfterAdReload)
            {
                if (InterAdState == AdState.NotAvailable /*&& _interstitalReloadCount <= 10*/)
                {
                    TimerInterstitialAdReload = 0;
                    LoadInterstitialAd();
                }
            }
        }

        CaculaterCounterInterAd();
        CaculaterCounterInterOpenAd();

        if (IsPreloadReward)
        {
            if (RewardAdState == AdState.NotAvailable /*&& _rewardReloadCount <= 10*/)
                LoadRewardedAd();
        }

        if (IsPreloadRewardSecond)
        {
            if (RewardSecondAdState == AdState.NotAvailable /*&& _rewardReloadCount <= 10*/)
                LoadRewardedSecondAd();
        }

        if (IsPreloadRewardThrid)
        {
            if (RewardThridAdState == AdState.NotAvailable /*&& _rewardReloadCount <= 10*/)
                LoadRewardedThridAd();
        }

        if (IsPreloadOpen)
        {
            if (RuntimeStorageData.Player.IsLoadAds == false)
                return;
            TimerOpenAdReload += Time.deltaTime;
            if (TimerOpenAdReload > AfterAdReload)
            {
                if (OpenAdState == AdState.NotAvailable /*&& _openReloadCount <= 10*/)
                {
                    TimerOpenAdReload = 0;
                    LoadAppOpenAd();
                }    
            }
        }
        CaculaterCounterOpenAd();
    }

    public UnityAction ActionOnAfterInterstitalAd;
    [Header("Ad Interstitial")]
    public bool UseInterstitial = true;
    public GameObject _loadingInterstitalAd;

    public void ResetInterstitialAdCounter()
    {
        InterAdSpaceTimeAutoCounter = 0;
        InterHomeAdSpaceTimeAutoCounter = 0;
    }    


    public bool IsPreloadInterstitial = true;
    public AdState InterAdState = AdState.NotAvailable;
    public enum AdShowState
    {
        None,
        Pending
    }

    public AdShowState InterAdShowState = AdShowState.None;

    public int _interstitalReloadCount = 0;

    public float InterAdSpaceTimeAutoCounter = 0;
#if UNITY_ANDROID
    public string _adUnitInterId = "ca-app-pub-5904408074441373/8357882100";
#elif UNITY_IPHONE
    public string _adUnitInterId = "ca-app-pub-5904408074441373/5604148681";
#else
    public string _adUnitInterId = "unused";
#endif

    private InterstitialAd _interstitialAd;
    private InterstitialAd _interstitialHomeAd;
    private InterstitialAd _interstitialOpenAd;

    public void LoadInterstitialAd()
    {
        if (RuntimeStorageData.Player.IsLoadAds == false)
            return;
        if (InterAdState == AdState.Loading)
            return;
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
            ListenToInterAdEvents();
            InterAdState = AdState.Ready;
            _interstitalReloadCount = 0;
        });
    }

    public void ShowInterstitialAd(UnityAction Callback)
    {
        if (RuntimeStorageData.Player.IsLoadAds == false)
        {
            Callback?.Invoke();
            return;
        }
        if (InterAdShowState == AdShowState.Pending)
            return;

        if (_interstitialAd != null && 
            _interstitialAd.CanShowAd())
        {
            Debug.Log($"[{this.GetType().ToString()}] Showing interstitial ad.");
            InterAdShowState = AdShowState.Pending;
            _loadingInterstitalAd?.SetActive(true);
            OpenAdSpaceTimeCounter = 0;

            ActionOnAfterInterstitalAd = () =>
            {
                InterAdShowState = AdShowState.None;
                InterAdState = AdState.NotAvailable;
                if (Callback != null)
                    Callback?.Invoke();
            };
            CoroutineUtils.PlayCoroutine(() =>
            {
                _loadingInterstitalAd?.SetActive(false);
                _interstitialAd.Show();
            }, 1.0f);

            ResetInterstitialAdCounter();
        }
        else
        {
            Debug.Log($"[{this.GetType().ToString()}] Interstitial ad is not ready yet.");
            if (Callback != null)
                Callback?.Invoke();
        }
    }

    private void ListenToInterAdEvents()
    {
        _interstitialAd.OnAdFullScreenContentClosed += () => { ActionOnAfterInterstitalAd?.Invoke(); };
        _interstitialAd.OnAdFullScreenContentFailed += (AdError error) => { ActionOnAfterInterstitalAd?.Invoke(); };
    }

    [Header("Ad Home Interstitial")]

    public bool IsPreloadinterstitialHome = true;
    public AdState InterHomeAdState = AdState.NotAvailable;
    public AdShowState InterHomeAdShowState = AdShowState.None;
    public int _interstitialHomeAdReloadCount = 0;

    public float InterHomeAdSpaceTimeAutoCounter = 0;

#if UNITY_ANDROID
    public string _adUnitInterHomeId = "ca-app-pub-5904408074441373/8357882100";
#elif UNITY_IPHONE
    public string _adUnitInterHomeId = "ca-app-pub-5904408074441373/5604148681";
#else
    public string _adUnitInterHomeId = "unused";
#endif

    public void LoadInterstitialHomeAd()
    {
        if (RuntimeStorageData.Player.IsLoadAds == false)
            return;
        if (InterHomeAdState == AdState.Loading)
            return;
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
            InterHomeAdState = AdState.Ready;
            ListenToInterHomeAdEvents();
            _interstitialHomeAdReloadCount = 0;
        });
    }

    public void ShowInterstitialHomeAd(UnityAction Callback, UnityAction CallbackAfterInter)
    {
        if (RuntimeStorageData.Player.IsLoadAds == false)
        {
            Callback?.Invoke();
            CallbackAfterInter?.Invoke();
            return;
        }    
        if (InterHomeAdShowState == AdShowState.Pending)
            return;
        if (InterHomeAdSpaceTimeAutoCounter < Manager.Instance.InterHomeReloadTimer)
        {
            if (Callback != null)
                Callback?.Invoke();
            return;
        }    

        if (_interstitialHomeAd != null &&
            _interstitialHomeAd.CanShowAd())
        {
            Debug.Log($"[{this.GetType().ToString()}] Showing interstitial home ad.");
            InterHomeAdShowState = AdShowState.Pending;
            OpenAdSpaceTimeCounter = 0;

            ActionOnAfterInterstitalAd = () =>
            {
                InterHomeAdShowState = AdShowState.None;
                InterHomeAdState = AdState.NotAvailable;
                if (CallbackAfterInter != null)
                    CallbackAfterInter?.Invoke();
                if (Callback != null)
                    Callback?.Invoke();
            };
            _interstitialHomeAd.Show();
            ResetInterstitialAdCounter();
        }
        else
        {
            if (Callback != null)
                Callback?.Invoke();
        }
    }

    private void ListenToInterHomeAdEvents()
    {
        _interstitialHomeAd.OnAdFullScreenContentClosed += () => { ActionOnAfterInterstitalAd?.Invoke(); };
        _interstitialHomeAd.OnAdFullScreenContentFailed += (AdError error) => { ActionOnAfterInterstitalAd?.Invoke(); };
    }

    [Header("Ad Open Interstitial")]
#if UNITY_ANDROID
    public string _adUnitInterOpenId = "ca-app-pub-5904408074441373/7180531807";
#elif UNITY_IPHONE
    public string _adUnitInterOpenId = "ca-app-pub-5904408074441373/4650159660";
#else
    public string _adUnitInterId = "unused";
#endif

    public float InterOpenAdMaximusTimeCounter = 0.0f;
    public float InterOpenAdMaximusTime = 15.0f;

    public void PromiseShowInterstitialAd(UnityAction afterAd)
    {
        if (RuntimeStorageData.Player.IsLoadAds == false)
        {
            afterAd?.Invoke();
            return;
        } 
        Debug.Log($"[{this.GetType().ToString()}] Promise show interstitial open Ad");
        var adRequest = new AdRequest();
        InterstitialAd.Load(_adUnitInterOpenId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                return;
            }
            _interstitialOpenAd = ad;
            _interstitialOpenAd.OnAdPaid += (AdValue adValue) => { };
            _interstitialOpenAd.OnAdFullScreenContentClosed += () => { ActionOnAfterInterstitalAd?.Invoke(); };
            _interstitialOpenAd.OnAdFullScreenContentFailed += (AdError error) => { ActionOnAfterInterstitalAd?.Invoke(); };

            ActionOnAfterInterstitalAd = () => { afterAd?.Invoke(); };
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
            {
                Manager.Instance.CompleteOpenAd();
            }
        }
    }
    private void CaculaterCounterInterAd()
    {
        if (Manager.Instance.IsIngame == true && Manager.Instance.IngameScreenID == "GameUICanvas")
        {
            InterAdSpaceTimeAutoCounter += Time.deltaTime;
            if (InterAdSpaceTimeAutoCounter > Manager.Instance.InterAutoReloadTimer)
            {
                InterAdSpaceTimeAutoCounter = 0;
                ShowInterstitialAd(() =>
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        CanvasSystem.Instance.ShowNativeIntertitial();
                    });
                });
            }
        }

        if (Manager.Instance.IsIngame == true)
        {
            InterHomeAdSpaceTimeAutoCounter += Time.deltaTime;
        }
    }



    [Header("Ad Reward")]
    public bool UseReward = true;
    public bool IsPreloadReward = true;
    public AdState RewardAdState = AdState.NotAvailable;
    public int _rewardReloadCount = 0;
#if UNITY_ANDROID
    public string _adUnitRewardId = "ca-app-pub-5904408074441373/5867450136";
#elif UNITY_IPHONE
    public string _adUnitRewardId = "ca-app-pub-5904408074441373/4291067011";
#else
    public string _adUnitRewardId = "unused";
#endif

    private RewardedAd _rewardedAd;

    public void LoadRewardedAd()
    {
        if (RewardAdState == AdState.Loading)
            return;
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
            ListenToRewardAdEvents();
            RewardAdState = AdState.Ready;
            _rewardReloadCount = 0;
        });
    }

    public void ShowRewardedAd(UnityAction RewardComplete)
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            OpenAdSpaceTimeCounter = 0;
            _rewardedAd.Show((Reward reward) =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    RewardComplete?.Invoke();
                });
            });
        }
    }

    private void ListenToRewardAdEvents()
    {
        _rewardedAd.OnAdFullScreenContentClosed += () => { RewardAdState = AdState.NotAvailable; };
        _rewardedAd.OnAdFullScreenContentFailed += (AdError error) => { RewardAdState = AdState.NotAvailable; };
    }

    [Header("Ad reward second")]
    public bool IsPreloadRewardSecond = true;
    public AdState RewardSecondAdState = AdState.NotAvailable;
    public int _rewardSecondLoadCount = 0;

#if UNITY_ANDROID
    public string _adUnitRewardSecondId = "ca-app-pub-5904408074441373/5867450136";
#elif UNITY_IPHONE
    public string _adUnitRewardSecondId = "ca-app-pub-5904408074441373/4291067011";
#else
    public string _adUnitRewardSecondId = "unused";
#endif

    private RewardedAd _rewardedSecondAd;
    public void LoadRewardedSecondAd()
    {
        if (RewardSecondAdState == AdState.Loading)
            return;
        RewardSecondAdState = AdState.Loading;

        if (_rewardedSecondAd != null)
        {
            _rewardedSecondAd.Destroy();
            _rewardedSecondAd = null;
        }
        var adRequest = new AdRequest();

        RewardedAd.Load(_adUnitRewardSecondId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                RewardSecondAdState = AdState.NotAvailable;
                _rewardSecondLoadCount += 1;
                return;
            }

            _rewardedSecondAd = ad;
            ListenToRewardSecondAdEvents();
            RewardSecondAdState = AdState.Ready;
            _rewardSecondLoadCount = 0;
        });
    }

    public void ShowRewardedSecondAd(UnityAction RewardComplete)
    {
        if (_rewardedSecondAd != null && _rewardedSecondAd.CanShowAd())
        {
            OpenAdSpaceTimeCounter = 0;
            _rewardedSecondAd.Show((Reward reward) =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    RewardComplete?.Invoke();
                });
            });
        }
    }

    private void ListenToRewardSecondAdEvents()
    {
        _rewardedSecondAd.OnAdFullScreenContentClosed += () => { RewardSecondAdState = AdState.NotAvailable; };
        _rewardedSecondAd.OnAdFullScreenContentFailed += (AdError error) => { RewardSecondAdState = AdState.NotAvailable; };
    }

    [Header("Ad reward third")]
    public bool IsPreloadRewardThrid = true;
    public AdState RewardThridAdState = AdState.NotAvailable;
    public int _rewardThridLoadCount = 0;

#if UNITY_ANDROID
    public string _adUnitRewardThriddId = "ca-app-pub-5904408074441373/5867450136";
#elif UNITY_IPHONE
    public string _adUnitRewardThriddId = "ca-app-pub-5904408074441373/4291067011";
#else
    public string _adUnitRewardThriddId = "unused";
#endif

    private RewardedAd _rewardedThridAd;

    public void LoadRewardedThridAd()
    {
        if (RewardThridAdState == AdState.Loading)
            return;
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
            ListenToRewardThridAdEvents();
            RewardThridAdState = AdState.Ready;
            _rewardThridLoadCount = 0;
        });
    }

    public void ShowRewardedThridAd(UnityAction RewardComplete)
    {
        if (_rewardedThridAd != null && _rewardedThridAd.CanShowAd())
        {
            OpenAdSpaceTimeCounter = 0;
            _rewardedThridAd.Show((Reward reward) =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    RewardComplete?.Invoke();
                });
            });
        }
    }

    private void ListenToRewardThridAdEvents()
    {
        _rewardedThridAd.OnAdFullScreenContentClosed += () => { RewardThridAdState = AdState.NotAvailable; };
        _rewardedThridAd.OnAdFullScreenContentFailed += (AdError error) => { RewardThridAdState = AdState.NotAvailable; };
    }

    [Header("Ad Open")]
    public bool UseOpen = true;
    public bool IsPreloadOpen = true;
    public AdState OpenAdState = AdState.NotAvailable;
    public int _openReloadCount = 0;
#if UNITY_ANDROID
    public string _adUnitOpenId = "ca-app-pub-5904408074441373/7978608159";
#elif UNITY_IPHONE
    public string _adUnitOpenId = "ca-app-pub-5904408074441373/2977985343";
#else
    public string _adUnitOpenId = "unused";
#endif
    private AppOpenAd appOpenAd;

    public float OpenAdSpaceTimeCounter = 0.0f;
    public float OpenAdSpaceTime = 3.0f;

    public void CaculaterCounterOpenAd()
    {
        if (RuntimeStorageData.Player.IsLoadAds == false)
            return;
        OpenAdSpaceTimeCounter += Time.deltaTime;
    }    

    public void LoadAppOpenAd()
    {
        if (RuntimeStorageData.Player.IsLoadAds == false)
            return;
        if (OpenAdState == AdState.Loading)
            return;
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
                ListenToOpenAdEvents();
                OpenAdState = appOpenAd.CanShowAd() == true ? AdState.Ready : AdState.NotAvailable;
                _openReloadCount = 0;
            });
    }

    private void ListenToOpenAdEvents()
    {
        appOpenAd.OnAdFullScreenContentClosed += () => { OpenAdState = AdState.NotAvailable; };
        appOpenAd.OnAdFullScreenContentFailed += (AdError error) => { OpenAdState = AdState.NotAvailable; };
    }

    public void CheckingOpenAd()
    {
        if (OpenAdSpaceTimeCounter < OpenAdSpaceTime)
            return;
        Debug.Log($"[{this.GetType().ToString()}] Checking Open Ad");
        OpenAdSpaceTimeCounter = 0;
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            if (RuntimeStorageData.Player.IsLoadAds == false)
                return;
            ShowAppOpenAd();
        });
    }

    public bool IsAdAvailable
    {
        get { return appOpenAd != null && appOpenAd.CanShowAd() == true; }
    }

    public void ShowAppOpenAd()
    {
        if (IsAdAvailable)
        {
            Debug.Log($"[{this.GetType().ToString()}] Showing app open ad.");
            appOpenAd.Show();
        }
    }
#else

    public float InterAdSpaceTimeAutoCounter = 0;

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
