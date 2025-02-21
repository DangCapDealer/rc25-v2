using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    private SoundPrefab _soundPrefab;
    public SoundPrefab GetSoundData() => _soundPrefab;

    public Color _activeColor;
    public Color _inactiveColor;

    public void CreateCharacter(Camera gameCamera)
    {
        touchDisable = false;
        var isFound = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (child.name == "CharacterCanvas") continue;
            if (GameManager.Instance.Style == GameManager.GameStyle.Battle)
            {
                if (transform.position.x > 0)
                {
                    if(child.name == GameManager.GameStyle.Battle.ToString())
                    {
                        isFound = true;
                        onAnimationCharacter(child);
                        setupSound(GameManager.GameStyle.Battle);
                    }
                    else child.SetActive(false);
                }
                else if(transform.position.x < 0)
                {
                    if (child.name == GameManager.GameStyle.Normal.ToString())
                    {
                        isFound = true;
                        onAnimationCharacter(child);
                        setupSound(GameManager.GameStyle.Normal);
                    }
                    else child.SetActive(false);
                }
                continue;
            }
            if (child.name == GameManager.Instance.Style.ToString())
            {
                isFound = true;
                onAnimationCharacter(child);
                setupSound(GameManager.Instance.Style);
            }
            else child.SetActive(false);
        }

        if (isFound == false)
        {
            LogSystem.LogError($"Not found {transform.name}");
        }  
        else
        {
            RegisterCameraCanvas(gameCamera);
            RegisterCanvas();
            SetAnimationCanvas("Mute", "Stop");
            SetAnimationCanvas("Headphone", "Stop");
            SoundManager.Instance.PlayOnShot(Sound.CharacterDrop);
        }
    }

    private void setupSound(GameManager.GameStyle gameStyle)
    {
        var soundId = $"{transform.name}_{gameStyle}";
        _soundPrefab = SoundSpawn.Instance.Find(soundId);
        _soundPrefab.Mute = false;
    }

    private void onAnimationCharacter(Transform _targetObject)
    {
        if (_targetObject != null)
        {
            _targetObject.SetActive(true);
            var skeletonAnimation = _targetObject.GetComponentsInChildren<SkeletonAnimation>();
            if (skeletonAnimation != null)
            {
                for (int i = 0; i < skeletonAnimation.Length; i++)
                {
                    var skeleton = skeletonAnimation[i];
                    if (skeleton.skeleton == null)
                    {
                        LogSystem.LogError("Skeleton is null in SetColor()!");
                        continue;
                    }
                    skeleton.skeleton.SetColor(_activeColor);
                }
            }
            else LogSystem.LogError($"Skeleton not found {_targetObject.name}");
            _targetObject.localScale = _targetObject.localScale.WithY(0);
            _targetObject.DOKill();
            _targetObject.DOScaleY(_targetObject.localScale.x, 0.3f);
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
        var canvasScript = canvasObject.GetComponent<CharacterCanvasHandle>();
        canvasScript.CreateCanvas();
        var characterSetting = canvasObject.GetChild(0);

        var btnRemove = characterSetting.GetChild(1).GetComponent<Button>();
        btnRemove.onClick.RemoveAllListeners();
        btnRemove.onClick.AddListener(() =>
        {
            BtnRemove();
            canvasScript.BtnReset();
        });
        var btnSound = characterSetting.GetChild(2).GetComponent<Button>();
        btnSound.onClick.RemoveAllListeners();
        btnSound.onClick.AddListener(() =>
        {
            BtnSound();
            canvasScript.BtnReset();
        });
        var btnHeadphone = characterSetting.GetChild(3).GetComponent<Button>();
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

        for (int i = 0; i < transform.childCount; i++)
        {
            var _child = transform.GetChild(i);
            if (_child.name == "CharacterCanvas")
                continue;
            if (_child.IsActive())
            {
                _child.GetComponentsInChildren<SkeletonAnimation>().SimpleForEach(_skeleton => _skeleton.skeleton.SetColor(_soundPrefab.Mute ? _inactiveColor : _activeColor));
            }    
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
            //Debug.Log(IsHeadphone);
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
        if(GameManager.Instance.Style == GameManager.GameStyle.Normal || 
            GameManager.Instance.Style == GameManager.GameStyle.Horror)
        {
            TouchReset();
            var canvasObject = this.transform.FindChildByParent("CharacterCanvas");
            canvasObject.SetActive(!canvasObject.IsActive());
        }
    }

    void HandleDoubleClick()
    {
        if (GameManager.Instance.Style == GameManager.GameStyle.Normal ||
            GameManager.Instance.Style == GameManager.GameStyle.Horror)
        {
            TouchReset();
            var canvasObject = this.transform.FindChildByParent("CharacterCanvas");
            var canvasScript = canvasObject.GetComponent<CharacterCanvasHandle>();
            var characterSetting = canvasObject.GetChild(0);
            var btn = characterSetting.GetChild(1).GetComponent<Button>();
            btn.onClick?.Invoke();
        }
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
