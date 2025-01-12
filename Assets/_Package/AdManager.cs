using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using EditorCools;
using DG.Tweening;


#if ADMOB
using GoogleMobileAds.Api;
#endif

public class AdManager : MonoSingletonGlobal<AdManager>
{
#if ADMOB
    //public bool IsUsingUMP = true;
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
            PromiseShowInterstitialAd(null, () =>
            {
                if (StoryManager.Instance != null)
                    StoryManager.Instance.CompleteLoading();
            });
            Dictionary<string, AdapterStatus> map = initStatus.getAdapterStatusMap();
            foreach (KeyValuePair<string, AdapterStatus> keyValuePair in map)
            {
                string className = keyValuePair.Key;
                AdapterStatus status = keyValuePair.Value;
                switch (status.InitializationState)
                {
                    case AdapterState.NotReady:
                        // The adapter initialization did not complete.
                        MonoBehaviour.print("Adapter: " + className + " not ready.");
                        break;
                    case AdapterState.Ready:
                        // The adapter was successfully initialized.
                        MonoBehaviour.print("Adapter: " + className + " is initialized.");
                        break;
                }
            }
        });
        yield return new WaitUntil(() => IsInitalized);
        yield return WaitForSecondCache.WAIT_TIME_EIGHT;
        LoadRewardedAd();
        //LoadBannerAd();
        LoadInterstitialAd();
        LoadAppOpenAd();


    }

    private float AfterAdReload = 10.0f;
    private float TimerBannerAdReload = 0.0f;
    private float TimerInterstitialAdReload = 0.0f;
    private float TimerRewardedAdReload = 0.0f;
    private float TimerOpenAdReload = 0.0f;


    private float timerLogUMP = 0;

    private void Update()
    {
        if (IsInitalized == false)
            return;

        //if (IsPreloadBanner)
        //{
        //    TimerBannerAdReload += Time.deltaTime;
        //    if (TimerBannerAdReload > AfterAdReload)
        //    {
        //        if (BannerAdState == AdState.NotAvailable /*&& _interstitalReloadCount <= 10*/)
        //        {
        //            TimerBannerAdReload = 0;
        //            LoadBannerAd();
        //        }
        //    }
        //}

        if (IsPreloadInterstitial)
        {
            TimerInterstitialAdReload += Time.deltaTime;
            if (TimerInterstitialAdReload > AfterAdReload)
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
            TimerRewardedAdReload += Time.deltaTime;
            if (TimerRewardedAdReload > AfterAdReload)
            {
                TimerRewardedAdReload = 0;
                if (RewardAdState == AdState.NotAvailable /*&& _rewardReloadCount <= 10*/)
                    LoadRewardedAd();
            }
        }

        if (IsPreloadOpen)
        {
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

        //AdManager.Instance.ShowInterstitialAdAutomatic();
    }

    [Header("Ad Banner")]
    public bool IsPreloadBanner = true;
    public bool IsBanner = false;
    public AdState BannerAdState = AdState.NotAvailable;
    public AdBannerSize _status = AdBannerSize.FullWidth;
    //public GameObject _bannerOverlay;
    public int _bannerReloadCount = 0;
#if UNITY_ANDROID
    public string _adUnitBannerId = "";
#elif UNITY_IPHONE
    public string _adUnitBannerId = "";
#else
    public string _adUnitBannerId = "unused";
#endif
    BannerView _bannerView;

    /// <summary>
    /// Creates a 320x50 banner view at top of the screen.
    /// </summary>
    public void CreateBannerView()
    {
        Debug.Log($"[{this.GetType().ToString()}] Creating banner view");

        // If we already have a banner, destroy the old one.
        if (_bannerView != null)
        {
            DestroyBannerView();
        }
        int w = 0;
        int bannerHeight = 50;
        int h = Screen.height / 2 - bannerHeight / 2;

        AdSize adaptiveSize = _status == AdBannerSize.FullWidth ? AdSize.GetLandscapeAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth) : AdSize.Banner;
        Debug.Log($"[{this.GetType().ToString()}] adaptiveSize " + adaptiveSize);
        // Create a 320x50 banner at top of the screen
        _bannerView = new BannerView(_adUnitBannerId, adaptiveSize, AdPosition.BottomLeft);
        ListenToBannerAdEvents();
    }

    public void LoadBannerAd(bool IsHide = true)
    {
        if (RuntimeStorageData.Player.IsAds)
            return;
        if (BannerAdState == AdState.Loading)
            return;
        BannerAdState = AdState.Loading;

        // create an instance of a banner view first.
        if (_bannerView == null)
        {
            CreateBannerView();
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        Debug.Log($"[{this.GetType().ToString()}] Loading banner ad.");
        _bannerView.LoadAd(adRequest);
        if (IsBanner) ShowBannerAd();
        else HideBannerAd();
    }

    /// <summary>
    /// Destroys the banner view.
    /// </summary>
    public void DestroyBannerView()
    {
        if (_bannerView != null)
        {
            BannerAdState = AdState.NotAvailable;
            Debug.Log($"[{this.GetType().ToString()}] Destroying banner view.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }

    /// <summary>
    /// listen to events the banner view may raise.
    /// </summary>
    private void ListenToBannerAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        _bannerView.OnBannerAdLoaded += () =>
        {
            _bannerReloadCount = 0;
            BannerAdState = AdState.Ready;
            Debug.Log($"[{this.GetType().ToString()}] Banner view loaded an ad with response : "
                + _bannerView.GetResponseInfo());
            Debug.Log(string.Format("[AdManager] Ad Height: {0}, width: {1}", _bannerView.GetHeightInPixels(), _bannerView.GetWidthInPixels()));
        };
        // Raised when an ad fails to load into the banner view.
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            _bannerReloadCount += 1;
            BannerAdState = AdState.NotAvailable;
            Debug.Log($"[{this.GetType().ToString()}] Banner view failed to load an ad with error : "
                + error);
        };
        // Raised when the ad is estimated to have earned money.
        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            //AppflyerEventSender.Instance.logAdRevenue(adValue);
            Debug.Log(String.Format("Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        _bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        _bannerView.OnAdClicked += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        _bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Banner view full screen content closed.");
        };
    }

    public void ShowBannerAd()
    {
        IsBanner = true;
        if (RuntimeStorageData.Player.IsAds)
            return;
        //if (_bannerOverlay != null)
        //    _bannerOverlay.SetActive(true);
        //if (_bannerView != null)
        //    _bannerView.Show();
    }

    public void HideBannerAd()
    {
        IsBanner = false;
        //if (_bannerOverlay != null)
        //    _bannerOverlay.SetActive(false);
        //if (_bannerView != null)
        //    _bannerView.Hide();
    }


    public UnityAction ActionOnAfterInterstitalAd;
    [Header("Ad Interstitial")]
    public bool IsPreloadInterstitial = true;
    public AdState InterAdState = AdState.NotAvailable;
    public enum AdShowState
    {
        None,
        Pending
    }

    public AdShowState InterAdShowState = AdShowState.None;

    public int _interstitalReloadCount = 0;
    public GameObject _loadingInterstitalAd;

    public float InterAdSpaceTimeCounter = 0.0f;
    public float InterAdSpaceTimeAutoCounter = 0;
    public float InterAdSpaceTime = 15.0f;
    public float InterAdSpaceTimeAuto = 60.0f;
    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    public string _adUnitInterId = "ca-app-pub-5904408074441373/8357882100";
    [Header("Ad Open Interstitial")]
    public string _adUnitInterOpenId = "ca-app-pub-5904408074441373/7180531807";
#elif UNITY_IPHONE
    public string _adUnitInterId = "ca-app-pub-5904408074441373/5604148681";
    [Header("Ad Open Interstitial")]
    public string _adUnitInterOpenId = "ca-app-pub-5904408074441373/4650159660";
#else
    public string _adUnitInterId = "unused";
#endif

    public float InterOpenAdMaximusTimeCounter = 0.0f;
    public float InterOpenAdMaximusTime = 15.0f;

    private void CaculaterCounterInterOpenAd()
    {
        if(InterOpenAdMaximusTimeCounter <= InterOpenAdMaximusTime)
        {
            InterOpenAdMaximusTimeCounter += Time.deltaTime;
            if (InterOpenAdMaximusTimeCounter > InterOpenAdMaximusTime)
            {
                StoryManager.Instance.CompleteLoading();
            }    
        }    
    }    


    private void CaculaterCounterInterAd()
    {
        InterAdSpaceTimeCounter += Time.deltaTime;
        if(RuntimeStorageData.Player.NumberOfDay > 3 && Manager.Instance.IsIngame == true)
        {
            InterAdSpaceTimeAutoCounter += Time.deltaTime;
            if (InterAdSpaceTimeAutoCounter > InterAdSpaceTimeAuto)
            {
                InterAdSpaceTimeAutoCounter = 0;
                ShowInterstitialAd(null);
            }    
        }    
    }    

    private InterstitialAd _interstitialAd;
    private InterstitialAd _interstitialOpenAd;

    /// <summary>
    /// Loads the interstitial ad.
    /// </summary>
    public void LoadInterstitialAd()
    {
        if (RuntimeStorageData.Player.IsAds)
            return;
        if (InterAdState == AdState.Loading)
            return;
        InterAdState = AdState.Loading;

        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log($"[{this.GetType().ToString()}] Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(_adUnitInterId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            // if error is not null, the load request failed.
            if (error != null || ad == null)
            {
                InterAdState = AdState.NotAvailable;
                _interstitalReloadCount += 1;
                Debug.Log($"[{this.GetType().ToString()}] interstitial ad failed to load an ad " +
                               "with error : " + error);
                return;
            }

            Debug.Log($"[{this.GetType().ToString()}] Interstitial ad loaded with response : "
                      + ad.GetResponseInfo());

            _interstitialAd = ad;
            ListenToInterAdEvents();
            InterAdState = AdState.Ready;
            _interstitalReloadCount = 0;
        });
    }

    /// <summary>
    /// Show the interstitial ad with space time.
    /// </summary>
    public void ShowInterstitialAdWithSpaceTime(UnityAction CALLBACK)
    {
        Debug.Log($"[{this.GetType().ToString()}] Show Interstitial Ad With Space Time");
        if (InterAdSpaceTimeCounter > InterAdSpaceTime)
        {
            InterAdSpaceTimeCounter = 0;
            ShowInterstitialAd(CALLBACK);
        }
        else
        {
            if (CALLBACK != null)
                CALLBACK?.Invoke();
        }
    }

    /// <summary>
    /// Shows the interstitial ad.
    /// </summary>
    /// 
    public void ShowInterstitialAd(UnityAction CALLBACK)
    {
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
                if (CALLBACK != null)
                    CALLBACK?.Invoke();
                _loadingInterstitalAd?.SetActive(false);
            };
            CoroutineUtils.PlayCoroutine(() =>
            {
                _interstitialAd.Show();
            }, 1.0f);
        }
        else
        {
            Debug.Log($"[{this.GetType().ToString()}] Interstitial ad is not ready yet.");
            if (CALLBACK != null)
                CALLBACK?.Invoke();
        }
    }

    public void PromiseShowInterstitialAd(UnityAction showAd, UnityAction afterAd)
    {
        Debug.Log($"[{this.GetType().ToString()}] Promise show interstitial open Ad");
        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(_adUnitInterOpenId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            // if error is not null, the load request failed.
            if (error != null || ad == null)
            {
                Debug.Log($"[{this.GetType().ToString()}] interstitial ad failed to load an ad " +
                               "with error : " + error);
                return;
            }

            Debug.Log($"[{this.GetType().ToString()}] Interstitial ad loaded with response : "
                      + ad.GetResponseInfo());

            _interstitialOpenAd = ad;
            _interstitialOpenAd.OnAdPaid += (AdValue adValue) =>
            {
                Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when the ad closed full screen content.
            _interstitialOpenAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log($"[{this.GetType().ToString()}] Interstitial ad full screen content closed.");
                ActionOnAfterInterstitalAd?.Invoke();
            };
            // Raised when the ad failed to open full screen content.
            _interstitialOpenAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
                Debug.Log($"[{this.GetType().ToString()}] Interstitial ad failed to open full screen content " +
                               "with error : " + error);
                ActionOnAfterInterstitalAd?.Invoke();
            };

            Debug.Log($"[{this.GetType().ToString()}] Showing interstitial ad.");
            showAd?.Invoke();
            ActionOnAfterInterstitalAd = () =>
            {
                afterAd?.Invoke();
            };

            if (InterOpenAdMaximusTimeCounter <= InterOpenAdMaximusTime)
            {
                InterOpenAdMaximusTimeCounter = InterOpenAdMaximusTime;
                _interstitialOpenAd.Show();
            }
            else
            {
                Debug.Log("Out of time show open ad");
            }    
        });
    }

    private void ListenToInterAdEvents()
    {
        // Raised when the ad is estimated to have earned money.
        _interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            //AppflyerEventSender.Instance.logAdRevenue(adValue);
            Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        _interstitialAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        _interstitialAd.OnAdClicked += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        _interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        _interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Interstitial ad full screen content closed.");
            InterAdState = AdState.NotAvailable;
            ActionOnAfterInterstitalAd?.Invoke();
        };
        // Raised when the ad failed to open full screen content.
        _interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Interstitial ad failed to open full screen content " +
                           "with error : " + error);
            InterAdState = AdState.NotAvailable;
            ActionOnAfterInterstitalAd?.Invoke();
        };
    }



    [Header("Ad Reward")]
    public bool IsPreloadReward = true;
    public AdState RewardAdState = AdState.NotAvailable;
    public int _rewardReloadCount = 0;
    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    public string _adUnitRewardId = "ca-app-pub-5904408074441373/5867450136";
