using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NoAdsUICanvas : PopupCanvas
{
    public int productIndex = 0;
    public void BtnBuyProduct()
    {
        if(RuntimeStorageData.Player.IsProductId(InappController.Instance.GetProductIdByIndex(productIndex)) == false)
        {
            BuyProduct(InappController.Instance.GetProductIdByIndex(productIndex));
        }
        else
        {
            Debug.Log("Out of Product");
        }

    }

    private void BuyProduct(string productName)
    {
        InappController.Instance.BuyProductID(productName, (_purchaseComplete) =>
        {
            if (_purchaseComplete == true)
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    RuntimeStorageData.Player.AddProductId(productName);
                    GameEvent.OnIAPurchaseMethod(productName, "add");
                    base.Hide();
                });
            }
        });
    }

    [Header("Info")]
    public Text _infoText;
    public Text _policyText;

    private void Start()
    {
        var productPrice = InappController.Instance.GetProductInfo(InappController.Instance.GetProductIdByIndex(productIndex));
        _infoText.text = $"Only {productPrice}/week";
        var productName = InappController.Instance.GetProductName(InappController.Instance.GetProductIdByIndex(productIndex));
        _policyText.text = string.Format(_policyText.text, productName, productPrice);
    }

    public void BtnRealPolicy()
    {
        Application.OpenURL("https://support.google.com/googleplay/answer/7018481?hl=en&co=GENIE.Platform%3DAndroid");
    }

    public override void Show(Popup p)
    {
        if (p == Popup.NoAdsEvent || p == Popup.NoAdsVipMember)
            AdManager.Instance.HideBanner();
        base.Show(p);
    }

    public override void Hide()
    {
        //AdManager.Instance.ShowNativeOverlayBannerAd();
        AdManager.Instance.ShowBanner();
        base.Hide();
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
