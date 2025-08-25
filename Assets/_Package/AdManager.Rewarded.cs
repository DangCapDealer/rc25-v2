using UnityEngine;
using UnityEngine.Events;
#if ADMOB
using GoogleMobileAds.Api;
using PimDeWitte.UnityMainThreadDispatcher;
#endif

public partial class AdManager
{
#if ADMOB
    [Header("AD REWARD")]
    public bool UseReward = true;
    public bool IsPreloadReward = true;
    public AdState RewardAdState = AdState.NotAvailable;
    public int _rewardReloadCount = 0;
    public string _adUnitRewardId = "ca-app-pub-5904408074441373/1580420414";

    private RewardedAd _rewardedAd;

    private void LoadRewardedAd()
    {
        if (RewardAdState == AdState.Loading) return;
        RewardAdState = AdState.Loading;

        if (_rewardedAd != null) { _rewardedAd.Destroy(); _rewardedAd = null; }

        var adRequest = new AdRequest();
        RewardedAd.Load(_adUnitRewardId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                RewardAdState = AdState.NotAvailable;
                _rewardReloadCount += 1;
                return;
            }

            _rewardedAd = ad;
            _rewardedAd.OnAdPaid += (revenue) => { AppflyerEventSender.Instance.logAdRevenue(revenue); };
            _rewardedAd.OnAdFullScreenContentClosed += () => { RewardAdState = AdState.NotAvailable; };
            _rewardedAd.OnAdFullScreenContentFailed += (AdError error2) => { RewardAdState = AdState.NotAvailable; };
            RewardAdState = AdState.Ready;
            _rewardReloadCount = 0;
        });
    }

    public void ShowRewardedAd(UnityAction RewardComplete)
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            ResetOpenAdSpaceTime();
            _rewardedAd.Show((Reward reward) => UnityMainThreadDispatcher.Instance().Enqueue(() => RewardComplete?.Invoke()));
        }
    }

    [Header("AD REWARD THIRD")]
    public bool IsPreloadRewardThrid = true;
    public AdState RewardThridAdState = AdState.NotAvailable;
    public int _rewardThridLoadCount = 0;
    public string _adUnitRewardThriddId = "ca-app-pub-5904408074441373/7387280867";

    private RewardedAd _rewardedThridAd;

    private void LoadRewardedThridAd()
    {
        if (RewardThridAdState == AdState.Loading) return;
        RewardThridAdState = AdState.Loading;

        if (_rewardedThridAd != null) { _rewardedThridAd.Destroy(); _rewardedThridAd = null; }

        var adRequest = new AdRequest();
        RewardedAd.Load(_adUnitRewardThriddId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                RewardThridAdState = AdState.NotAvailable;
                _rewardThridLoadCount += 1;
                return;
            }

            _rewardedThridAd = ad;
            _rewardedThridAd.OnAdPaid += (revenue) => { AppflyerEventSender.Instance.logAdRevenue(revenue); };
            _rewardedThridAd.OnAdFullScreenContentClosed += () => { RewardThridAdState = AdState.NotAvailable; };
            _rewardedThridAd.OnAdFullScreenContentFailed += (AdError error2) => { RewardThridAdState = AdState.NotAvailable; };
            RewardThridAdState = AdState.Ready;
            _rewardThridLoadCount = 0;
        });
    }

    public void ShowRewardedThridAd(UnityAction RewardComplete)
    {
        if (_rewardedThridAd != null && _rewardedThridAd.CanShowAd())
        {
            ResetOpenAdSpaceTime();
            _rewardedThridAd.Show((Reward reward) => UnityMainThreadDispatcher.Instance().Enqueue(() => RewardComplete?.Invoke()));
        }
    }
#endif
}
