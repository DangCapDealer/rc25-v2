using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    private SoundPrefab _soundPrefab;

    public bool IsMute = false;
    public bool IsHeadphone = false;

    public void CreateCharacter(Camera gameCamera)
    {
        IsMute = false;
        IsHeadphone = false;

        _soundPrefab = SoundSpawn.Instance.Find(transform.name);
        _soundPrefab.Mute = IsMute;

        RegisterCameraCanvas(gameCamera);
        RegisterCanvas();
    }

    private void RegisterCameraCanvas(Camera gameCamera)
    {
        var canvasObject = this.transform.GetChild(0);
        var canvasScript = canvasObject.GetComponent<Canvas>();
        canvasScript.worldCamera = gameCamera;
    }    

    private void RegisterCanvas()
    {
        var canvasObject = this.transform.GetChild(0);
        canvasObject.SetActive(false);
        var characterSetting = canvasObject.GetChild(0);

        var btnRemove = characterSetting.GetChild(0).GetComponent<Button>();
        btnRemove.onClick.RemoveAllListeners();
        btnRemove.onClick.AddListener(() => BtnRemove());
        var btnSound = characterSetting.GetChild(1).GetComponent<Button>();
        btnSound.onClick.RemoveAllListeners();
        btnSound.onClick.AddListener(() => BtnSound());
        var btnHeadphone = characterSetting.GetChild(2).GetComponent<Button>();
        btnHeadphone.onClick.RemoveAllListeners();
        btnHeadphone.onClick.AddListener(() => BtnHeadphone());
    }    

    public void BtnSound()
    {
        GameEvent.OnCharacterUISetupMethod("Mute", this.transform);
    }   
    
    public void BtnHeadphone()
    {
        GameEvent.OnCharacterUISetupMethod("Headphone", this.transform);
    }   
    
    public void BtnRemove()
    {
        GameSpawn.Instance.CreateBaseCharacter(this.transform);
        GameSpawn.Instance.RemoveCharacter(this.gameObject);
    }

    private void OnEnable()
    {
        GameEvent.OnTouchBegan += OnTouchBegan;
        GameEvent.OnCharacterUISetup += OnCharacterUISetup;
    }

    private void OnDisable()
    {
        GameEvent.OnTouchBegan += OnTouchBegan;
        GameEvent.OnCharacterUISetup -= OnCharacterUISetup;
    }

    private void OnCharacterUISetup(string msg, Transform target)
    {
        switch (msg)
        {
            case "Mute":
                CaculateMute(target);
                break;
            case "Headphone":
                CaculateHeadphone(target);
                break;
        }
    }

    private void CaculateMute(Transform target)
    {
        if(target == this.transform)
        {
            IsMute = !IsMute;
            //AudioSource.mute = IsMute;

            //if(AudioSource.mute == true)
            //{
            //    SetAnimationCanvas("Mute", "Play");
            //}    
            //else
            //{
            //    SetAnimationCanvas("Mute", "Stop");
            //}    
        }      
    }

    private int TurnOfHeadphone;
    private void CaculateHeadphone(Transform target)
    {
        TurnOfHeadphone += 1;
        if (target == this.transform)
        {
            IsHeadphone = TurnOfHeadphone == 1 ? true : false;
            if(IsHeadphone == true)
            {
                SetAnimationCanvas("Headphone", "Play");
            }   
            else
            {
                SetAnimationCanvas("Headphone", "Stop");
            }    

        }   
        else
        {
            IsHeadphone = TurnOfHeadphone == 1 ? true : false;
            //if(IsHeadphone == true)
            //{
            //    AudioSource.mute = true;
            //    SetAnimationCanvas("Mute", "Stop");
            //    SetAnimationCanvas("Headphone", "Play");
            //}    
            //else
            //{
            //    AudioSource.mute = IsMute;
            //    if (AudioSource.mute == true)
            //    {
            //        SetAnimationCanvas("Mute", "Play");
            //    }
            //    else
            //    {
            //        SetAnimationCanvas("Mute", "Stop");
            //    }
            //    SetAnimationCanvas("Headphone", "Stop");
            //}    
        }

        if (TurnOfHeadphone == 2) TurnOfHeadphone = 0;
    }    

    private void OnTouchBegan(RaycastHit hit)
    {
        if (hit.transform != this.transform)
            return;

        var canvasObject = this.transform.GetChild(0);
        canvasObject.SetActive(!canvasObject.IsActive());
    }

    private void SetAnimationCanvas(params string[] msg)
    {
        if (msg[0] == "Mute")
        {
            var canvasObject = this.transform.GetChild(0);
            var characterSetting = canvasObject.GetChild(0);
            var animation = characterSetting.GetChild(1).GetComponent<Animation>();
            if (msg[1] == "Play")
            {
                animation.Play();
            }    
            else if (msg[1] == "Stop")
            {
                animation.Stop();
                var image = characterSetting.GetChild(1).GetComponent<Image>();
                image.color = image.color.WithAlpha(1.0f);
            }
        }
        else if (msg[0] == "Headphone")
        {
            var canvasObject = this.transform.GetChild(0);
            var characterSetting = canvasObject.GetChild(0);
            var animation = characterSetting.GetChild(2).GetComponent<Animation>();
            if (msg[1] == "Play")
            {
                animation.Play();
            }
            else if (msg[1] == "Stop")
            {
                animation.Stop();
                var image = characterSetting.GetChild(2).GetComponent<Image>();
                image.color = image.color.WithAlpha(1.0f);
            }
        }    
    }    
}
