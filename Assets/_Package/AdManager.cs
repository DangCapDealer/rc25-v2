using System.Collections;
using UnityEngine;
using UnityEngine.Events;
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


    [Header("ADMOB SETTING")]
    public bool IsInitalized = false;
    private bool IsCanUpdate = false;
    public bool IsBlockedAutoIntertitialAd = false;

    private void Start() => StartCoroutine(Bacon.UMP.Instance.DOGatherConsent(LoadAds()));

    private IEnumerator LoadAds()
    {
        IsCanUpdate = false;
        yield return new WaitUntil(() => Bacon.UMP.Instance.IsUMPReady);
        Debug.Log("[AdManager] UMP is ready");
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.SetiOSAppPauseOnBackground(true);
        MobileAds.Initialize(initStatus =>
        {
            if (initStatus == null)
            {
                Debug.LogError($"[{GetType()}] Admob Initialize Failed");
                Manager.Instance.IsAds = false;
                IsInitalized = false;
                onComplete();
                return;
            }
            Debug.Log($"[{GetType()}] Admob Initialized");
            Manager.Instance.IsAds = true;
            IsInitalized = true;
            PromiseShowInterstitialAd(OnDispatcher);

            void OnDispatcher() => UnityMainThreadDispatcher.Instance().Enqueue(onComplete); 
            void onComplete() => Manager.Instance.CompleteOpenAd();
        });

        yield return new WaitUntil(() => IsInitalized);

        if (RuntimeStorageData.Player.IsLoadAds == true) LoadAppOpenAd();
#if !UNITY_EDITOR
        yield return WaitForSecondCache.WAIT_TIME_FIVE;
#endif
        // Reward
        LoadRewardedAd();
        LoadRewardedThridAd();

        if (RuntimeStorageData.Player.IsLoadAds == true)
        {
            LoadBannerAd();
            LoadNativeOverlayAd();
            LoadInterstitialAd();
            LoadInterstitialHomeAd();
        }

        IsCanUpdate = true;
    }

    private float timerReload = 10.0f;
    private void Update()
    {
        if (!IsInitalized || !IsCanUpdate) return;

        // Preload reward(s)
        if (IsPreloadReward && RewardAdState == AdState.NotAvailable) LoadRewardedAd();
        if (IsPreloadRewardThrid && RewardThridAdState == AdState.NotAvailable) LoadRewardedThridAd();

        // Tick counters
        CaculaterCounterInterAd();
        CaculaterCounterInterOpenAd();
        CaculaterCounterOpenAd();
        CaculaterCounterBannerAd();
        CaculaterCounterCollapseAd();

        if (RuntimeStorageData.Player.IsLoadAds == false) return;

        if (IsPreloadBanner && BannerAdState == AdState.NotAvailable) LoadBannerAd();
        
        timerReload += Time.deltaTime;
        if (timerReload < 5.0f) return;

        if (IsPreloadinterstitialHome && InterHomeAdState == AdState.NotAvailable) LoadInterstitialHomeAd();
        if (IsPreloadInterstitial && InterAdState == AdState.NotAvailable) LoadInterstitialAd();
        if (IsPreloadOpen && OpenAdState == AdState.NotAvailable) LoadAppOpenAd();
        if (IsPreloadNativeOverlayAd && NativeOverlayAdState == AdState.NotAvailable) LoadNativeOverlayAd();

        timerReload = 0;
    }

    private void ResetInterstitialAdCounter()
    {
        InterAdSpaceTimeAutoCounter = 0;
        InterHomeAdSpaceTimeAutoCounter = 0;
    }
#else
    [HideInInspector] public float InterAdSpaceTimeAutoCounter = 0;
    [HideInInspector] public float OpenAdSpaceTimeCounter = 0;

    private void Start() { Manager.Instance.CompleteOpenAd(); }

    public void ShowRewardedAd(UnityAction cb) { cb?.Invoke(); }
    public void ShowRewardedSecondAd(UnityAction cb) { cb?.Invoke(); }
    public void ShowRewardedThridAd(UnityAction cb) { cb?.Invoke(); }
    public void ShowInterstitialAdWithSpaceTime(UnityAction cb) { cb?.Invoke(); }
    public void ShowInterstitialAd(UnityAction cb) { cb?.Invoke(); }
    public void ShowInterstitialHomeAd(UnityAction cb, UnityAction after) { cb?.Invoke(); after?.Invoke(); }
    public void CheckingOpenAd() { Debug.Log($"[{GetType()}] Checking Open Ad."); }
    public void ShowNativeOverlayAd() { }
    public void ShowBanner() { }
    public void HideBanner() { }
    public void ResetOpenAdSpaceTime() { }
#endif
}
    