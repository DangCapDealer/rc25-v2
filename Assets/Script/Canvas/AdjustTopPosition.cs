using UnityEngine;

public class AdjustImagePosition : MonoBehaviour
{
    public float rate = 1.0f / 3.0f;
    public float minHeight = 300.0f;
    public RectTransform rectTransform;

    private void OnEnable()
    {
        if (GameManager.Instance.NumberOfCharacter > 10)
            rate = 0.23f;
        else
            rate = 0.33f;

        onCaculateTabHeight();
    }

    private void onCaculateTabHeight()
    {
        if (rectTransform != null)
        {
            Debug.Log(Screen.height);
            float screenHeight = Screen.height;
            float imageHeight = screenHeight * rate;
            if (imageHeight < minHeight)
                imageHeight = minHeight;
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
