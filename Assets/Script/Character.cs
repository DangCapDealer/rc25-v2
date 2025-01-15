using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    private SoundPrefab _soundPrefab;

    //public bool IsMute = false;
    //public bool IsHeadphone = false;

    public void CreateCharacter(Camera gameCamera)
    {
        //IsMute = false;
        //IsHeadphone = false;

        _soundPrefab = SoundSpawn.Instance.Find(transform.name);
        _soundPrefab.Mute = false;

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
        //GameSpawn.Instance.CreateBaseCharacter(this.transform);
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
            _soundPrefab.Mute = !_soundPrefab.Mute;
            if (_soundPrefab.Mute == true)
            {
                SetAnimationCanvas("Mute", "Play");
                SetAnimationCanvas("Headphone", "Stop");
            }
            else
            {
                SetAnimationCanvas("Mute", "Stop");
                SetAnimationCanvas("Headphone", "Stop");
            }
        }
        else
        {
            SetAnimationCanvas("Headphone", "Stop");
        }
    }

    private void CaculateHeadphone(Transform target)
    {
        if (target == this.transform)
        {
            _soundPrefab.Mute = false;
            SetAnimationCanvas("Mute", "Stop");
            SetAnimationCanvas("Headphone", "Play");
        }   
        else
        {
            _soundPrefab.Mute = true;
            SetAnimationCanvas("Mute", "Play");
            SetAnimationCanvas("Headphone", "Stop");
        }
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
