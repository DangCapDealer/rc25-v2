using DG.Tweening;
using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameLoadingUICanvas : MonoBehaviour
{
    public CanvasGroup _canvasGroup;
    public Transform _content;

    public void ShowLoading(UnityAction Callback)
    {
        _canvasGroup.alpha = 0;
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            _content.Show();
            _canvasGroup.DOFade(1.0f, 0.1f).OnComplete(() =>
            {
                DOVirtual.DelayedCall(1.2f, () =>
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() => _content.Hide());
                        _canvasGroup.DOFade(0.0f, 0.2f);
                        Callback?.Invoke();
                    });
                });
            });
        });
    }    
}
