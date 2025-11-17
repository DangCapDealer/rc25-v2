using GoogleMobileAds.Api;
using System;
using JKit.Monetize.Ads;
using UnityEngine;

public partial class AdsController : MonoBehaviour
{
    [Header("NativeInter")] public string AdNativeUnitId = "ca-app-pub-3940256099942544/2247696110";
    // public StateAds NativeAdState = StateAds.Loading;
    // public StateAds NativeAdState_2 = StateAds.Loading;

    // public NativeAd nativeAd;
    // public NativeAd nativeAd_2;
    // public bool isLoaded = false;
    // public bool isLoaded_2 = false;

    public enum StateAds
    {
        NotAvailable,
        Loading,
        Ready,
        Showing,
    }

    private NativeOverlay _nativeOverlayFullScreen;
    private StateAds _nativeFullScreenState = StateAds.NotAvailable;

    private void ShowNativeFullScreen()
    {
        if (_nativeFullScreenState == StateAds.Ready)
        {
            _nativeOverlayFullScreen.Show();
        }
    }

    private void ReloadNativeFullScreen()
    {
        if (_nativeFullScreenState == StateAds.NotAvailable)
        {
            _nativeFullScreenState = StateAds.Loading;
            NativeOverlay.Load(AdNativeUnitId, 30, (overlay, error) =>
            {
                if (error != null)
                {
                    _nativeFullScreenState = StateAds.NotAvailable;
                }
                else
                {
                    _nativeFullScreenState = StateAds.Ready;
                    _nativeOverlayFullScreen = overlay;
                    _nativeOverlayFullScreen.OnClosed += OnClosed;
                }
            });
        }
    }

    private void OnClosed()
    {
        if (_nativeOverlayFullScreen != null)
        {
            _nativeOverlayFullScreen.Destroy();
            _nativeOverlayFullScreen = null;
        }

        _nativeFullScreenState = StateAds.NotAvailable;
    }

    // private void UnregisterNativeHandlers(NativeAd ad)
    // {
    //     if (ad == null) return;
    //     ad.OnPaidEvent -= OnPaidEvent; // chỉ cần cái này
    // }

    // private void ReloadNativeInter()
    // {
    //     if (NativeAdState == StateAds.NotAvailable)
    //         RequestNativeAd();
    //     if (NativeAdState_2 == StateAds.NotAvailable)
    //         RequestNativeAd2();
    // }

    // public void RequestNativeAd()
    // {
    //     Debug.Log($"[{this.GetType().ToString()}] Load banner native Ad");
    //     if (NativeAdState == StateAds.Loading)
    //         return;
    //     NativeAdState = StateAds.Loading;
    //     if (this.nativeAd != null)
    //     {
    //         UnregisterNativeHandlers(nativeAd);
    //         this.nativeAd.Destroy();
    //         this.nativeAd = null;
    //     }
    //     AdLoader adLoader = new AdLoader.Builder(AdNativeUnitId)
    //         .ForNativeAd()
    //         .Build();
    //     adLoader.OnNativeAdClicked += OnNativeAdClicked;
    //     adLoader.OnNativeAdLoaded += this.HandleNativeAdLoaded;
    //     adLoader.OnAdFailedToLoad += HandleAdFailedToLoad;
    //     adLoader.OnNativeAdImpression += OnNativeAdImpression;
    //     adLoader.LoadAd(new AdRequest());
    // }


    // private void OnNativeAdImpression(object sender, EventArgs e)
    // {
    //     Debug.Log($"[{this.GetType().ToString()}] On Native Ad Impression");
    // }
    //
    // private void OnNativeAdClicked(object sender, EventArgs e)
    // {
    //     if (nativeAd != null)
    //     {
    //         NativeAdState = StateAds.NotAvailable;
    //         isLoaded = true;
    //     }
    //     Debug.Log($"{this.GetType().ToString()} Native ad Clicked.");
    // }
    // private void HandleNativeAdLoaded(object sender, NativeAdEventArgs args)
    // {
    //     Debug.Log($"[{this.GetType().ToString()}] Native ad loaded.");
    //     NativeAdState = StateAds.Ready;
    //     isLoaded = true;
    //     this.nativeAd = args.nativeAd;
    //     this.nativeAd.OnPaidEvent += OnPaidEvent;
    // }

    // private void OnPaidEvent(object sender, AdValueEventArgs e)
    // {
    //     AppflyerEventSender.Instance.logAdRevenue(e.AdValue);
    //     Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
    //         e.AdValue.Value,
    //         e.AdValue.CurrencyCode));
    // }
    //
    // private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    // {
    //     Debug.Log($"[{this.GetType().ToString()}] Native ad failed to load: " + e.ToString());
    //     NativeAdState = StateAds.NotAvailable;
    // }
    // public void RequestNativeAd2()
    // {
    //     Debug.Log($"{this.GetType().ToString()} Load banner native Ad ID 2");
    //     if (NativeAdState_2 == StateAds.Loading)
    //         return;
    //     NativeAdState_2 = StateAds.Loading;
    //     if (this.nativeAd_2 != null)
    //     {
    //         UnregisterNativeHandlers(nativeAd_2);
    //         this.nativeAd_2.Destroy();
    //         this.nativeAd_2 = null;
    //     }
    //     AdLoader adLoader = new AdLoader.Builder(AdNativeUnitId)
    //         .ForNativeAd()
    //         .Build();
    //     adLoader.OnNativeAdClicked += OnNativeAdClicked2;
    //     adLoader.OnNativeAdLoaded += this.HandleNativeAdLoaded2;
    //     adLoader.OnAdFailedToLoad += HandleAdFailedToLoad2;
    //     adLoader.OnNativeAdImpression += OnNativeAdImpression2;
    //     adLoader.LoadAd(new AdRequest());
    // }

    // private void OnNativeAdImpression2(object sender, EventArgs e)
    // {
    //     Debug.Log($"{this.GetType().ToString()} On Native Ad Impression");
    // }
    //
    // private void OnNativeAdClicked2(object sender, EventArgs e)
    // {
    //     if (nativeAd_2 != null)
    //     {
    //         NativeAdState_2 = StateAds.NotAvailable;
    //         isLoaded_2 = true;
    //     }
    //     Debug.Log($"{this.GetType().ToString()} Native ad Clicked ID 2.");
    // }
    // private void HandleNativeAdLoaded2(object sender, NativeAdEventArgs args)
    // {
    //     Debug.Log($"{this.GetType().ToString()} Native ad loaded ID 2.");
    //     NativeAdState_2 = StateAds.Ready;
    //     isLoaded_2 = true;
    //     this.nativeAd_2 = args.nativeAd;
    //     this.nativeAd_2.OnPaidEvent += OnPaidEvent2;
    // }
    //
    // private void OnPaidEvent2(object sender, AdValueEventArgs e)
    // {
    //     AppflyerEventSender.Instance.logAdRevenue(e.AdValue);
    //     Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
    //         e.AdValue.Value,
    //         e.AdValue.CurrencyCode));
    // }
    //
    // private void HandleAdFailedToLoad2(object sender, AdFailedToLoadEventArgs e)
    // {
    //     Debug.Log($"{this.GetType().ToString()} Native ad failed to load: " + e.ToString());
    //     NativeAdState_2 = StateAds.NotAvailable;
    // }
}