using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    private SoundPrefab _soundPrefab;
    public Transform _normal;
    public Transform _horror;
    private Color _activeColor = new Color(255f / 255f, 255f / 255f, 255f / 255f, 1f);
    private Color _inactiveColor = new Color(185f / 255f, 185f / 255f, 185f / 255f, 1f);

    //private float lastClickTime = 0f; // Lưu thời điểm click trước
    //private const float doubleClickThreshold = 0.3f; // Khoảng thời gian giữa 2 click để tính là double click


    public void CreateCharacter(Camera gameCamera)
    {
        var soundId = $"{transform.name}_{GameManager.Instance.Style}";
        _soundPrefab = SoundSpawn.Instance.Find(soundId);
        _soundPrefab.Mute = false;

        touchDisable = false;

        RegisterCameraCanvas(gameCamera);
        RegisterCanvas();

        SetAnimationCanvas("Mute", "Stop");
        SetAnimationCanvas("Headphone", "Stop");

        switch(GameManager.Instance.Style)
        {
            case GameManager.GameStyle.Normal:
                _normal.SetActive(true);
                _horror.SetActive(false);
                _normal.GetComponentsInChildren<SkeletonAnimation>().SimpleForEach(_skeleton => _skeleton.skeleton.SetColor(_activeColor));

                _normal.localScale = _normal.localScale.WithY(0);
                _normal.DOKill();
                _normal.DOScaleY(_normal.localScale.x, 0.3f);
                break;
            case GameManager.GameStyle.Horror:
                _normal.SetActive(false);
                _horror.SetActive(true);
                _horror.GetComponentsInChildren<SkeletonAnimation>().SimpleForEach(_skeleton => _skeleton.skeleton.SetColor(_activeColor));

                _horror.localScale = _horror.localScale.WithY(0);
                _horror.DOKill();
                _horror.DOScaleY(_horror.localScale.x, 0.3f);
                break;
        }

        SoundManager.Instance.PlayOnShot(Sound.CharacterDrop);
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
        var canvasScript = canvasObject.GetComponent<CharacterCanvasHandle>();
        var characterSetting = canvasObject.GetChild(0);

        var btnRemove = characterSetting.GetChild(0).GetComponent<Button>();
        btnRemove.onClick.RemoveAllListeners();
        btnRemove.onClick.AddListener(() =>
        {
            BtnRemove();
            canvasScript.BtnReset();
        });
        var btnSound = characterSetting.GetChild(1).GetComponent<Button>();
        btnSound.onClick.RemoveAllListeners();
        btnSound.onClick.AddListener(() =>
        {
            BtnSound();
            canvasScript.BtnReset();
        });
        var btnHeadphone = characterSetting.GetChild(2).GetComponent<Button>();
        btnHeadphone.onClick.RemoveAllListeners();
        btnHeadphone.onClick.AddListener(() =>
        {
            BtnHeadphone();
            canvasScript.BtnReset();
        });
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
        GameEvent.OnTouchBegan -= OnTouchBegan;
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

        switch (GameManager.Instance.Style)
        {
            case GameManager.GameStyle.Normal:
                _normal.GetComponentsInChildren<SkeletonAnimation>().SimpleForEach(_skeleton => _skeleton.skeleton.SetColor(_soundPrefab.Mute ? _inactiveColor : _activeColor));
                break;
            case GameManager.GameStyle.Horror:
                _horror.GetComponentsInChildren<SkeletonAnimation>().SimpleForEach(_skeleton => _skeleton.skeleton.SetColor(_soundPrefab.Mute ? _inactiveColor : _activeColor));
                break;
        }
    }

    private void CaculateMute(Transform target)
    {
        if (target == this.transform) _soundPrefab.Mute = !_soundPrefab.Mute;
        else _soundPrefab.Mute = false;
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

    private int touchNumber = 0;
    private float touchTime = 0;
    private float touchThreshold = 0.2f;
    private bool touchDisable = false;

    private void OnTouchBegan(RaycastHit hit)
    {
        if (touchDisable == true)
            return;
        if (hit.transform != this.transform)
            return;

        if (touchNumber == 0)
        {
            touchNumber += 1;
            touchTime = Time.time;
            Invoke("HandleSingleClick", touchThreshold);
        }
        else if (touchNumber == 1)
        {
            if(Time.time - touchTime < touchThreshold)
            {
                touchDisable = true;
                CancelInvoke("HandleSingleClick");
                HandleDoubleClick();
            }     
        }
    }

    private void TouchReset()
    {
        touchNumber = 0;
        touchTime = 0;
    }    

    void HandleSingleClick()
    {
        TouchReset();

        var canvasObject = this.transform.FindChildByParent("CharacterCanvas");
        canvasObject.SetActive(!canvasObject.IsActive());
    }

    void HandleDoubleClick()
    {
        TouchReset();

        var canvasObject = this.transform.FindChildByParent("CharacterCanvas");
        var canvasScript = canvasObject.GetComponent<CharacterCanvasHandle>();
        var characterSetting = canvasObject.GetChild(0);
        var btn = characterSetting.GetChild(0).GetComponent<Button>();
        btn.onClick?.Invoke();
    }

    private void SetAnimationCanvas(params string[] msg)
    {
        if (msg[0] == "Mute")
        {
            var canvasObject = this.transform.FindChildByParent("CharacterCanvas");
            var scriptCharacterCanvas = canvasObject.GetComponent<CharacterCanvasHandle>();
            if (msg[1] == "Play")
            {
                scriptCharacterCanvas.SetMuteCharacterUI(0);
            }    
            else if (msg[1] == "Stop")
            {
                scriptCharacterCanvas.SetMuteCharacterUI(1);
            }
        }
        else if (msg[0] == "Headphone")
        {
            var canvasObject = this.transform.FindChildByParent("CharacterCanvas");
            var scriptCharacterCanvas = canvasObject.GetComponent<CharacterCanvasHandle>();
            if (msg[1] == "Play")
            {
                scriptCharacterCanvas.SetHeadphoneCharacterUI(1);
            }
            else if (msg[1] == "Stop")
            {
                scriptCharacterCanvas.SetHeadphoneCharacterUI(0);
            }
        }    
    }    
}
