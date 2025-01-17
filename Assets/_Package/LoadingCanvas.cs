using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingCanvas : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] private Image progress;

    private Coroutine hideCorotine;
    public void Hide()
    {
        Debug.Log("[LoadingCanvas] Hide");
        this.canvasGroup.DOKill();
        this.canvasGroup.DOFade(0.0f, 0.3f).OnComplete(() => this.canvasGroup.transform.SetActive(false));
#if UNITY_EDITOR
        this.canvasGroup.transform.SetActive(false);
#endif
    }

    private Coroutine fillCoroutine;

    private IEnumerator FillImage(Action CALLBACK, float startValue = 0.0f, float endValue = 1.0f, float fillSpeed = 1.0f, float fillTime = 1.0f)
    {
        float elapsedTime = 0.0f;
        while (elapsedTime < fillTime)
        {
            float fillAmount = Mathf.Lerp(startValue, endValue, elapsedTime / fillTime);
            progress.fillAmount = fillAmount;
            elapsedTime += Time.deltaTime * fillSpeed;
            yield return null;
        }

        progress.fillAmount = endValue;
        yield return null;
        CALLBACK?.Invoke();
    }

    public void Show(Action CALLBACK, float startValue = 0.0f, float endValue = 1.0f, float fillSpeed = 1.0f, float fillTime = 1.0f)
    {
        Debug.Log("[LoadingCanvas] Show");
        if (fillCoroutine != null)
            StopCoroutine(fillCoroutine);

        fillCoroutine = StartCoroutine(FillImage(CALLBACK, startValue, endValue, fillSpeed, fillTime));
    }    
}
