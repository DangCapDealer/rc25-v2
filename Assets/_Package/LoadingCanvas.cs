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
    public void Show()
    {
        this.transform.SetActive(true);
        progress.fillAmount = 0;
        this.canvasGroup.transform.SetActive(true);
        this.canvasGroup.DOKill();
        this.canvasGroup.DOFade(1f, 0.5f).OnComplete(() =>
        {
            StartFilling(() => Manager.Instance.IsLoading = false);
        });
    }

    public void Hide()
    {
        this.canvasGroup.DOKill();
        this.canvasGroup.DOFade(0.0f, 0.5f).OnComplete(() => this.canvasGroup.transform.SetActive(false));
    }

    private Coroutine fillCoroutine;
    private void StartFilling(Action CALLBACK)
    {
        if (fillCoroutine != null)
        {
            StopCoroutine(fillCoroutine);
        }

        fillCoroutine = StartCoroutine(FillImage(CALLBACK));
    }

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
        if (fillCoroutine != null)
        {
            StopCoroutine(fillCoroutine);
        }

        fillCoroutine = StartCoroutine(FillImage(CALLBACK, startValue, endValue, fillSpeed, fillTime));
    }    
}
