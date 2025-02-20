using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCanvasHandle : MonoBehaviour
{
    public float Delay = 5.0f;
    public Transform MuteParent;

    private float clipCaculate = 0;
    private float clipLenght = 0;
    private Image clipTimeImage;

    public void SetMuteCharacterUI(int index)
    {
        for (int i = 0; i < MuteParent.childCount; i++)
        {
            if(i == index) MuteParent.GetChild(i).gameObject.SetActive(true);
            else MuteParent.GetChild(i).gameObject.SetActive(false);
        }
    }

    public Transform HeadphoneParent;
    public void SetHeadphoneCharacterUI(int index)
    {
        for (int i = 0; i < HeadphoneParent.childCount; i++)
        {
            if (i == index) HeadphoneParent.GetChild(i).SetActive(true);
            else HeadphoneParent.GetChild(i).SetActive(false);
        }
    }

    private void OnEnable()
    {
        BtnReset();
    }

    public void CreateCanvas()
    {
        var characterSetting = transform.GetChild(0);
        var characterSettingRect = characterSetting.GetComponent<RectTransform>();
        if (GameManager.Instance.Style == GameManager.GameStyle.Normal ||
            GameManager.Instance.Style == GameManager.GameStyle.Horror)
        {
            this.transform.SetActive(false);
            characterSettingRect.sizeDelta = characterSettingRect.sizeDelta.WithY(65.0f);
            characterSetting.GetChild(0).SetActive(false);
        }
        else if (GameManager.Instance.Style == GameManager.GameStyle.Battle)
        {
            this.transform.SetActive(true);
            characterSettingRect.sizeDelta = characterSettingRect.sizeDelta.WithY(80.0f);
            characterSetting.GetChild(0).SetActive(true);

            clipTimeImage = characterSetting.GetChild(0).GetChild(0).GetComponent<Image>();
            clipTimeImage.fillAmount = 1;

            var parent = this.transform.parent;
            var parentScript = parent.GetComponent<Character>();
            clipLenght = parentScript.GetSoundData().GetClipLenght();
            clipCaculate = clipLenght;
        }

        var canvas = this.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.overrideSorting = true;
            canvas.sortingLayerName = "Character";
        }
    }    

    public void BtnReset()
    {
        Delay = 5.0f;
    }

    private void Update()
    {
        if(GameManager.Instance.Style == GameManager.GameStyle.Normal || GameManager.Instance.Style == GameManager.GameStyle.Horror)
        {
            Delay -= Time.deltaTime;
            if (Delay < 0) this.transform.Hide();
        }    
        else if(GameManager.Instance.Style == GameManager.GameStyle.Battle)
        {
            if(clipCaculate > 0)
            {
                clipCaculate -= Time.deltaTime;
                if(clipCaculate < 0)
                {
                    clipTimeImage.fillAmount = 0;
                    var characterSetting = transform.GetChild(0);
                    var btnRemove = characterSetting.FindChildByParent("BtnRemove").GetComponent<Button>();
                    btnRemove.onClick?.Invoke();
                }
                else
                {
                    clipTimeImage.fillAmount = clipCaculate / clipLenght;
                }
            }
        }
    }
}
