using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCanvasHandle : MonoBehaviour
{
    public float Delay = 5.0f;
    public Transform MuteParent;
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

    public void BtnReset()
    {
        Delay = 5.0f;
    }

    private void Update()
    {
        Delay -= Time.deltaTime;
        if(Delay < 0 ) this.transform.Hide();
    }
}