#elif UNITY_IPHONE
    public string _adUnitRewardId = "ca-app-pub-5904408074441373/4291067011";
#else
    public string _adUnitRewardId = "unused";
#endif

    private RewardedAd _rewardedAd;

    /// <summary>
    /// Loads the rewarded ad.
    /// </summary>
    public void LoadRewardedAd()
    {
        if (RewardAdState == AdState.Loading)
            return;
        RewardAdState = AdState.Loading;

        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log($"[{this.GetType().ToString()}] Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(_adUnitRewardId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            // if error is not null, the load request failed.
            if (error != null || ad == null)
            {
                RewardAdState = AdState.NotAvailable;
                _rewardReloadCount += 1;
                Debug.Log($"[{this.GetType().ToString()}] Rewarded ad failed to load an ad " +
                               "with error : " + error);
                return;
            }

            Debug.Log($"[{this.GetType().ToString()}] Rewarded ad loaded with response : "
                      + ad.GetResponseInfo());

            _rewardedAd = ad;
            ListenToRewardAdEvents();
            RewardAdState = AdState.Ready;
            _rewardReloadCount = 0;
        });
    }

    public void ShowRewardedAd(UnityAction CALLBACK)
    {
        const string rewardMsg = "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            OpenAdSpaceTimeCounter = 0;
            _rewardedAd.Show((Reward reward) =>
            {
                CALLBACK?.Invoke();
                // TODO: Reward the user.
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
            });
        }
    }

    private void ListenToRewardAdEvents()
    {
        // Raised when the ad is estimated to have earned money.
        _rewardedAd.OnAdPaid += (AdValue adValue) =>
        {
            //AppflyerEventSender.Instance.logAdRevenue(adValue);
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        _rewardedAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        _rewardedAd.OnAdClicked += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        _rewardedAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        _rewardedAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Rewarded ad full screen content closed.");
            RewardAdState = AdState.NotAvailable;
        };
        // Raised when the ad failed to open full screen content.
        _rewardedAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Rewarded ad failed to open full screen content " +
                           "with error : " + error);
            RewardAdState = AdState.NotAvailable;
        };
    }

    [Header("Ad Open")]
    public bool IsPreloadOpen = true;
    public AdState OpenAdState = AdState.NotAvailable;
    public int _openReloadCount = 0;
    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    public string _adUnitOpenId = "ca-app-pub-5904408074441373/7978608159";
