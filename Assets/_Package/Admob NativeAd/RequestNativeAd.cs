#if ADMOB
using GoogleMobileAds.Api;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestNativeAd : MonoBehaviour
{
#if ADMOB
    [Header("Native Ad Item")]
    public NativeAdPosition Position;
    public string AdNativeUnitId = "ca-app-pub-5904408074441373/4554368467";
    public AdManager.AdState NativeAdState = AdManager.AdState.NotAvailable;

    public bool IsUsed = true;
    public bool IsReloadNativeAd = true;
    public bool nativeAdLoaded = false;

    public NativeAd nativeAd;
    public float TimeAfterReload = 30.0f;
    private float CaculateTime = 0.0f;

    public event Action OnClickedNativeAd;
    public event Action OnChangeNativeAd;

    private void Update()
    {
        if (AdManager.Instance.IsInitalized == false) return;
        if (RuntimeStorageData.Player.IsLoadAds == false) return;
        if (Position == NativeAdPosition.Banner && Manager.Instance.IsNativeBanner == false) return;
        if (Position == NativeAdPosition.BannerCollapse && Manager.Instance.IsNativeMREC == false) return;
        if (Position == NativeAdPosition.Interstitial && Manager.Instance.IsNativeInter == false) return;
        if (NativeAdState == AdManager.AdState.NotAvailable) RequestAd();
        else if (NativeAdState == AdManager.AdState.Ready)
        {
            if (IsUsed == false) return;
            if (IsReloadNativeAd == false) return;
            CaculateTime += Time.deltaTime;
            if (CaculateTime > TimeAfterReload)
            {
                CaculateTime = 0.0f;
                NativeAdState = AdManager.AdState.NotAvailable;
            }
        }
    }

    private void RequestAd()
    {
        Debug.Log($"[{this.GetType().ToString()}] Load native Ad");

        if (NativeAdState == AdManager.AdState.Loading) return;
        NativeAdState = AdManager.AdState.Loading;

        AdLoader adLoader = new AdLoader.Builder(AdNativeUnitId)
            .ForNativeAd()
            .Build();
        adLoader.OnNativeAdLoaded += this.HandleNativeAdLoaded;
        adLoader.OnAdFailedToLoad += HandleAdFailedToLoad;
        adLoader.OnNativeAdClicked += OnNativeAdClicked;
        adLoader.OnNativeAdImpression += OnNativeAdImpression;
        adLoader.LoadAd(new AdRequest());
    }

    private void OnNativeAdImpression(object sender, EventArgs e)
    {
        Debug.Log($"[{this.GetType().ToString()}] On Native Ad Impression");
    }

    private void OnNativeAdClicked(object sender, EventArgs e)
    {
        if (NativeAdState == AdManager.AdState.Ready)
        {
            CaculateTime = 0.0f;
            NativeAdState = AdManager.AdState.NotAvailable;

            OnClickedNativeAd?.Invoke();
            Debug.Log($"[{this.GetType().ToString()}] Native ad Clicked.");
        }
    }

    private void HandleNativeAdLoaded(object sender, NativeAdEventArgs args)
    {
        Debug.Log($"[{this.GetType().ToString()}] Native ad loaded.");
        NativeAdState = AdManager.AdState.Ready;
        this.nativeAd = args.nativeAd;
        this.nativeAd.OnPaidEvent += OnPaidEvent;
        this.nativeAdLoaded = true;
        this.IsUsed = false;

        OnChangeNativeAd?.Invoke();
    }

    private void OnPaidEvent(object sender, AdValueEventArgs e)
    {
        Debug.Log(String.Format($"[{this.GetType().ToString()}] Native ad paid {0} {1}.",
            e.AdValue.Value,
            e.AdValue.CurrencyCode));
    }

    private void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        Debug.Log($"[{this.GetType().ToString()}] Native ad failed to load: " + e.ToString());
        NativeAdState = AdManager.AdState.NotAvailable;
    }
#else
    [Header("Native Ad Item")]
    public NativeAdPosition Position;
#endif
}
