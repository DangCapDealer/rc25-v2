using DG.Tweening;
using PimDeWitte.UnityMainThreadDispatcher;
using UnityEngine;

public partial class GameUICanvas : MonoBehaviour
{
    public Transform Content;
    public GameObject CharacterUIPrefab;

    public Transform BtnUnlockTransform;
    public Transform BtnAddTransform;

    private float LastClickAutoTime = 0;
    public float SpaceTimeButton = 0.6f;

    [Header("Auto Add Slot Character")]
    public float PopupAdCharacterAfter = 60.0f;
    private float _timerPopupAddCharacter = 0;

    private void Update()
    {
        AutoSlotCharacter();

        Mode3Update();
    }

    public void CreateGame()
    {
        if (GameManager.Instance.IsGameDefault())
        {
            if (RuntimeStorageData.Player.IsProductId(InappController.Instance.GetProductIdByIndex(0)) || 
                RuntimeStorageData.Player.IsProductId(InappController.Instance.GetProductIdByIndex(1)))
            {
                GameManager.Instance.NumberOfCharacter = 10;
                BtnAddTransform.SetActive(false);
            }
            else BtnAddTransform.SetActive(true);
        }

        _timerPopupAddCharacter = 0;

        Mode3Create();


        CreateUIGame();
        GridInCamera.Instance.CreatePosition();
        GameEvent.OnUIThemeMethod(GameManager.Instance.Style.ToString());
    }

    private void CreateUIGame()
    {
        Debug.Log("[GameUICanvas] Create UI Game");
        bool IsCharacterAds = false;
        var dataCharacterSO = GameSpawn.Instance.GetAllCharacter();
        for (int i = 0; i < dataCharacterSO.Length; i++)
        {
            var _childCount = Content.childCount;
            GameObject _child = null;
            if(i >= _childCount) _child = PoolByID.Instance.GetPrefab(CharacterUIPrefab, Content);
            else _child = Content.GetChild(i).gameObject;
            var _script = _child.GetComponent<CharacterUIHandle>();
            _script.Create(dataCharacterSO[i]);   

            if(_child.name.EndsWith("_ads") == true)
                IsCharacterAds = true;
        }
        BtnUnlockTransform.SetActive(Manager.Instance.IsPopupUnlock == false ? false : IsCharacterAds);
    }

    //private bool IsCharacterAds = false;

