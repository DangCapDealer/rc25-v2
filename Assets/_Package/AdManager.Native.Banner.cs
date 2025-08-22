#if ADMOB
using DG.Tweening;
using GoogleMobileAds.Api;
using PimDeWitte.UnityMainThreadDispatcher;

#endif
using UnityEngine;
using UnityEngine.UI;

public partial class AdManager : MonoSingletonGlobal<AdManager>
{
#if ADMOB
    [Header("Nativer Overlay Banner Ad")]
    public bool IsPreloadNativeOverlayBannerAd = true;
    public AdState NativeOverlayBannerAdState = AdState.NotAvailable;
    public int NativerOverlayBannerAdReloadCount = 0;
    public int NativeOverlayBannerShowCount = 0;

    private bool IsChangeBanner = false;
    private bool IsBannerImpression = false;

    //public float CollapseAdSpaceTimeCounter = 0.0f;
    //public float CollapseAdSpaceTime = 15.0f;
    public string _adUnitNativerOverlayBannerId = "ca-app-pub-3940256099942544/2247696110";

    private NativeOverlayAd _nativeOverlayBannerAd;

    private void CaculaterCounterCollapseBannerAd()
    {
        //if (RuntimeStorageData.Player.IsLoadAds == false) return;
        //if (Manager.Instance.IsLoading == true) return;

        //if (IsChangeBanner == false) return;
        //if (IsBannerShow == true)
        //{
        //    IsChangeBanner = false;

        //    RenderNativeOverlayBannerAd();
        //    _nativeOverlayBannerAd.Show();
        //}    


    }

    /// <summary>
    /// Loads the ad.
    /// </summary>
    public void LoadNativeOverlayBannerAd()
    {
        if (RuntimeStorageData.Player.IsLoadAds == false) return;
        if (NativeOverlayBannerAdState == AdState.Loading) return;
        NativeOverlayBannerAdState = AdState.Loading;

        // Clean up the old ad before loading a new one.
        if (_nativeOverlayBannerAd != null)
        {
            DestroyNativeOverlayBannerAd();
        }

        Debug.Log($"[{this.GetType().ToString()}] Loading native overlay banner ad.");

        // Create a request used to load the ad.
        var adRequest = new AdRequest();

        // Optional: Define native ad options.
        var options = new NativeAdOptions();
        options.AdChoicesPlacement = AdChoicesPlacement.BottomRightCorner;
        options.MediaAspectRatio = MediaAspectRatio.Any;

        // Send the request to load the ad.
        NativeOverlayAd.Load(_adUnitNativerOverlayBannerId, adRequest, options,
            (NativeOverlayAd ad, LoadAdError error) =>
            {
                if (error != null)
                {
                    Debug.LogError($"[{this.GetType().ToString()}] Native Overlay Banner ad failed to load an ad " +
                               " with error: " + error);

                    NativeOverlayBannerAdState = AdState.NotAvailable;
                    return;
                }

                // The ad should always be non-null if the error is null, but
                // double-check to avoid a crash.
                if (ad == null)
                {
                    Debug.LogError($"[{this.GetType().ToString()}] Unexpected error: Native Overlay Banner ad load event " +
                               " fired with null ad and null error.");

                    NativeOverlayBannerAdState = AdState.NotAvailable;
                    return;
                }

                // The operation completed successfully.
                Debug.Log($"[{this.GetType().ToString()}] Native Overlay Banner ad loaded with response : " +
                       ad.GetResponseInfo());
                _nativeOverlayBannerAd = ad;
                _nativeOverlayBannerAd.OnAdClicked += onBannerAdClicked;
                _nativeOverlayBannerAd.OnAdImpressionRecorded += onBannerImpression;

                NativeOverlayBannerAdState = AdState.Ready;
                IsChangeBanner = true;
                IsBannerImpression = false;

                Debug.Log(_nativeOverlayBannerAd?.GetResponseInfo());
            });
    }

    private void onBannerAdClicked()
    {
        //UnityMainThreadDispatcher.Instance().Enqueue(() => HideNativeOverlayBannerAd());
        //LoadNativeOverlayBannerAd();
    }

    private void onBannerImpression()
    {
        IsBannerImpression = true;
    }    

    //private NativeTemplateStyle overlayBannerStyle;

    /// <summary>
    /// Shows the ad.
    /// </summary>
    public void ShowNativeOverlayBannerAd()
    {
        if (RuntimeStorageData.Player.IsLoadAds == false) return;
        IsBannerShow = true;
        if (_nativeOverlayBannerAd != null)
        {
            Debug.Log($"[{this.GetType().ToString()}] Showing Native Overlay Banner ad.");

            NativeOverlayBannerShowCount += 1;

            //var NativeOverlayAdCanvas = this.transform.FindChildByParent("NativeOverlayAdCanvas");
            //NativeOverlayAdCanvas.Show();

            Debug.Log("Rendering Native Overlay ad.");

            // Define a native template style with a custom style.
            var overlayBannerStyle = new NativeTemplateStyle();
            overlayBannerStyle.TemplateId = NativeTemplateId.Medium;
            overlayBannerStyle.MainBackgroundColor = MainBackgroundColor;
            overlayBannerStyle.CallToActionText = new NativeTemplateTextStyle();
            overlayBannerStyle.CallToActionText.BackgroundColor = CallToActionTextBackgroundColor;
            overlayBannerStyle.CallToActionText.TextColor = Color.white;
            overlayBannerStyle.CallToActionText.FontSize = 9;
            overlayBannerStyle.CallToActionText.Style = NativeTemplateFontStyle.Bold;


            //int screenWidth = Screen.width;
            //int screenHeight = (int)(Screen.height * 0.15f);
            //var adSize = new AdSize(screenWidth, screenHeight);

            // Renders a native overlay ad at the default size
            // and anchored to the bottom of the screne.
            //_nativeOverlayAd.RenderTemplate(overlayBannerStyle, adSize, AdPosition.Bottom);
            _nativeOverlayBannerAd.RenderTemplate(overlayBannerStyle, AdPosition.Bottom);
            _nativeOverlayBannerAd.Show();

            //DOVirtual.DelayedCall(3.0f, () =>
            //{
            //    if (IsBannerImpression == false)
            //    {
            //        DestroyNativeOverlayBannerAd();
            //        DOVirtual.DelayedCall(2.0f, ShowNativeOverlayBannerAd);
            //        //ShowNativeOverlayBannerAd();
            //    }
            //});
        }
    }

    /// <summary>
    /// Hides the ad.
    /// </summary>
    public void HideNativeOverlayBannerAd()
    {
        IsBannerShow = false;
        Debug.Log("Hide Native Overlay Ad");
        if (_nativeOverlayBannerAd != null)
        {
            Debug.Log($"[{this.GetType().ToString()}] Hiding Native Overlay Banner ad.");

            //var NativeOverlayAdCanvas = this.transform.FindChildByParent("NativeOverlayAdCanvas");
            //NativeOverlayAdCanvas.Hide();

            _nativeOverlayBannerAd.Hide();
        }
    }

    /// <summary>
    /// Destroys the native overlay ad.
    /// </summary>
    public void DestroyNativeOverlayBannerAd()
    {
        if (_nativeOverlayBannerAd != null)
        {
            Debug.Log($"[{this.GetType().ToString()}] Destroying native overlay Banner ad.");
            _nativeOverlayBannerAd.Destroy();
            _nativeOverlayBannerAd = null;

            NativeOverlayBannerAdState = AdState.NotAvailable;
        }
    }
#endif
}
