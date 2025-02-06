using DG.Tweening;
using PimDeWitte.UnityMainThreadDispatcher;
using UnityEngine;

public class GameUICanvas : MonoBehaviour
{
    public Transform Content;
    public GameObject CharacterUIPrefab;
    //public Transform Pointer;

    public Transform BtnUnlockTransform;

    //public Transform _banner;

    private void Start()
    {
        //_banner.SetActive(RuntimeStorageData.Player.IsLoadAds);
    }


    public void CreateGame()
    {
        CreateUIGame();
        GridInCamera.Instance.CreatePosition();
        GameEvent.OnUIThemeMethod(GameManager.Instance.Style.ToString());
    }

    private void CreateUIGame()
    {
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

        Debug.Log($"----------- {IsCharacterAds}");
        BtnUnlockTransform.SetActive(Manager.Instance.IsPopupUnlock == false ? false : IsCharacterAds);
    }

    private bool IsCharacterAds = false;

    public void AddSlotInGame()
    {
        if (GameManager.Instance.NumberOfCharacter >= 10)
            return;

        AdManager.Instance.ShowRewardedThridAd(() =>
        {
            //IsCharacterAds = true;
            //GameManager.Instance.NumberOfCharacter += 1;
            //GameSpawn.Instance.CreateNewPositionCharacter();
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                GameManager.Instance.NumberOfCharacter += 1;
                GameSpawn.Instance.CreateNewPositionCharacter();
            });
        });
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
        AdManager.Instance.ShowInterstitialHomeAd(() =>
        {
            //IsGotoHomeAfterIntertitialAd = true;
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                CanvasSystem.Instance.ChooseScreen("HomeUICanvas");
                GameManager.Instance.GameReset();
                GameSpawn.Instance.RemoveAllCharacter();
                SoundSpawn.Instance.MuteAll();
                CanvasSystem.Instance.ShowNativeCollapse();
                MusicManager.Instance.PlaySound(Music.Main);
            });
        }, () =>
        {
            //IsShowNativeIntertitialAd = true;
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                CanvasSystem.Instance.ShowNativeIntertitial();
            });
        });
    }

    //private bool IsGotoHomeAfterIntertitialAd = false;
    //private bool IsShowNativeIntertitialAd = false;
    //private void AsyncIntertitialEvent()
    //{
    //    //if(IsGotoHomeAfterIntertitialAd == true)
    //    //{
    //    //    IsGotoHomeAfterIntertitialAd = false;
    //    //    CanvasSystem.Instance.ChooseScreen("HomeUICanvas");
    //    //    GameManager.Instance.GameReset();
    //    //    GameSpawn.Instance.RemoveAllCharacter();
    //    //    SoundSpawn.Instance.MuteAll();
    //    //    CanvasSystem.Instance.ShowNativeCollapse();
    //    //    MusicManager.Instance.PlaySound(Music.Main);
    //    //}   
        
    //    //if(IsShowNativeIntertitialAd == true)
    //    //{
    //    //    IsShowNativeIntertitialAd = false;
    //    //    CanvasSystem.Instance.ShowNativeIntertitial();
    //    //}  
        
    //    //if(IsCharacterAds == true)
    //    //{
    //    //    IsCharacterAds = false;
    //    //    GameManager.Instance.NumberOfCharacter += 1;
    //    //    GameSpawn.Instance.CreateNewPositionCharacter();
    //    //}
    //}    

    //private void Update()
    //{
    //    AsyncIntertitialEvent();
    //}

    public void BtnAuto()
    {
        var targetObject = GameSpawn.Instance.GetOncePositionInPool();
        if (targetObject == null)
            return;

        Transform target = null;
        int counter = 0;
        while (counter < 15)
        {
            var child  = Content.GetChild(Random.Range(0, Content.childCount));
            if (child.name.EndsWith("_ads") == true)
                continue;
            if(GameManager.Instance.Style == GameManager.GameStyle.Normal)
            {
                if (child.GetChild(0).IsActive() == true)
                {
                    target = child.GetChild(0);
                    break;
                }
            }
            else if(GameManager.Instance.Style == GameManager.GameStyle.Horror)
            {
                if (child.GetChild(1).IsActive() == true)
                {
                    target = child.GetChild(1);
                    break;
                }
            }
            counter++;
        }
        if (target == null)
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
