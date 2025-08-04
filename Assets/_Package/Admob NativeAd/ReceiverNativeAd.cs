#if ADMOB
using GoogleMobileAds.Api;
#endif
using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReceiverNativeAd : MonoBehaviour
{
    public NativeAdPosition adPosition;
    public GameObject _content;

#if ADMOB
    public RawImage adIcon;
    public RawImage adImage;
    public RawImage adChoices;
    public GameObject adLabel;
    public Text adHeadline;
    public GameObject adCTA;
    public Text adCallToAction;
    public Text adAdvertiser;
    public Text adBody;

    private NativeAd nativeAd;
    public string ctaSize = "Medium";

    public Color adColor = Color.white;
    public bool IsReloadNativeAd = true;
    private RequestNativeAd NativeAdHandle;

    private bool IsNativeImport = false;
    private float timer = 0;
    private int adIndex = 0;

    public void BtnClose()
    {
        if (adPosition == NativeAdPosition.Banner) return;
        UnityMainThreadDispatcher.Instance().Enqueue(() => gameObject.SetActive(false));
    }

    private void OnEnable()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => _content.SetActive(false));
        timer = 0;
        NativeAdHandle = AdNativeManager.Instance.GetNativeAd(adPosition);
        NativeAdHandle.IsReloadNativeAd = IsReloadNativeAd;

        NativeAdHandle.OnChangeNativeAd += NativeAdHandle_OnChangeNativeAd;
        NativeAdHandle.OnClickedNativeAd += NativeAdHandle_OnClickedNativeAd;

        IsNativeImport = false;
#if UNITY_EDITOR
        UnityMainThreadDispatcher.Instance().Enqueue(() => _content.SetActive(true));
#endif
    }

    private void OnDisable()
    {
        NativeAdHandle.OnChangeNativeAd -= NativeAdHandle_OnChangeNativeAd;
        NativeAdHandle.OnClickedNativeAd -= NativeAdHandle_OnClickedNativeAd;
    }    

    private void NativeAdHandle_OnClickedNativeAd()
    {

    }

    private void NativeAdHandle_OnChangeNativeAd()
    {
        IsNativeImport = false;
    }    

    private void Update()
    {
        if (RuntimeStorageData.Player.IsLoadAds == false) return;
        if (NativeAdHandle == null) return;
        if (IsNativeImport == true) return;

        if (NativeAdHandle.nativeAdLoaded == true)
        {
            NativeAdHandle.IsUsed = true;
            IsNativeImport = true;
            nativeAd = NativeAdHandle.nativeAd;

            _content.SetActive(true);
            var IconTexture = this.nativeAd.GetIconTexture();
            var HeadlineText = this.nativeAd.GetHeadlineText();
            var BodyText = this.nativeAd.GetBodyText();
            var CallToActionText = this.nativeAd.GetCallToActionText();
            var ImageTextures = this.nativeAd.GetImageTextures();

            if (IconTexture != null) adIcon.color = Color.white;
            else adIcon.color = adColor;

            adIcon.texture = IconTexture;
            adHeadline.text = HeadlineText;
            adBody.text = BodyText;
            adCallToAction.text = CallToActionText;
            if (!this.nativeAd.RegisterCallToActionGameObject(adCTA)) Debug.Log($"[{this.GetType().ToString()}] Register CTA game object error!!!");
            adImage.texture = ImageTextures[0];
        }
        else if (_content.IsActive())
        {
#if !UNITY_EDITOR
            timer += Time.deltaTime;
            if(timer >= 0.5f)
            {
                timer = 0;
               _content.SetActive(false);
            } 
#endif
        }
    }
#else
    private void Update()
    {
        if (this.transform.IsActive() == true)
            this.transform.SetActive(false);
    }
#endif

}
