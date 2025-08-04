#if ADMOB
using GoogleMobileAds.Api;
#endif
using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ReceiverMultiNativeAd : MonoBehaviour
{
    public NativeAdPosition[] adPositions;
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
#if ADMOB
    private NativeAd nativeAd;
#endif
    public string ctaSize = "Medium";

    public Color adColor = Color.white;
    public bool IsReloadNativeAd = true;
    private RequestNativeAd[] NativeAdHandles;

    private bool IsNativeImport = false;
    private int NativeIndex = 0;



    private float NativeAutoHideTimer = 0;

    public void BtnClose()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => gameObject.SetActive(false));
    }


    private void OnEnable()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => _content.SetActive(false));
        NativeAutoHideTimer = 0;

        if(NativeAdHandles == null) NativeAdHandles = new RequestNativeAd[adPositions.Length];
        for (int i = 0; i < NativeAdHandles.Length; i++)
        {
            NativeAdHandles[i] = AdNativeManager.Instance.GetNativeAd(adPositions[i]);
            NativeAdHandles[i].IsReloadNativeAd = IsReloadNativeAd;
            NativeAdHandles[i].OnChangeNativeAd += NativeAdHandle_OnChangeNativeAd;
            NativeAdHandles[i].OnClickedNativeAd += NativeAdHandle_OnClickedNativeAd;
        }

        IsNativeImport = false;
        NativeIndex = 0;
#if UNITY_EDITOR
        UnityMainThreadDispatcher.Instance().Enqueue(() => _content.SetActive(true));
#endif
    }

    private void OnDisable()
    {
        for (int i = 0; i < NativeAdHandles.Length; i++)
        {
            NativeAdHandles[i].OnChangeNativeAd -= NativeAdHandle_OnChangeNativeAd;
            NativeAdHandles[i].OnClickedNativeAd -= NativeAdHandle_OnClickedNativeAd;
        }
    }

    private void NativeAdHandle_OnClickedNativeAd()
    {
        Debug.Log($"[ReceiverMultiNativeAd] Native Ad Clicked {Time.time}");
        NativeIndex += 1;
        if (NativeIndex >= NativeAdHandles.Length) NativeIndex = 0;
        IsNativeImport = false;

        Debug.Log($"[ReceiverMultiNativeAd] Native Ad Index {NativeIndex}");
    }

    private void NativeAdHandle_OnChangeNativeAd()
    {
        //IsNativeImport = false;
    }

    private void Update()
    {
        if (RuntimeStorageData.Player.IsLoadAds == false) return;
        if (IsNativeImport == true) return;
        if (NativeAdHandles[NativeIndex] == null) return;

        if (NativeAdHandles[NativeIndex].nativeAdLoaded == true)
        {
            NativeAdHandles[NativeIndex].IsUsed = true;
            IsNativeImport = true;
#if ADMOB
            nativeAd = NativeAdHandles[NativeIndex].nativeAd;
            RegisterAd(nativeAd);
#endif
        }
        else if (_content.IsActive())
        {
#if !UNITY_EDITOR
            NativeAutoHideTimer += Time.deltaTime;
            if(NativeAutoHideTimer >= 0.5f)
            {
                NativeAutoHideTimer = 0;
               _content.SetActive(false);
            } 
#endif
        }
    }
#if ADMOB
    private void RegisterAd(NativeAd importNativeAd)
    {
        Debug.Log($"[ReceiverMultiNativeAd] Register Ad Index {NativeIndex}");

        _content.Show();
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
        if (!this.nativeAd.RegisterCallToActionGameObject(adCTA)) Debug.Log($"[ReceiverMultiNativeAd] Register CTA game object error!!!");
        adImage.texture = ImageTextures[0];
    }    
#endif
}
