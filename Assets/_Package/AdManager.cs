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
            PromiseShowInterstitialAd(null, () => { Manager.Instance.CompleteOpenAd(); });
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
        if (UseReward == true)
        {
            LoadRewardedAd();
            LoadRewardedSecondAd();
            LoadRewardedThridAd();
        }    
        if (UseBanner == true)
        {
            LoadBannerAd();
            LoadBannerMERCAd();
        }    
        if (UseInterstitial == true)
        {
            LoadInterstitialAd();
            LoadInterstitialHomeAd();
        }
        if (UseOpen == true)
        {
            LoadAppOpenAd();
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

        TimerBannerAutoReload += Time.deltaTime;
        if(TimerBannerAutoReload >= Manager.Instance.BannerReloadTimer)
        {
            if (BannerAdState == AdState.Ready)
                BannerAdState = AdState.NotAvailable;
            if (BannerMERCAdState == AdState.Ready)
                BannerMERCAdState = AdState.NotAvailable;
        }    

        if (IsPreloadBanner)
        {
            TimerBannerAdReload += Time.deltaTime;
            if (TimerBannerAdReload > AfterAdReload)
            {
                if (BannerAdState == AdState.NotAvailable /*&& _interstitalReloadCount <= 10*/)
                {
                    TimerBannerAdReload = 0;
                    LoadBannerAd();
                }
            }
        }

        if (IsPreloadBannerMERC)
        {
            TimerBannerMERCAdReload += Time.deltaTime;
            if (TimerBannerMERCAdReload > AfterAdReload)
            {
                if (BannerMERCAdState == AdState.NotAvailable /*&& _interstitalReloadCount <= 10*/)
                {
                    TimerBannerMERCAdReload = 0;
                    LoadBannerMERCAd();
                }
            }
        }

        if (IsPreloadInterstitial)
        {
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
            TimerRewardedAdReload += Time.deltaTime;
            if (TimerRewardedAdReload > AfterAdReload)
            {
                TimerRewardedAdReload = 0;
                if (RewardAdState == AdState.NotAvailable /*&& _rewardReloadCount <= 10*/)
                    LoadRewardedAd();
            }
        }

        if (IsPreloadRewardSecond)
        {
            TimerRewardedSecondAdReload += Time.deltaTime;
            if (TimerRewardedSecondAdReload > AfterAdReload)
            {
                TimerRewardedSecondAdReload = 0;
                if (RewardSecondAdState == AdState.NotAvailable /*&& _rewardReloadCount <= 10*/)
                    LoadRewardedSecondAd();
            }
        }

        if (IsPreloadRewardThrid)
        {
            TimerRewardedThirdAdReload += Time.deltaTime;
            if (TimerRewardedThirdAdReload > AfterAdReload)
            {
                TimerRewardedThirdAdReload = 0;
                if (RewardThridAdState == AdState.NotAvailable /*&& _rewardReloadCount <= 10*/)
                    LoadRewardedThridAd();
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
    public bool UseBanner = false;
    public bool IsPreloadBanner = true;
    public AdState BannerAdState = AdState.NotAvailable;
    //public GameObject _bannerOverlay;
    public int _bannerReloadCount = 0;
    //public Transform _backgroundAd;
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

        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        Debug.Log($"[{this.GetType().ToString()}] adaptiveSize " + adaptiveSize);
        // Create a 320x50 banner at top of the screen
        _bannerView = new BannerView(_adUnitBannerId, adaptiveSize, AdPosition.Bottom);
        ListenToBannerAdEvents();
    }

    public void LoadBannerAd()
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
            ShowBannerAd();
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
        if (RuntimeStorageData.Player.IsAds)
            return;
        if (_bannerView != null)
        {
            //_backgroundAd.SetActive(true);
            _bannerView.Show();
        }    
    }

    public void HideBannerAd()
    {
        if (_bannerView != null)
        {
            //_backgroundAd.SetActive(false);
            _bannerView.Hide();
        }    
    }

    [Header("Banner MERC")]
    public bool IsPreloadBannerMERC = true;
    public AdState BannerMERCAdState = AdState.NotAvailable;
    public int _bannerMERCReloadCount = 0;
    public Transform _backgroundMERCAd;

#if UNITY_ANDROID
    public string _adUnitBannerMERCId = "";
#elif UNITY_IPHONE
    public string _adUnitBannerMERCId = "";
#else
    public string _adUnitBannerMERCId = "unused";
#endif

    BannerView _bannerMERCView;

    public void LoadBannerMERCAd()
    {
        if (RuntimeStorageData.Player.IsAds)
            return;
        if (BannerMERCAdState == AdState.Loading)
            return;
        BannerMERCAdState = AdState.Loading;

        // create an instance of a banner view first.
        if (_bannerMERCView == null)
        {
            Debug.Log($"[{this.GetType().ToString()}] Creating banner merc view");

            // If we already have a banner, destroy the old one.
            if (_bannerMERCView != null)
            {
                BannerMERCAdState = AdState.NotAvailable;
                Debug.Log($"[{this.GetType().ToString()}] Destroying banner merc view.");
                _bannerMERCView.Destroy();
                _bannerMERCView = null;
            }
            int w = 0;
            int bannerHeight = 50;
            int h = Screen.height / 2 - bannerHeight / 2;

            //AdSize adaptiveSize = _statusMERC == AdBannerSize.FullWidth ? AdSize.GetLandscapeAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth) : AdSize.Banner;
            AdSize adaptiveSize = AdSize.MediumRectangle;
            Debug.Log($"[{this.GetType().ToString()}] adaptiveSize " + adaptiveSize);
            // Create a 320x50 banner at top of the screen
            _bannerMERCView = new BannerView(_adUnitBannerMERCId, adaptiveSize, AdPosition.Bottom);
            // Raised when an ad is loaded into the banner view.
            _bannerMERCView.OnBannerAdLoaded += () =>
            {
                _bannerMERCReloadCount = 0;
                BannerMERCAdState = AdState.Ready;
                Debug.Log($"[{this.GetType().ToString()}] Banner view loaded an ad with response : "
                    + _bannerMERCView.GetResponseInfo());
                Debug.Log(string.Format("[AdManager] Ad Height: {0}, width: {1}", _bannerMERCView.GetHeightInPixels(), _bannerMERCView.GetWidthInPixels()));
            };
            // Raised when an ad fails to load into the banner view.
            _bannerMERCView.OnBannerAdLoadFailed += (LoadAdError error) =>
            {
                _bannerMERCReloadCount += 1;
                BannerMERCAdState = AdState.NotAvailable;
                Debug.Log($"[{this.GetType().ToString()}] Banner view failed to load an ad with error : "
                    + error);
            };
            // Raised when the ad is estimated to have earned money.
            _bannerMERCView.OnAdPaid += (AdValue adValue) =>
            {
                //AppflyerEventSender.Instance.logAdRevenue(adValue);
                Debug.Log(String.Format("Banner view paid {0} {1}.",
                    adValue.Value,
                    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            _bannerMERCView.OnAdImpressionRecorded += () =>
            {
                Debug.Log($"[{this.GetType().ToString()}] Banner view recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            _bannerMERCView.OnAdClicked += () =>
            {
                Debug.Log($"[{this.GetType().ToString()}] Banner view was clicked.");
            };
            // Raised when an ad opened full screen content.
            _bannerMERCView.OnAdFullScreenContentOpened += () =>
            {
                Debug.Log($"[{this.GetType().ToString()}] Banner view full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            _bannerMERCView.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log($"[{this.GetType().ToString()}] Banner view full screen content closed.");
            };
        }

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        Debug.Log($"[{this.GetType().ToString()}] Loading banner MERC ad.");
        _bannerMERCView.LoadAd(adRequest);
        HideBannerMERCAd();
    }

    public void ShowBannerMERCAd()
    {
        if (_bannerMERCView != null)
        {
            _backgroundMERCAd.SetActive(true);
            _bannerMERCView.Show();
        }    
    }

    public void HideBannerMERCAd()
    {
        if (_bannerMERCView != null)
        {
            _backgroundMERCAd.SetActive(false);
            _bannerMERCView.Hide();
        }    
    }

    public void BtnCloseMERCAd()
    {
        HideBannerMERCAd();
        ShowBannerAd();
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
    //public float InterAdSpaceTimeAuto = 60.0f;
    // These ad units are configured to always serve test ads.
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
    /// Shows the interstitial ad.
    /// </summary>
    /// 
    public void ShowInterstitialAd(UnityAction Callback)
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
                InterAdState = AdState.NotAvailable;
                if (Callback != null)
                    Callback?.Invoke();
                _loadingInterstitalAd?.SetActive(false);
            };
            CoroutineUtils.PlayCoroutine(() =>
            {
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
            ActionOnAfterInterstitalAd?.Invoke();
        };
        // Raised when the ad failed to open full screen content.
        _interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Interstitial ad failed to open full screen content " +
                           "with error : " + error);
            ActionOnAfterInterstitalAd?.Invoke();
        };
    }

    [Header("Ad Home Interstitial")]

    public bool IsPreloadinterstitialHome = true;
    public AdState InterHomeAdState = AdState.NotAvailable;
    public AdShowState InterHomeAdShowState = AdShowState.None;
    public int _interstitialHomeAdReloadCount = 0;

    public float InterHomeAdSpaceTimeAutoCounter = 0;
    //public float InterHomeAdSpaceTimeAuto = 30.0f;

#if UNITY_ANDROID
    public string _adUnitInterHomeId = "ca-app-pub-5904408074441373/8357882100";
#elif UNITY_IPHONE
    public string _adUnitInterHomeId = "ca-app-pub-5904408074441373/5604148681";
#else
    public string _adUnitInterHomeId = "unused";
#endif

    public void LoadInterstitialHomeAd()
    {
        if (RuntimeStorageData.Player.IsAds)
            return;
        if (InterHomeAdState == AdState.Loading)
            return;
        InterHomeAdState = AdState.Loading;

        // Clean up the old ad before loading a new one.
        if (_interstitialHomeAd != null)
        {
            _interstitialHomeAd.Destroy();
            _interstitialHomeAd = null;
        }

        Debug.Log($"[{this.GetType().ToString()}] Loading the interstitial home ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(_adUnitInterHomeId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            // if error is not null, the load request failed.
            if (error != null || ad == null)
            {
                InterHomeAdState = AdState.NotAvailable;
                _interstitialHomeAdReloadCount += 1;
                Debug.Log($"[{this.GetType().ToString()}] interstitial ad failed to load an ad " +
                               "with error : " + error);
                return;
            }

            Debug.Log($"[{this.GetType().ToString()}] Interstitial ad loaded with response : "
                      + ad.GetResponseInfo());

            _interstitialHomeAd = ad;
            InterHomeAdState = AdState.Ready;
            ListenToInterHomeAdEvents();
            _interstitialHomeAdReloadCount = 0;
        });
    }

    public void ShowInterstitialHomeAd(UnityAction Callback)
    {
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
                if (Callback != null)
                    Callback?.Invoke();
            };
            _interstitialHomeAd.Show();
            ResetInterstitialAdCounter();
        }
        else
        {
            Debug.Log($"[{this.GetType().ToString()}] Interstitial home ad is not ready yet.");
            if (Callback != null)
                Callback?.Invoke();
        }
    }

    private void ListenToInterHomeAdEvents()
    {
        // Raised when the ad is estimated to have earned money.
        _interstitialHomeAd.OnAdPaid += (AdValue adValue) =>
        {
            //AppflyerEventSender.Instance.logAdRevenue(adValue);
            Debug.Log(String.Format("Interstitial home ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        _interstitialHomeAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Interstitial home ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        _interstitialHomeAd.OnAdClicked += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Interstitial home ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        _interstitialHomeAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Interstitial home ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        _interstitialHomeAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Interstitial home ad full screen content closed.");
            ActionOnAfterInterstitalAd?.Invoke();
        };
        // Raised when the ad failed to open full screen content.
        _interstitialHomeAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Interstitial home ad failed to open full screen content " +
                           "with error : " + error);
            ActionOnAfterInterstitalAd?.Invoke();
        };
    }

#if UNITY_ANDROID
    [Header("Ad Open Interstitial")]
    public string _adUnitInterOpenId = "ca-app-pub-5904408074441373/7180531807";
#elif UNITY_IPHONE
    [Header("Ad Open Interstitial")]
    public string _adUnitInterOpenId = "ca-app-pub-5904408074441373/4650159660";
#else
    public string _adUnitInterId = "unused";
#endif

    public float InterOpenAdMaximusTimeCounter = 0.0f;
    public float InterOpenAdMaximusTime = 15.0f;

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
            ActionOnAfterInterstitalAd = () => { afterAd?.Invoke(); };

            if (InterOpenAdMaximusTimeCounter <= InterOpenAdMaximusTime)
            {
                InterOpenAdMaximusTimeCounter = 999;
                _interstitialOpenAd.Show();
            }
            else
            {
                Debug.Log("Out of time show open ad");
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
                ShowInterstitialAd(null);
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

    public void ShowRewardedAd(UnityAction Callback)
    {
        const string rewardMsg = "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            OpenAdSpaceTimeCounter = 0;
            _rewardedAd.Show((Reward reward) =>
            {
                Callback?.Invoke();
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

        // Clean up the old ad before loading a new one.
        if (_rewardedSecondAd != null)
        {
            _rewardedSecondAd.Destroy();
            _rewardedSecondAd = null;
        }

        Debug.Log($"[{this.GetType().ToString()}] Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(_adUnitRewardSecondId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            // if error is not null, the load request failed.
            if (error != null || ad == null)
            {
                RewardSecondAdState = AdState.NotAvailable;
                _rewardSecondLoadCount += 1;
                Debug.Log($"[{this.GetType().ToString()}] Rewarded ad failed to load an ad " +
                               "with error : " + error);
                return;
            }

            Debug.Log($"[{this.GetType().ToString()}] Rewarded ad loaded with response : "
                      + ad.GetResponseInfo());

            _rewardedSecondAd = ad;
            ListenToRewardSecondAdEvents();
            RewardSecondAdState = AdState.Ready;
            _rewardSecondLoadCount = 0;
        });
    }

    public void ShowRewardedSecondAd(UnityAction Callback)
    {
        const string rewardMsg = "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (_rewardedSecondAd != null && _rewardedSecondAd.CanShowAd())
        {
            OpenAdSpaceTimeCounter = 0;
            _rewardedSecondAd.Show((Reward reward) =>
            {
                Callback?.Invoke();
                // TODO: Reward the user.
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
            });
        }
    }

    private void ListenToRewardSecondAdEvents()
    {
        // Raised when the ad is estimated to have earned money.
        _rewardedSecondAd.OnAdPaid += (AdValue adValue) =>
        {
            //AppflyerEventSender.Instance.logAdRevenue(adValue);
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        _rewardedSecondAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        _rewardedSecondAd.OnAdClicked += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        _rewardedSecondAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        _rewardedSecondAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Rewarded ad full screen content closed.");
            RewardSecondAdState = AdState.NotAvailable;
        };
        // Raised when the ad failed to open full screen content.
        _rewardedSecondAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Rewarded ad failed to open full screen content " +
                           "with error : " + error);
            RewardSecondAdState = AdState.NotAvailable;
        };
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

        // Clean up the old ad before loading a new one.
        if (_rewardedThridAd != null)
        {
            _rewardedThridAd.Destroy();
            _rewardedThridAd = null;
        }

        Debug.Log($"[{this.GetType().ToString()}] Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(_adUnitRewardThriddId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            // if error is not null, the load request failed.
            if (error != null || ad == null)
            {
                RewardThridAdState = AdState.NotAvailable;
                _rewardThridLoadCount += 1;
                Debug.Log($"[{this.GetType().ToString()}] Rewarded ad failed to load an ad " +
                               "with error : " + error);
                return;
            }

            Debug.Log($"[{this.GetType().ToString()}] Rewarded ad loaded with response : "
                      + ad.GetResponseInfo());

            _rewardedThridAd = ad;
            ListenToRewardThridAdEvents();
            RewardThridAdState = AdState.Ready;
            _rewardThridLoadCount = 0;
        });
    }

    public void ShowRewardedThridAd(UnityAction Callback)
    {
        const string rewardMsg = "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (_rewardedThridAd != null && _rewardedThridAd.CanShowAd())
        {
            OpenAdSpaceTimeCounter = 0;
            _rewardedThridAd.Show((Reward reward) =>
            {
                Callback?.Invoke();
                // TODO: Reward the user.
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
            });
        }
    }

    private void ListenToRewardThridAdEvents()
    {
        // Raised when the ad is estimated to have earned money.
        _rewardedThridAd.OnAdPaid += (AdValue adValue) =>
        {
            //AppflyerEventSender.Instance.logAdRevenue(adValue);
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        _rewardedThridAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        _rewardedThridAd.OnAdClicked += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        _rewardedThridAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        _rewardedThridAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Rewarded ad full screen content closed.");
            RewardThridAdState = AdState.NotAvailable;
        };
        // Raised when the ad failed to open full screen content.
        _rewardedThridAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.Log($"[{this.GetType().ToString()}] Rewarded ad failed to open full screen content " +
                           "with error : " + error);
            RewardThridAdState = AdState.NotAvailable;
        };
    }

    [Header("Ad Open")]
    public bool UseOpen = true;
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

    private void Start()
    {
        Manager.Instance.ShowLoading();
    }

    public void ShowRewardedAd(UnityAction Callback)
    {
        Debug.Log($"[{this.GetType().ToString()}] Show Rewarded Ad.");
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

    public void CheckingOpenAd()
    {
        Debug.Log($"[{this.GetType().ToString()}] Checking Open Ad.");
    }
#endif
        }
