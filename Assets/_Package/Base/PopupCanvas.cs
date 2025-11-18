using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PopupCanvas : MonoBehaviour
{
    public bool IsActive = false;
    public Popup popup;
    public PopupAnimation popupAnimation;
    public Transform panel;

    protected UnityAction onCloseAction;

    public virtual void Show(Popup p)
    {
        if (popup == p)
        {
            if (IsActive == true)
                return;
            IsActive = true;
            onCloseAction = null;
            this.gameObject.SetActive(true);

            showAnimation();
        }
        else
        {
            Hide();
        }
    }

    public virtual void Hide()
    {
        if (!IsActive)
            return;
        IsActive = false;
        hideAnimation();
    }   


    
    private void showAnimation()
    {
        switch (popupAnimation)
        {
            case PopupAnimation.Punch:
                this.panel.localScale = Vector3.one;
                this.panel.DOKill();
                this.panel.DOPunchScale(Vector3.one * 0.1f, 0.25f, 2, 0.2f).SetEase(Ease.OutBack);
                break;
            case PopupAnimation.MoveToUp:
                var rect = this.panel.GetComponent<RectTransform>();
                rect.anchoredPosition = rect.anchoredPosition.WithY(-1200);
                rect.DOKill();
                rect.DOAnchorPosY(0, 0.3f);
                break;
            case PopupAnimation.MoveToRight:
                var rectRight = this.panel.GetComponent<RectTransform>();
                rectRight.anchoredPosition = rectRight.anchoredPosition.WithX(1200);
                rectRight.DOKill();
                rectRight.DOAnchorPosX(0, 0.3f);
                break;
        }
    }

    private void hideAnimation()
    {
        switch (popupAnimation)
        {
            case PopupAnimation.Punch:
                this.panel.DOKill();
                this.panel.DOScale(Vector3.zero, 0.25f).OnComplete(() =>
                {
                    onCloseAction?.Invoke();
                    onCloseAction = null;
                    this.gameObject.SetActive(false);
                });
                break;
            case PopupAnimation.MoveToUp:
                var rect = this.panel.GetComponent<RectTransform>();
                rect.DOKill();
                rect.DOAnchorPosY(-1200, 0.3f).OnComplete(() =>
                {
                    onCloseAction?.Invoke();
                    onCloseAction = null;
                    this.gameObject.SetActive(false);
                });
                break;
            default:
                onCloseAction?.Invoke();
                onCloseAction = null;
                this.gameObject.SetActive(false);
                break;
        }
    }
}