    public void AddSlotInGame()
    {
        if (GameManager.Instance.NumberOfCharacter >= 10)
            return;

        AdManager.Instance.ShowRewardedThridAd(() =>
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                GameManager.Instance.NumberOfCharacter += 1;
                GameSpawn.Instance.CreateNewPositionCharacter();
            });
        });
    }

    private void AutoSlotCharacter()
    {
        //if (GameManager.Instance.NumberOfCharacter >= 10) return;
        //if (GameManager.Instance.IsGameCustom()) return;

        //_timerPopupAddCharacter += Time.deltaTime;
        //if(_timerPopupAddCharacter > PopupAdCharacterAfter)
        //{
        //    _timerPopupAddCharacter = 0;
        //    CanvasSystem.Instance._popupUICanvas.ShowPopup(Popup.UnlockOnce);
        //}    
    }    


    public void UnlockAllCharacter()
    {
        BtnUnlockTransform.SetActive(false);
        for (int i = 0; i < Content.childCount; i++)
        {
            var _child = Content.GetChild(i);
            if (_child.name.EndsWith("ads") == false)
                continue;
            var _script = _child.GetComponent<CharacterUIHandle>();
            _script.Unlock();
        }
    }

    private void OnEnable()
    {
        GameEvent.OnUIDragDown += OnUIDragDown;
        GameEvent.OnUIDragUp += OnUIDragUp;
        GameEvent.OnUIDrag += OnUIDrag;
        GameEvent.OnUITheme += OnUITheme;


        if (Manager.Instance != null)
            Manager.Instance.IngameScreenID = "GameUICanvas";
        if (AdManager.Instance != null)
            AdManager.Instance.InterAdSpaceTimeAutoCounter = 0;
    }

    private void OnDisable()
    {
        GameEvent.OnUIDragDown -= OnUIDragDown;
        GameEvent.OnUIDragUp -= OnUIDragUp;
        GameEvent.OnUIDrag -= OnUIDrag;
        GameEvent.OnUITheme -= OnUITheme;
    }

    private Transform _targetMsg;
    private Transform _contentParent;
    private void OnUIDragDown(string msg)
    {
        if (_targetMsg != null || _contentParent != null)
            return;

        _targetMsg = GetPositionByName(msg);
        _contentParent = _targetMsg.parent;
        _targetMsg.SetParent(this.transform);
    }

    private Transform _lastTargetObjectFromDrag;
    private void OnUIDrag(string msg)
    {
        if (_targetMsg == null || _contentParent == null)
            return;

        var local = _targetMsg.position.AddX(-30.0f).AddY(1.0f).WithZ(0);
        var targetObject = GameSpawn.Instance.CheckingNearPositionInPool(local);
        if (targetObject != null)
        {
            if(_lastTargetObjectFromDrag == null)
            {
                _lastTargetObjectFromDrag = targetObject;
                changeStateBaseCharacter(targetObject, true, false);
            }   
            else if(_lastTargetObjectFromDrag != targetObject)
            {
                changeStateBaseCharacter(_lastTargetObjectFromDrag, false, true);
                changeStateBaseCharacter(targetObject, true, false);
                _lastTargetObjectFromDrag = targetObject;
            }

            GameSpawn.Instance.CharacterMessage("Character", "CharacterUICanvas", "Show");  
        }    
        else
        {
            if (_lastTargetObjectFromDrag != null)
            {
                changeStateBaseCharacter(_lastTargetObjectFromDrag, false, true);
                _lastTargetObjectFromDrag = null;
            }    
        }    
    }

    private void changeStateBaseCharacter(Transform _targetObject, params bool[] actives)
    {
        var _target = _targetObject.FindChildByParent("Normal");
        if (_target != null) _target.SetActive(actives[0]);
        var _untarget = _targetObject.FindChildByParent("Horror");
        if (_untarget != null) _untarget.SetActive(actives[1]);
    }    

    private void OnUIDragUp(string msg)
    {
        if (_targetMsg == null || _contentParent == null)
            return;

        _lastTargetObjectFromDrag = null;
        var local = _targetMsg.position.AddX(-30.0f).AddY(1.0f).WithZ(0);
        var targetObject = GameSpawn.Instance.CheckingNearPositionInPool(local);
        if (targetObject == null)
        {
            _targetMsg.DOKill();
            _targetMsg.DOMove(_contentParent.position, 0.1f).OnComplete(() => 
            {
                _targetMsg.SetParent(_contentParent);
                _targetMsg = null;
                _contentParent = null;
            });
        }
        else
        {
            _targetMsg.SetParent(_contentParent);
            _targetMsg.SetActive(false);
            GameSpawn.Instance.SpawnCharacterIntoPosition(_contentParent.name, targetObject);
            _targetMsg = null;
            _contentParent = null;
        }
    }


    private void OnUITheme(string msg)
    {
        Content.ForChild(_child =>
        {
            var characterUI = _child.GetComponent<CharacterUIHandle>();
            if (characterUI != null)
            {
                characterUI.Reload();
            }
        });
    }

    private Transform GetPositionByName(string name)
    {
        for (int i = 0; i < Content.childCount; i++)
        {
            var child = Content.GetChild(i);
            if (child.name == name)
            {
                for (int j = 0; j < child.childCount; j++)
                {
                    if (child.GetChild(j).IsActive() == true)
                        return child.GetChild(j);
                }
            }
        }

        return null;
    }

    public void ReloadCharacterUIButton(string name)
    {
        Content.ForChild(_child =>
        {
            if (_child.name == name)
            {
                var characterUI = _child.GetComponent<CharacterUIHandle>();
                if (characterUI != null)
                {
                    characterUI.Reload();
                }
            }
        });
    }    

    public void BtnHome()
    {
        if (Time.time - LastClickAutoTime < SpaceTimeButton)
            return;
        TutorialSystem.Instance.DisableTutorial();
        CanvasSystem.Instance._loadingUICanvas.ShowLoading(() =>
        {
            AdManager.Instance.ShowInterstitialHomeAd(() =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    CanvasSystem.Instance.ChooseScreen("HomeUICanvas");
                    CanvasSystem.Instance.AutoNoAd();
                    GameManager.Instance.GameReset();
                    GameManager.Instance.State = GameManager.GameState.Stop;
                    GameSpawn.Instance.RemoveAllCharacter();
                    SoundSpawn.Instance.MuteAll();
                    //CanvasSystem.Instance.ShowNativeCollapse();
                    MusicManager.Instance.PlaySound(Music.Main);
                });
            }, () =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(AdManager.Instance.ShowNativeOverlayAd);
            });
        });
    }

    public void BtnAuto()
    {
        if (SoundSpawn.Instance.IsReady() == false) return;
        var targetObject = GameSpawn.Instance.GetOncePositionInPool();
        if (targetObject == null) return;
        Transform target = null;
        int counter = 0;
        while (counter < 15)
        {
            var child  = Content.GetChild(Random.Range(0, Content.childCount));
            if (child.name.EndsWith("_ads") == true)
                continue;
            var iconObject = child.FindChildByParent("Icon");
            if (iconObject.IsActive() == true) 
            { 
                target = iconObject;
                break; 
            }
            counter++;
        }
        if (target == null) return;
        target.SetActive(false);
        LastClickAutoTime = Time.time;
        GameSpawn.Instance.SpawnCharacterIntoPosition(target.parent.name, targetObject);
    }

    public void BtnSetting()
    {

    }   
    
    public void BtnReset()
    {
        if (Time.time - LastClickAutoTime < SpaceTimeButton)
            return;
        GameSpawn.Instance.RemoveAllCharacter();
        SoundSpawn.Instance.MuteAll();
        GameManager.Instance.GameReset();
        GridInCamera.Instance.CreatePosition();

        Mode3UIReset();

        GameEvent.OnUIThemeMethod(GameManager.Instance.Style.ToString());
    }    

    public void BtnUnlock()
    {
        CanvasSystem.Instance._popupUICanvas.ShowPopup(Popup.Unlock);   
    }    
}
