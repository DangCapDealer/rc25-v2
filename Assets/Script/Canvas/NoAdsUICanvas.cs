using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NoAdsUICanvas : PopupCanvas
{
    public void BtnBuyProduct()
    {
        InappController.Instance.BuyProductID("remove_ads_subscription", (_purchaseComplete) =>
        {
            if (_purchaseComplete == true)
            {
                RuntimeStorageData.Player.IsLoadAds = false;
                GameEvent.OnIAPurchaseMethod("remove_ads_subscription");
                base.Hide();
            }
        });
    }

    public void BtnRestorePurchase()
    {
        base.Hide();
    }

    public void BtnTermOfUser()
    {
        base.Hide();
    }

    public void BtnPrivacyPolicy()
    {
        base.Hide();
    }
}
