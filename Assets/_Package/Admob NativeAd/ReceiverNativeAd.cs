using GoogleMobileAds.Api;
using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReceiverNativeAd : MonoBehaviour
{
    public NativeAdPosition adPosition;
    public GameObject _content;


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
    [HideInInspector] public RequestNativeAd NativeAdHandle;

    private bool IsNativeImport = false;
    private float timer = 0;

    public void BtnClose()
    {
        if (adPosition == NativeAdPosition.Banner) return;
        if (NativeAdHandle == null) return;

        NativeAdHandle.NativeAdState = AdManager.AdState.NotAvailable;
        this.gameObject.SetActive(false);
    }


#if ADMOB
    private void OnEnable()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => _content.SetActive(false));
        timer = 0;
        NativeAdHandle = AdNativeManager.Instance.GetNativeAd(adPosition);
        NativeAdHandle.IsReloadNativeAd = IsReloadNativeAd;
        NativeAdHandle.OnChangeNativeAd += _OnChangeNativeAd;
        NativeAdHandle.OnClickedNativeAd += _OnClickedNativeAd;
        IsNativeImport = false;
#if UNITY_EDITOR
        UnityMainThreadDispatcher.Instance().Enqueue(() => _content.SetActive(true));
#endif
    }

    private void OnDisable()
    {
        NativeAdHandle.OnChangeNativeAd -= _OnChangeNativeAd;
        NativeAdHandle.OnClickedNativeAd -= _OnClickedNativeAd;
    }

    private void _OnChangeNativeAd()
    {
        IsNativeImport = false;
    }

    private void _OnClickedNativeAd()
    {
        if (adPosition == NativeAdPosition.Banner) 
            UnityMainThreadDispatcher.Instance().Enqueue(() => _content.SetActive(false));
        else 
            UnityMainThreadDispatcher.Instance().Enqueue(() => gameObject.SetActive(false));
        UnityMainThreadDispatcher.Instance().Enqueue(() => IsNativeImport = false);
    }

    private void Update()
    {
        if (RuntimeStorageData.Player.IsLoadAds == false) return;
        if (NativeAdHandle == null) return;
        if (IsNativeImport == true) return;

        if (NativeAdHandle.NativeAdLoaded() == true)
        {
            nativeAd = NativeAdHandle.nativeAd;
            NativeAdHandle.IsUsed = true;
            IsNativeImport = true;

            _content.SetActive(true);
            var IconTexture = this.nativeAd.GetIconTexture();
            var HeadlineText = this.nativeAd.GetHeadlineText();
            var BodyText = this.nativeAd.GetBodyText();
            var CallToActionText = this.nativeAd.GetCallToActionText();
            var ImageTextures = this.nativeAd.GetImageTextures();

            adIcon.color = IconTexture == null ? adColor : Color.white;

            adIcon.texture = IconTexture;
            adHeadline.text = HeadlineText;
            adBody.text = BodyText;
            adCallToAction.text = CallToActionText;
            if (!this.nativeAd.RegisterCallToActionGameObject(adCTA)) Debug.Log($"[{this.GetType().ToString()}] Register CTA game object error!!!");
            adImage.texture = ImageTextures[0];
        }
        else
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
