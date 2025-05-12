using AppsFlyerSDK;
using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppflyerEventSender : MonoBehaviour
{
    public bool logAppflyerEvent = true;
    public bool isAppflyerDebug = true;
    public static AppflyerEventSender Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

    }
    private void Start()
    {
        AppsFlyerAdRevenue.start();
        AppsFlyer.setIsDebug(isAppflyerDebug);
    }
    private readonly float microValue = 1000000;
    public void logAdRevenue(AdValue adValue)
    {
        Dictionary<string, string> additionalParams = new Dictionary<string, string>();

        double value = adValue.Value / microValue;
        Debug.Log("======" + value);
        AppsFlyerAdRevenue.logAdRevenue("admob",
                                        AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeGoogleAdMob,
                                        value,
                                        adValue.CurrencyCode,
                                        additionalParams);
    }

    public void af_tutorial_completion(bool af_success,int af_tutorial_id)
    {
        if (!logAppflyerEvent) return;
        Dictionary<string, string> eventValues = new Dictionary<string, string>();
        eventValues.Add("af_success", af_success.ToString());
        eventValues.Add("af_tutorial_id", af_tutorial_id.ToString());
        sendEvent("af_tutorial_completion", eventValues);
    }
    public void af_level_achieved(string af_level, string af_score)
    {
        if (!logAppflyerEvent) return;
        Dictionary<string, string> eventValues = new Dictionary<string, string>();
        eventValues.Add("af_level", af_level.ToString());
        eventValues.Add("af_score", af_score.ToString());
        sendEvent("af_level_achieved", eventValues);
    }
    public void af_inters_ad_eligible()
    {
        sendEvent("af_inters_ad_eligible", null);
    }
    public void af_inters_api_called()
    {
        sendEvent("af_inters_api_called", null);
    }
    public void af_inters_displayed()
    {
        sendEvent("af_inters_displayed", null);
    }
    public void af_rewarded_ad_eligible()
    {

        sendEvent("af_rewarded_ad_eligible", null);
    }
    public void af_rewarded_api_called()
    {
        sendEvent("af_rewarded_api_called", null);
    }
    public void af_rewarded_displayed()
    {
        sendEvent("af_rewarded_displayed", null);
    }
    public void af_rewarded_ad_completed()
    {
        sendEvent("af_rewarded_ad_completed", null);
    }
    void sendEvent(string eventName, Dictionary<string, string> eventValues)
    {
        if (logAppflyerEvent)
        AppsFlyer.sendEvent(AFInAppEvents.PURCHASE, eventValues);
    }
}