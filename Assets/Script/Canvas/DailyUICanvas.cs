using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyUICanvas : PopupCanvas
{
    public Transform[] contents;

    public override void Show(Popup p)
    {
        base.Show(p);
        if(p == Popup.Checkin)
        {
            for (int i = 0; i < contents.Length; i++)
            {
                if(i < RuntimeStorageData.Player.NumberOfCheckIn)
                {
                    var img = contents[i].FindChildByRecursion("Image").GetComponent<Image>();
                    img.color = "858585".toColor();
                    var text = contents[i].FindChildByRecursion("Text (TMP)").GetComponent<TMP_Text>();
                    text.text = "Claimed";
                }
            }
        }
    }

    public void ReceiverCharacter(string ID)
    {
        if (RuntimeStorageData.Player.DayCheckIn == DateTime.Now.Day)
            return;
        if (RuntimeStorageData.Player.NumberOfCheckIn >= contents.Length)
            return;
        if (contents[RuntimeStorageData.Player.NumberOfCheckIn].name == ID)
        {
            RuntimeStorageData.Player.AddCharacterUnlockData(ID);
            var img = contents[RuntimeStorageData.Player.NumberOfCheckIn].FindChildByRecursion("Image").GetComponent<Image>();
            img.color = "858585".toColor();
            var text = contents[RuntimeStorageData.Player.NumberOfCheckIn].FindChildByRecursion("Text (TMP)").GetComponent<TMP_Text>();
            text.text = "Claimed";
            RuntimeStorageData.Player.NumberOfCheckIn += 1;
            RuntimeStorageData.Player.DayCheckIn = DateTime.Now.Day;
        }
    }
}
