using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class TutorialSystem : MonoSingleton<TutorialSystem>
{
    public Canvas canvas;
    [Header("HAND UI")]
    public Transform hand;
    private RectTransform handRect;

    private float _autoHandle;
    public float _autoDelay = 20.0f;

    private void Start()
    {
        handRect = hand.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (CanvasSystem.Instance._popupUICanvas.IsCanvas()) return;

        AutoHomeUI();
        AutoGameUI();
    }

    public void DisableTutorial()
    {
        hand.DOKill();
        hand.Hide();
    }    

    private void AutoHomeUI()
    {
        if (CanvasSystem.Instance.ScreenID != "HomeUICanvas") return;
        var complete = EventCheckingWithTime();
        if (complete)
        {
            var _btns = CanvasSystem.Instance._homeUICanvas._btnModes;
            for(int i = 0; i < _btns.Length; i++)
            {
                if (_btns[i].IsActive() == false) continue;
                if (RuntimeStorageData.Player.Modes.Contains(_btns[i].name) == false)
                {
                    hand.Show();
                    hand.position = _btns[i].FindChildByParent("Position").position.WithZ(0);
                    handRect.localPosition = handRect.localPosition.WithZ(0);
                    hand.DOKill();
                    hand.DOScale(0.8f, 0.3f).OnComplete(() => hand.DOScale(1.0f, 0.3f).OnComplete(() => hand.DOScale(0.8f, 0.3f).OnComplete(() => hand.DOScale(1.0f, 0.3f).OnComplete(() => DOVirtual.DelayedCall(0.3f, () => hand.Hide())))));
                    break;
                }    
            }    
        }
    }    



    private void AutoGameUI()
    {
        if (CanvasSystem.Instance.ScreenID != "GameUICanvas") return;
        var complete = EventCheckingWithTime();
        if (complete)
        {
            var baseObject = GameSpawn.Instance.IsReadyBaseObject();
            if (baseObject == null) return;

            var content = CanvasSystem.Instance._gameUICanvas.Content;
            int attempts = 0;
            int maxAttempts = content.childCount * 2;

            while (attempts < maxAttempts)
            {
                Transform randomChild = content.GetChild(Random.Range(0, content.childCount));
                attempts++;
                if (randomChild == null) continue;
                if (randomChild.name.EndsWith("_ads")) continue;
                if (randomChild.FindChildByParent("Icon").IsActive() == false) continue;

                hand.Show();
                hand.position = randomChild.position.WithZ(0);
                handRect.localPosition = handRect.localPosition.WithZ(0);
                hand.DOKill();
                hand.DOScale(0.8f, 0.3f).OnComplete(() =>
                {
                    hand.DOMove(baseObject.position.AddX(30), 0.7f).OnUpdate(() =>
                    {
                        handRect.localPosition = handRect.localPosition.WithZ(0);
                    }).OnComplete(() =>
                    {
                        handRect.localPosition = handRect.localPosition.WithZ(0);
                        hand.DOScale(1.0f, 0.3f).OnComplete(() => { DOVirtual.DelayedCall(0.3f, () => hand.Hide()); });
                    });
                });
                break;
            }
        }
    }    

    private bool EventCheckingWithTime()
    {
        _autoHandle += Time.deltaTime;
        if(_autoHandle > _autoDelay)
        {
            _autoHandle = 0;
            return true;
        }
        return false;
    }    
}
