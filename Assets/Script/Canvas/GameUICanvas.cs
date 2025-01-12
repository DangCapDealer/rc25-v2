using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameUICanvas : MonoBehaviour
{
    public Transform Content;
    public Mask ContentMask;

    public Transform Pointer;

    public void CreateGame()
    {
        GridInCamera.Instance.CreatePosition();
        GameEvent.OnThemeStypeMethod(GameManager.Instance.Style.ToString());
    }

    public void AddSlotInGame()
    {
        if (GameManager.Instance.NumberOfCharacter >= 10)
            return;

        GameManager.Instance.NumberOfCharacter += 1;

        GridInCamera.Instance.CreatePosition();
        GameEvent.OnThemeStypeMethod(GameManager.Instance.Style.ToString());
    }    

    private void OnEnable()
    {
        GameEvent.OnUIDragDown += OnUIDragDown;
        GameEvent.OnUIDrag += OnUIDrag;
        GameEvent.OnUIDragUp += OnUIDragUp;
        GameEvent.OnThemeStype += OnThemeStype;
    }

    private void OnDisable()
    {
        GameEvent.OnUIDragDown -= OnUIDragDown;
        GameEvent.OnUIDrag -= OnUIDrag;
        GameEvent.OnUIDragUp -= OnUIDragUp;
        GameEvent.OnThemeStype -= OnThemeStype;
    }

    private void OnUIDragDown(string msg)
    {
        ContentMask.enabled = false;
    }

    private void OnUIDrag(string msg)
    {
        //var target = GetPositionByName(msg);
        //var local = target.position.AddX(-30.0f).AddY(1.0f).WithZ(0);

        //Pointer.position = local;
    }

    private void OnUIDragUp(string msg)
    {
        var target = GetPositionByName(msg);
        var local = target.position.AddX(-30.0f).AddY(1.0f).WithZ(0);
        var targetObject = GameSpawn.Instance.CheckingNearCharacterInPool(local);
        if (targetObject == null)
        {
            var rect = target.GetComponent<RectTransform>();
            rect.DOKill();
            rect.DOAnchorPos(Vector2.zero, 0.1f).OnComplete(() => ContentMask.enabled = true);
        }
        else
        {
            ContentMask.enabled = true;
            GameSpawn.Instance.SpawnCharacterIntoPosition(msg, targetObject);
        }
    }


    private void OnThemeStype(string msg)
    {
        Content.ForChild(_child =>
        {
            _child.ForChild(x =>
            {
                if (x.name.EndsWith(msg) == true) x.SetActive(true);
                else x.SetActive(false);
            });
        });
    }

    private Transform GetPositionByName(string name)
    {
        for(int i = 0; i < Content.childCount; i++)
        {
            var child = Content.GetChild(i);
            if (child.name == name)
            {
                for(int j = 0; j < child.childCount; j++)
                {
                    if (child.GetChild(j).IsActive() == true)
                        return child.GetChild(j);
                }    
            }    
        }    

        return null;
    }

}
