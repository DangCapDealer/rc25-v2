using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Import DOTween namespace
using System; // Required for Action

public class LoadingCanvas : MonoBehaviour
{
    public CanvasGroup canvasGroup; // Assign this in the Inspector
    public Image progress; // Assign this in the Inspector

    // No need for hideCoroutine or fillCoroutine when using DOTween's tweening system directly

    /// <summary>
    /// Hides the loading canvas with a fade-out animation using DOTween.
    /// </summary>
    public void Hide()
    {
        Debug.Log("[LoadingCanvas] Hide");
        // Kill any existing DOTween tweens on this canvasGroup to prevent conflicts
        this.canvasGroup.DOKill(true);
        // Fade out the canvasGroup to 0 opacity over 0.3 seconds
        // On completion, deactivate the GameObject associated with the canvasGroup
        this.canvasGroup.DOFade(0.0f, 0.3f).OnComplete(() => this.canvasGroup.gameObject.SetActive(false));

        // The #if UNITY_EDITOR block might be redundant if the DOTween animation handles the immediate deactivation
        // in editor as well. If you still need it for specific editor-only behavior, keep it.
        // For most cases, the OnComplete callback is sufficient.
#if UNITY_EDITOR
        // If you want it to disappear instantly in the editor *before* the tween finishes, uncomment this:
        // this.canvasGroup.gameObject.SetActive(false); 
#endif
    }

    /// <summary>
    /// Shows the loading canvas and fills an image based on the provided parameters using DOTween.
    /// </summary>
    /// <param name="callback">Action to invoke when the fill animation is complete.</param>
    /// <param name="startValue">The starting fill amount (0.0 to 1.0).</param>
    /// <param name="endValue">The ending fill amount (0.0 to 1.0).</param>
    /// <param name="fillTime">The duration of the fill animation in seconds.</param>
    public void Show(Action callback, float startValue = 0.0f, float endValue = 1.0f, float fillTime = 1.0f)
    {
        Debug.Log("[LoadingCanvas] Show");

        // Ensure the canvas is active before starting animations
        this.canvasGroup.gameObject.SetActive(true);
        // Ensure canvasGroup is visible (alpha is 1) at the start of the show animation, 
        // in case it was previously faded out.
        this.canvasGroup.alpha = 1.0f;

        // Kill any existing DOTween tweens on the progress image to prevent conflicts
        this.progress.DOKill(true);

        // Set the initial fill amount
        this.progress.fillAmount = startValue;

        // Animate the fillAmount of the progress image
        // No need for fillSpeed, fillTime directly dictates the duration.
        this.progress.DOFillAmount(endValue, fillTime)
            .SetEase(Ease.Linear) // You can choose different easing functions
            .OnComplete(() => callback?.Invoke()); // Invoke callback when animation is complete
    }

    // You might want to add Awake or Start to initialize canvasGroup and progress if they are not
    // assigned directly in the Inspector and you want to find them via code.
    private void Awake()
    {
        // Example: If canvasGroup and progress are not assigned in inspector
        // canvasGroup = GetComponent<CanvasGroup>();
        // progress = GetComponentInChildren<Image>(); // Adjust if your hierarchy is different
    }
}