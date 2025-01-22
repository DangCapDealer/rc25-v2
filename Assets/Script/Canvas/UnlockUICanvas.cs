using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnlockUICanvas : PopupCanvas
{
    public int NumberOfAds = 0;
    public TMP_Text TMP_NumberOfAds;
    private float cd;
    public Transform CDObject;
    public TMP_Text TMP_Countdown;

    public bool UnlockAll = false;
    public bool LoadTMP = false;

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
                UnlockAll = true;
            }
            LoadTMP = true;
            cd = 4;
        });
    }

    private void LoadTMPNumberOfAds()
    {
        TMP_NumberOfAds.text = $"{NumberOfAds} / 3";
    }

    private void Update()
    {
        if(cd>0)
        {
            cd -= Time.deltaTime;
            TMP_Countdown.text = ((int)cd).ToString();
            if(cd < 0)
            {
                CDObject.SetActive(false);
            }    
        }   
        
        if(UnlockAll)
        {
            UnlockAll = false;
            CanvasSystem.Instance._gameUICanvas.UnlockAllCharacter();

            base.Hide();
        }
        
        if(LoadTMP)
        {
            CDObject.SetActive(true);
            LoadTMP = false;
            LoadTMPNumberOfAds();
        }    
    }
}
