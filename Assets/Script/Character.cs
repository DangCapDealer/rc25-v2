using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    private SoundPrefab _soundPrefab;
    public Transform _normal;
    public Transform _horror;

    public void CreateCharacter(Camera gameCamera)
    {
        var soundId = $"{transform.name}_{GameManager.Instance.Style}";
        _soundPrefab = SoundSpawn.Instance.Find(soundId);
        _soundPrefab.Mute = false;

        RegisterCameraCanvas(gameCamera);
        RegisterCanvas();

        SetAnimationCanvas("Mute", "Stop");
        SetAnimationCanvas("Headphone", "Stop");

        switch(GameManager.Instance.Style)
        {
            case GameManager.GameStyle.Normal:
                _normal.SetActive(true);
                _horror.SetActive(false);
                break;
            case GameManager.GameStyle.Horror:
                _normal.SetActive(false);
                _horror.SetActive(true);
                break;
        }    
    }

    private void RegisterCameraCanvas(Camera gameCamera)
    {
        var canvasObject = this.transform.FindChildByParent("CharacterCanvas");
        var canvasScript = canvasObject.GetComponent<Canvas>();
        canvasScript.worldCamera = gameCamera;
    }    

    private void RegisterCanvas()
    {
        var canvasObject = this.transform.FindChildByParent("CharacterCanvas");
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
        GameSpawn.Instance.RemoveCharacter(this.gameObject);
        _soundPrefab.Mute = true;
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
        }
        else
        {
            _soundPrefab.Mute = false;
        }
        SetAnimationCanvas("Mute", _soundPrefab.Mute == true ? "Play" : "Stop");
        SetAnimationCanvas("Headphone", "Stop");
        _soundPrefab.IsHeadphone = false;
    }

    private void CaculateHeadphone(Transform target)
    {
        if (target == this.transform)
        {
            _soundPrefab.Mute = false;
            SetAnimationCanvas("Headphone", _soundPrefab.IsHeadphone == true ? "Stop" : "Play");
            DOVirtual.DelayedCall(0.1f, () => { _soundPrefab.IsHeadphone = !_soundPrefab.IsHeadphone; });
        }   
        else
        {
            var scriptTarget = target.GetComponent<Character>();
            var IsHeadphone = scriptTarget._soundPrefab.IsHeadphone;
            Debug.Log(IsHeadphone);
            _soundPrefab.Mute = !IsHeadphone;
            SetAnimationCanvas("Mute", _soundPrefab.Mute == true ? "Play" : "Stop");
            SetAnimationCanvas("Headphone", "Stop");
        }
    }    

    private void OnTouchBegan(RaycastHit hit)
    {
        if (hit.transform != this.transform)
            return;

        var canvasObject = this.transform.FindChildByParent("CharacterCanvas");
        canvasObject.SetActive(!canvasObject.IsActive());
    }

    private void SetAnimationCanvas(params string[] msg)
    {
        if (msg[0] == "Mute")
        {
            var canvasObject = this.transform.FindChildByParent("CharacterCanvas");
            var scriptCharacterCanvas = canvasObject.GetComponent<CharacterCanvasHandle>();
            //var characterSetting = canvasObject.GetChild(0);
            //var animation = characterSetting.GetChild(1).GetComponent<Animation>();
            if (msg[1] == "Play")
            {
                //animation.Play();
                scriptCharacterCanvas.SetMuteCharacterUI(0);
            }    
            else if (msg[1] == "Stop")
            {
                //animation.Stop();
                //var image = characterSetting.GetChild(1).GetComponent<Image>();
                //image.color = image.color.WithAlpha(1.0f);
                scriptCharacterCanvas.SetMuteCharacterUI(1);
            }
        }
        else if (msg[0] == "Headphone")
        {
            var canvasObject = this.transform.FindChildByParent("CharacterCanvas");
            var scriptCharacterCanvas = canvasObject.GetComponent<CharacterCanvasHandle>();
            //var characterSetting = canvasObject.GetChild(0);
            //var animation = characterSetting.GetChild(2).GetComponent<Animation>();
            if (msg[1] == "Play")
            {
                //animation.Play();
                scriptCharacterCanvas.SetHeadphoneCharacterUI(1);
            }
            else if (msg[1] == "Stop")
            {
                //animation.Stop();
                //var image = characterSetting.GetChild(2).GetComponent<Image>();
                //image.color = image.color.WithAlpha(1.0f);
                scriptCharacterCanvas.SetHeadphoneCharacterUI(0);
            }
        }    
    }    
}
