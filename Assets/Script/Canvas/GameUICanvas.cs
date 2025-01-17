using DG.Tweening;
using UnityEngine;

public class GameUICanvas : MonoBehaviour
{
    public Transform Content;
    public GameObject CharacterUIPrefab;
    //public Transform Pointer;

    public void CreateGame()
    {
        CreateUIGame();
        GridInCamera.Instance.CreatePosition();
        GameEvent.OnUIThemeMethod(GameManager.Instance.Style.ToString());
    }

    private void CreateUIGame()
    {
        var dataCharacterSO = GameSpawn.Instance.GetAllCharacter();
        for (int i = 0; i < dataCharacterSO.Length; i++)
        {
            var _childCount = Content.childCount;
            GameObject _child = null;
            if(i >= _childCount) _child = PoolByID.Instance.GetPrefab(CharacterUIPrefab, Content);
            else _child = Content.GetChild(i).gameObject;
            var _script = _child.GetComponent<CharacterUIHandle>();
            _script.Create(dataCharacterSO[i]);   
        }
    }

    public void AddSlotInGame()
    {
        if (GameManager.Instance.NumberOfCharacter >= 10)
            return;

        AdManager.Instance.ShowRewardedThridAd(() =>
        {
            GameManager.Instance.NumberOfCharacter += 1;
            GameSpawn.Instance.CreateNewPositionCharacter();
        });
    }

    public void UnlockAllCharacter()
    {
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

    private void OnUIDragUp(string msg)
    {
        if (_targetMsg == null || _contentParent == null)
            return;

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
            for (int j = 0; j < _child.childCount; j++)
            {
                var x = _child.GetChild(j);
                if (x.name.StartsWith("Icon") == false)
                    continue;

                x.localPosition = Vector2.zero;
                if (x.name.EndsWith(GameManager.Instance.Style.ToString()) == true)
                    x.SetActive(true);
                else
                    x.SetActive(false);
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
        for (int i = 0; i < Content.childCount; i++)
        {
            var child = Content.GetChild(i);
            if (child.name != name)
                continue;

            for(int j = 0;j < child.childCount; j++)
            {
                var x = child.GetChild(j);
                if (x.name.StartsWith("Icon") == false)
                    continue;

                x.localPosition = Vector2.zero;
                if (x.name.EndsWith(GameManager.Instance.Style.ToString()) == true)
                    x.SetActive(true);
                else
                    x.SetActive(false);
            }    
        }
    }    

    public void BtnHome()
    {
        if(AdManager.Instance != null)
        {
            AdManager.Instance.ShowInterstitialHomeAd(() =>
            {
                CanvasSystem.Instance.ChooseScreen("HomeUICanvas");
                GameManager.Instance.GameReset();
                GameSpawn.Instance.RemoveAllCharacter();
                SoundSpawn.Instance.MuteAll();
            });
        }    
        else
        {
            CanvasSystem.Instance.ChooseScreen("HomeUICanvas");
            GameManager.Instance.GameReset();
            GameSpawn.Instance.RemoveAllCharacter();
            SoundSpawn.Instance.MuteAll();
        }    
    }

    public void BtnAuto()
    {
        Transform target = null;
        for (int i = 0; i < Content.childCount; i++)
        {
            var child = Content.GetChild(i);
            if (child.GetChild(0).IsActive() == true)
            {
                target = child.GetChild(0);
                break;
            }
            else if (child.GetChild(1).IsActive() == true)
            {
                target = child.GetChild(1);
                break;
            }
        }

        var targetObject = GameSpawn.Instance.GetOncePositionInPool();
        if (targetObject == null)
            return;
        target.SetActive(false);
        GameSpawn.Instance.SpawnCharacterIntoPosition(target.parent.name, targetObject);
    }

    public void BtnSetting()
    {

    }   
    
    public void BtnReset()
    {
        GameSpawn.Instance.RemoveAllCharacter();
        SoundSpawn.Instance.MuteAll();
        GameManager.Instance.GameReset();
        GridInCamera.Instance.CreatePosition();
        GameEvent.OnUIThemeMethod(GameManager.Instance.Style.ToString());
    }    

    public void BtnUnlock()
    {
        CanvasSystem.Instance._popupUICanvas.ShowPopup(Popup.Unlock);   
    }    
}
