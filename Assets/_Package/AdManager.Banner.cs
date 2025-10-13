using UnityEngine;
#if ADMOB
using GoogleMobileAds.Api;
#endif

public partial class AdManager
{
#if ADMOB
    [Header("AD BANNER")]
    public bool IsPreloadBanner = true;
    public AdState BannerAdState = AdState.NotAvailable;
    public int _bannerReloadCount = 0;
    public string _adUnitBannerId = "";
    private BannerView bannerAd;

    private bool IsBanner = false;
    private float TimeBanner = 0;

    private void CaculaterCounterBannerAd()
    {
        // Chỉnh về 15s tự động load lại banner
        if (Manager.Instance.IsLoading == true) return;
        if (RuntimeStorageData.Player.IsLoadAds == false) return;
        if (Time.time - TimeBanner > 15.0f)
        {
            TimeBanner = Time.time;
            BannerAdState = AdState.NotAvailable;
        }
    }

    private void LoadBannerAd()
    {
        if (RuntimeStorageData.Player.IsLoadAds == false) return;
        if (BannerAdState == AdState.Loading) return;
        BannerAdState = AdState.Loading;

        if (bannerAd != null) { bannerAd.Destroy(); bannerAd = null; }

        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        bannerAd = new BannerView(_adUnitBannerId, adaptiveSize, AdPosition.Bottom);
        bannerAd.LoadAd(new AdRequest());

        if (IsBanner) ShowBanner(); else HideBanner();

        bannerAd.OnBannerAdLoaded += () => { BannerAdState = AdState.Ready; };
        bannerAd.OnBannerAdLoadFailed += (LoadAdError error) => { BannerAdState = AdState.NotAvailable; };
        bannerAd.OnAdClicked += () => { BannerAdState = AdState.NotAvailable; };
        bannerAd.OnAdPaid += (revenue) => { AppflyerEventSender.Instance.logAdRevenue(revenue); };
    }

    public void ShowBanner() { IsBanner = true; bannerAd?.Show(); }
    public void HideBanner() { IsBanner = false; bannerAd?.Hide(); }
#endif
}
