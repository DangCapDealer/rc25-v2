#if ADMOB
using DG.Tweening;
// using GoogleMobileAds.Api;
using PimDeWitte.UnityMainThreadDispatcher;
using JKit.Monetize.Ads;

#endif
using UnityEngine;
using UnityEngine.UI;

public partial class AdManager
{
#if ADMOB
    [Header("NATIVE OVERLAY AD")]
    public bool IsPreloadNativeOverlayAd = true;
    public AdState NativeOverlayAdState = AdState.NotAvailable;
    public int NativerOverlayAdReloadCount = 0;
    public int NativeOverlayShowCount = 0;

    // public bool NativerOverlayAdUsed = false;
    private bool NativeOverlayShowing = false;

    public float CollapseAdSpaceTimeCounter = 0.0f;
    public float CollapseAdSpaceTime = 15.0f;
    public string _adUnitNativerOverlayId = "ca-app-pub-3940256099942544/2247696110";

    private NativeOverlay _nativeOverlayAd;

    private void CaculaterCounterCollapseAd()
    {
        if (RuntimeStorageData.Player.IsLoadAds == false) return;
        if (Manager.Instance.IsLoading == true) return;
        if (NativeOverlayShowing == false) return;

        CollapseAdSpaceTimeCounter += Time.deltaTime;
        if (CollapseAdSpaceTimeCounter >= 30.0f)
        {
            CollapseAdSpaceTimeCounter = 0;
            LoadNativeOverlayAd();
        }
    }

    public void LoadNativeOverlayAd()
    {
        if (RuntimeStorageData.Player.IsLoadAds == false) return;
        if (NativeOverlayAdState == AdState.Loading) return;
        NativeOverlayAdState = AdState.Loading;

        if (_nativeOverlayAd != null)
        {
            Debug.Log($"[{this.GetType().ToString()}] Destroying existing Native Overlay ad before loading a new one.");
            _nativeOverlayAd.Destroy();
            _nativeOverlayAd = null;
        }

        NativeOverlay.Load(_adUnitNativerOverlayId, 30, (overlay, error) =>
        {
            if (error != null)
            {
                NativeOverlayAdState = AdState.NotAvailable;
            }
            else
            {
                NativeOverlayAdState = AdState.Ready;
                // NativerOverlayAdUsed = false;
                _nativeOverlayAd = overlay;
                _nativeOverlayAd.OnClosed += onAdClosed;
                _nativeOverlayAd.OnClicked += onAdClicked;
            }
        });
    }

    private void onAdClicked()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => LoadNativeOverlayAd());
    }

    private void onAdClosed()
    {
        ShowBanner();
        IsBlockedAutoIntertitialAd = false;
        NativeOverlayAdState = AdState.NotAvailable;
        NativeOverlayShowing = false;
    }

    public void ShowNativeOverlayAd()
    {
        if (RuntimeStorageData.Player.IsLoadAds == false) return;
        if (_nativeOverlayAd != null)
        {
            Debug.Log($"[{this.GetType().ToString()}] Showing Native Overlay ad.");
            NativeOverlayShowCount += 1;
            IsBlockedAutoIntertitialAd = true;
            DOVirtual.DelayedCall(0.1f, _nativeOverlayAd.Show);
            HideBanner();
            // NativerOverlayAdUsed = true;
            NativeOverlayShowing = true;
        }
    }

#else
    public void ShowAdNativeOverlay() { }
#endif
}
