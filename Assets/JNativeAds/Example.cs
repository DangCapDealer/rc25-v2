using System.Collections.Generic;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using JKit.Monetize.Ads;
using UnityEngine;
using UnityEngine.UI;

public class Example : MonoBehaviour
{
    [Tooltip("Kéo Button UI vào đây để gọi Android Activity")]
    public Button callAndroidActivityButton;

    private NativeOverlay nativeOverlay;

    void Start()
    {
        // Gán listener cho nút (nếu bạn sử dụng UI Button trong Unity)
        if (callAndroidActivityButton != null)
        {
            callAndroidActivityButton.onClick.AddListener(ShowNative);
            Debug.Log("Button listener assigned.");
        }
        else
        {
            Debug.LogWarning("Call Android Activity Button is not assigned in the Inspector.");
        }

        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        var request = new ConsentRequestParameters();
        var debugSettings = new ConsentDebugSettings
        {
            // Geography appears as in EEA for debug devices.
            DebugGeography = DebugGeography.EEA,
            TestDeviceHashedIds = new List<string> { SystemInfo.deviceUniqueIdentifier },
        };
        request.ConsentDebugSettings = debugSettings;
        ConsentInformation.Reset();
        ConsentInformation.Update(request, OnConsentInfoUpdated);
    }


    public void LoadAd()
    {
        NativeOverlay.Load("ca-app-pub-8190506959251235/2043096362",30, async (overlay, error) =>
        {
            if (overlay != null || error == null)
            {
                nativeOverlay = overlay;
                Debug.Log("NativeOverlay loaded successfully.");
            }
            else
            {
                await Awaitable.WaitForSecondsAsync(1);
                await Awaitable.MainThreadAsync();
                nativeOverlay?.Destroy();
                nativeOverlay = null;
                LoadAd();
            }
        });
    }

    private void OnConsentInfoUpdated(FormError consentError)
    {
        if (consentError != null)
        {
            Debug.LogError(consentError);
            return;
        }

        ConsentForm.LoadAndShowConsentFormIfRequired(formError =>
        {
            Debug.Log("Consent form loaded");

            if (formError != null)
            {
                Debug.LogError(formError);
                return;
            }

            if (ConsentInformation.CanRequestAds())
            {
                Debug.Log("Ads Provider Initialized.");

                MobileAds.Initialize(initStatus =>
                {
                    LoadAd();
                });
            }
        });
    }

    public void ShowNative()
    {
        if (nativeOverlay != null)
        {
            nativeOverlay.OnClosed += () => { Debug.Log("NativeOverlay closed."); };
            nativeOverlay.OnClicked += () => { Debug.Log("NativeOverlay clicked."); };
            nativeOverlay.OnImpression += () => { Debug.Log("NativeOverlay impression."); };
            
            nativeOverlay.Show();
        }
    }
}