using UnityEngine;

public class AdjustImagePosition : MonoBehaviour
{
    public float rate = 1.0f / 3.0f;
    public RectTransform rectTransform;

    private void OnEnable()
    {
        if (GameManager.Instance.Style == GameManager.GameStyle.Normal ||
            GameManager.Instance.Style == GameManager.GameStyle.Horror)
            rate = 0.33f;
        else if (GameManager.Instance.Style == GameManager.GameStyle.Battle)
            rate = 0.23f;

        onCaculateTabHeight();
    }

    private void onCaculateTabHeight()
    {
        if (rectTransform != null)
        {
            float screenHeight = Screen.height;
            float imageHeight = screenHeight * rate;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.zero.WithX(1);
            rectTransform.pivot = Vector2.zero.WithX(0.5f).WithY(0.5f);
            rectTransform.sizeDelta = Vector2.zero.WithX(rectTransform.sizeDelta.x).WithY(imageHeight);
            rectTransform.anchoredPosition = Vector2.zero.WithY(imageHeight / 2f);
        }
        else
        {
            Debug.LogError("RectTransform chưa được gán.");
        }
    }    
}
