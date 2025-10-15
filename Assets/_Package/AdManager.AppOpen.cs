using UnityEngine;
#if ADMOB
using DG.Tweening;
using GoogleMobileAds.Api;
using PimDeWitte.UnityMainThreadDispatcher;
#endif

public partial class AdManager
{
#if ADMOB
    [Header("AD OPEN")]
    public GameObject LoadingOpenAdCanvas;
    public bool IsPreloadOpen = true;
    public AdState OpenAdState = AdState.NotAvailable;
    public int _openReloadCount = 0;
    public string _adUnitOpenId = "ca-app-pub-5904408074441373/7523012234";
    private AppOpenAd appOpenAd;

    public float OpenAdSpaceTimeCounter = 0.0f;
    public float OpenAdSpaceTime = 2.0f;

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
        Debug.Log($"[{GetType()}] Loading App Open Ad.");

        if (appOpenAd != null) { appOpenAd.Destroy(); appOpenAd = null; }

        var adRequest = new AdRequest();
        AppOpenAd.Load(_adUnitOpenId, adRequest, (AppOpenAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                OpenAdState = AdState.NotAvailable;
                _openReloadCount += 1;
                Debug.LogError($"[{GetType()}] Failed to load the app open ad: {error.GetMessage()}");
                return;
            }

            appOpenAd = ad;
            appOpenAd.OnAdPaid += (revenue) => { AppflyerEventSender.Instance.logAdRevenue(revenue); };

            appOpenAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log($"[{GetType()}] App open ad closed.");
                UnityMainThreadDispatcher.Instance().Enqueue(() => LoadingOpenAdCanvas.Hide());
                OpenAdState = AdState.NotAvailable;
            };

            appOpenAd.OnAdFullScreenContentFailed += (AdError e) =>
            {
                Debug.LogError($"[{GetType()}] App open ad failed to show: {e.GetMessage()}");
                UnityMainThreadDispatcher.Instance().Enqueue(() => LoadingOpenAdCanvas.Hide());
                OpenAdState = AdState.NotAvailable;
            };

            OpenAdState = appOpenAd.CanShowAd() ? AdState.Ready : AdState.NotAvailable;
            _openReloadCount = 0;
        });
    }

    public void CheckingOpenAd()
    {
        if (OpenAdSpaceTimeCounter < OpenAdSpaceTime) return;
        Debug.Log($"[{GetType()}] Checking Open Ad");
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            if (RuntimeStorageData.Player.IsLoadAds == false) return;
            ResetOpenAdSpaceTime();
            ShowAppOpenAd();
        });
    }

    public void ResetOpenAdSpaceTime() { OpenAdSpaceTimeCounter = 0; }

    private bool IsAdAvailable => appOpenAd != null && appOpenAd.CanShowAd();

    private void ShowAppOpenAd()
    {
        Debug.Log($"[{GetType()}] Try to show app open ad.");
        if (IsAdAvailable)
        {
            Debug.Log($"[{GetType()}] Showing app open ad.");
            UnityMainThreadDispatcher.Instance().Enqueue(() => LoadingOpenAdCanvas.Show());
            DOVirtual.DelayedCall(0.1f, () => appOpenAd.Show());
        }
    }
#endif
}
