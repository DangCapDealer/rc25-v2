using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnlockUICanvas : PopupCanvas
{
    public int NumberOfAds = 0;
    public TMP_Text TMP_NumberOfAds;

    public override void Show(Popup p)
    {
        base.Show(p);
        if(p == Popup.Unlock)
        {
            LoadTMPNumberOfAds();
        }
    }

    public void ShowAd()
    {
        AdManager.Instance.ShowRewardedSecondAd(() =>
        {
            NumberOfAds += 1;
            if(NumberOfAds >= 3)
            {
                NumberOfAds = 0;
                CanvasSystem.Instance._gameUICanvas.UnlockAllCharacter();
                base.Hide();
            }
            LoadTMPNumberOfAds();
        });
    }

    private void LoadTMPNumberOfAds()
    {
        TMP_NumberOfAds.text = $"{NumberOfAds} / 3";
    }
}