#elif UNITY_IPHONE
    public string _adUnitOpenId = "ca-app-pub-5904408074441373/2977985343";
#else
    public string _adUnitOpenId = "unused";
#endif
    private AppOpenAd appOpenAd;

    public float OpenAdSpaceTimeCounter = 0.0f;
    public float OpenAdSpaceTime = 15.0f;

    public void CaculaterCounterOpenAd()
    {
        OpenAdSpaceTimeCounter += Time.deltaTime;
    }    

    /// <summary>
    /// Loads the app open ad.
    /// </summary>
    public void LoadAppOpenAd()
    {
        if (RuntimeStorageData.Player.IsAds)
            return;
        if (OpenAdState == AdState.Loading)
            return;
        OpenAdState = AdState.Loading;

        // Clean up the old ad before loading a new one.
        if (appOpenAd != null)
        {
            appOpenAd.Destroy();
            appOpenAd = null;
        }

        Debug.Log($"[{this.GetType().ToString()}] Loading the app open ad.");

        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        AppOpenAd.Load(_adUnitOpenId, adRequest,
            (AppOpenAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    OpenAdState = AdState.NotAvailable;
                    _openReloadCount += 1;
                    Debug.Log($"[{this.GetType().ToString()}] app open ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log($"[{this.GetType().ToString()}] App open ad loaded with response : "
                          + ad.GetResponseInfo());

                appOpenAd = ad;
                ListenToOpenAdEvents();
                OpenAdState = appOpenAd.CanShowAd() == true ? AdState.Ready : AdState.NotAvailable;
                _openReloadCount = 0;
            });
    }

    private void ListenToOpenAdEvents()
    {
        // Raised when the ad is estimated to have earned money.
        appOpenAd.OnAdPaid += (AdValue adValue) =>
        {
            //AppflyerEventSender.Instance.logAdRevenue(adValue);
            Debug.Log(String.Format("App open ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        appOpenAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] App open ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        appOpenAd.OnAdClicked += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] App open ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        appOpenAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] App open ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        appOpenAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] App open ad full screen content closed.");
            OpenAdState = AdState.NotAvailable;
        };
        // Raised when the ad failed to open full screen content.
        appOpenAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.Log($"[{this.GetType().ToString()}] App open ad failed to open full screen content " +
                           "with error : " + error);
            OpenAdState = AdState.NotAvailable;
        };
    }

    //[Button]
    public void CheckingOpenAd()
    {
        if (OpenAdSpaceTimeCounter < OpenAdSpaceTime && Manager.Instance.IsIngame == true)
            return;
        Debug.Log($"[{this.GetType().ToString()}] Checking Open Ad");
        OpenAdSpaceTimeCounter = 0;
        ShowAppOpenAd();
    }

    public bool IsAdAvailable
    {
        get
        {
            return appOpenAd != null && appOpenAd.CanShowAd() == true;
        }
    }

    /// <summary>
    /// Shows the app open ad.
    /// </summary>
    public void ShowAppOpenAd()
    {
        if (IsAdAvailable)
        {
            Debug.Log($"[{this.GetType().ToString()}] Showing app open ad.");
            appOpenAd.Show();
        }
        else
        {
            Debug.Log($"[{this.GetType().ToString()}] App open ad is not ready yet.");
        }
    }
#else

    public void ShowRewardedAd(UnityAction callback)
    {
        Debug.Log($"[{this.GetType().ToString()}] Show Rewarded Ad.");
        callback?.Invoke();
    }

    public void ShowInterstitialAdWithSpaceTime(UnityAction callback)
    {
        Debug.Log($"[{this.GetType().ToString()}] Show Interstitial Ad With Space Time.");
        callback?.Invoke();
    }

    public void ShowInterstitialAd(UnityAction callback)
    {
        Debug.Log($"[{this.GetType().ToString()}] Show Interstitial Ad.");
        callback?.Invoke();
    }

    public void CheckingOpenAd()
    {
        Debug.Log($"[{this.GetType().ToString()}] Checking Open Ad.");
    }
#endif
}
