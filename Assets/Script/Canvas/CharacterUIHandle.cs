using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUIHandle : MonoBehaviour
{
    public Transform[] icons;
    public Transform locked;

    public void Create(CharacterDataSO.CharacterSO data)
    {
        this.gameObject.SetActive(true);
        this.gameObject.name = data.ID;

        var baseImg = this.gameObject.GetComponent<Image>();
        baseImg.sprite = data.Icon;
        baseImg.preserveAspect = true;

        if (data.PayType == CharacterDataSO.PayType.Ads)
        {
            var dataCharacterLocal = RuntimeStorageData.Player.GetCharacterUnlockData(data.ID);
            if (dataCharacterLocal == null)
            {
                this.gameObject.name = $"{this.gameObject.name}_ads";
                locked.SetActive(true);
            }    
            else
            {
                var dateString = dataCharacterLocal.Time;
                DateTime dateTime = DateTime.Parse(dateString);
                if(dateTime.AddHours(12) < DateTime.Now)
                {
                    this.gameObject.name = $"{this.gameObject.name}_ads";
                    locked.SetActive(true);
                }  
                else
                {
                    locked.SetActive(false);
                }    
            }    
        }
        else
        {
            locked.SetActive(false);
        }
        for (int j = 0; j < icons.Length; j++)
        {
            var img = icons[j].GetComponent<Image>();
            img.sprite = data.Icon;
            img.preserveAspect = true;
        }
    }

    public void OnMode(GameManager.GameStyle mode)
    {
        for (int i = 0; i < icons.Length; i++)
        {
            if (i == (int)mode) icons[i].SetActive(true);
            else icons[i].SetActive(false);
        }
    }   
    
    public void OnLocked(bool active)
    {
        locked.SetActive(active);
    }    

    public void BtnUnlock()
    {
        
    }    

    public void BtnUnlockWithAds()
    {
        var ID = this.gameObject.name.Split('_')[0];
        Debug.Log($"Unlock character {ID}");
        RuntimeStorageData.Player.AddCharacterUnlockData(ID);
        this.gameObject.name = ID;
        locked.SetActive(false);
    }    
}
