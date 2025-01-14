using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static GameEvent;
using static UnityEngine.GraphicsBuffer;

public class GameUICanvas : MonoBehaviour
{
    public Transform Content;
    public Transform Pointer;

    public void CreateGame()
    {
        CreateUIGame();
        GridInCamera.Instance.CreatePosition();
        GameEvent.OnUIThemeMethod(GameManager.Instance.Style.ToString());
    }

    private void CreateUIGame()
    {
        Content.ForChild(_child =>
        {
            var dataSO = GameSpawn.Instance.FindCharacterData(_child.name);
            if (dataSO == null)
            {
                Debug.Log($"Data {_child.name} not found");
                return;
            }

            var imgs = _child.GetComponentsInChildren<Image>();
            imgs.ForEach(img =>
            {
                img.sprite = dataSO.Icon;
                img.preserveAspect = true;
            });
        });
    }

    public void AddSlotInGame()
    {
        if (GameManager.Instance.NumberOfCharacter >= 10)
            return;

        GameManager.Instance.NumberOfCharacter += 1;

        GridInCamera.Instance.CreatePosition();
        GameEvent.OnUIThemeMethod(GameManager.Instance.Style.ToString());
    }

    private void OnEnable()
    {
        GameEvent.OnUIDragDown += OnUIDragDown;
        GameEvent.OnUIDragUp += OnUIDragUp;
        GameEvent.OnUITheme += OnUITheme;

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
        _targetMsg = GetPositionByName(msg);
        _contentParent = _targetMsg.parent;
        _targetMsg.parent = this.transform;
    }

    private void OnUIDragUp(string msg)
    {
        var local = _targetMsg.position.AddX(-30.0f).AddY(1.0f).WithZ(0);
        var targetObject = GameSpawn.Instance.CheckingNearPositionInPool(local);
        if (targetObject == null)
        {
            _targetMsg.DOKill();
            _targetMsg.DOMove(_contentParent.position, 0.1f).OnComplete(() => { _targetMsg.parent = _contentParent; });
        }
        else
        {
            _targetMsg.parent = _contentParent;
            _targetMsg.SetActive(false);
            GameSpawn.Instance.SpawnCharacterIntoPosition(_contentParent.name, targetObject);
        }

        _targetMsg = null;
        _contentParent = null;
    }


    private void OnUITheme(string msg)
    {
        Content.ForChild(_child =>
        {
            _child.ForChild(x =>
            {
                x.localPosition = Vector2.zero;

                if (x.name.EndsWith(msg) == true) x.SetActive(true);
                else x.SetActive(false);
            });
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

            child.ForChild(x =>
            {
                x.localPosition = Vector2.zero;

                if (x.name.EndsWith(GameManager.Instance.Style.ToString()) == true) 
                    x.SetActive(true);
                else 
                    x.SetActive(false);
            });
        }
    }    

    public void BtnHome()
    {
        CanvasSystem.Instance.ChooseScreen("HomeUICanvas");
        GameManager.Instance.GameReset();
        GameSpawn.Instance.RemoveAllCharacter();
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
        GameManager.Instance.GameReset();
        GridInCamera.Instance.CreatePosition();
        GameEvent.OnUIThemeMethod(GameManager.Instance.Style.ToString());
    }    
}
