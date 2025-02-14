using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUIHandle : MonoBehaviour
{
    public Image icon;
    public Transform locked;

    private void OnEnable()
    {
        GameEvent.OnIAPurchase += OnIAPurechase;
    }

    private void OnDisable()
    {
        GameEvent.OnIAPurchase -= OnIAPurechase;
    }

    private void OnIAPurechase(string productID, string action)
    {
        if (RuntimeStorageData.Player.IsProductId(InappController.Instance.GetProductIdByIndex(0)) ||
            RuntimeStorageData.Player.IsProductId(InappController.Instance.GetProductIdByIndex(1)))
        {
            Unlock();
        }
    }

    public void Create(CharacterDataSO.CharacterSO data)
    {
        this.gameObject.SetActive(true);
        this.gameObject.name = data.ID;

        var baseImg = this.gameObject.GetComponent<Image>();
        baseImg.sprite = data.Icon;
        baseImg.preserveAspect = true;

        if (data.PayType == CharacterDataSO.PayType.Ads)
        {
#if ADMOB
            if (RuntimeStorageData.Player.IsProductId(InappController.Instance.GetProductIdByIndex(0)) ||
                RuntimeStorageData.Player.IsProductId(InappController.Instance.GetProductIdByIndex(1)))
            {
                locked.SetActive(false);
            }
            else
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
                    if (dateTime.AddHours(12) < DateTime.Now)
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
#else
            locked.SetActive(false);
#endif
        }
        else
        {
            locked.SetActive(false);
        }

        icon.sprite = data.Icon;
        icon.preserveAspect = true;
    }

    public void Reload()
    {
        icon.rectTransform.anchoredPosition = Vector2.zero;
        icon.transform.SetActive(true);
    }
    
    public void OnLocked(bool active)
    {
        locked.SetActive(active);
    }    

    public void Unlock()
    {
        var ID = this.gameObject.name.Split('_')[0];
        Debug.Log($"Unlock character {ID}");
        RuntimeStorageData.Player.AddCharacterUnlockData(ID);
        this.gameObject.name = ID;
        locked.SetActive(false);
    }    

    public void BtnUnlockWithAds()
    {
        AdManager.Instance.ShowRewardedAd(() =>
        {
            Unlock();
        });
    }    
}
