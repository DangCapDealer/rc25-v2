using UnityEngine;

public class AdjustImagePosition : MonoBehaviour
{
    //public enum AdjustType
    //{
    //    Rate,
    //    Pixel
    //}    

    //public AdjustType type = AdjustType.Rate;
    public float rate = 1.0f / 3.0f;
    public RectTransform rectTransform;

    void Start()
    {
        if (rectTransform != null)
        {
            // Lấy chiều cao của màn hình
            float screenHeight = Screen.height;

            // Tính toán chiều cao của hình ảnh
            float imageHeight = screenHeight * rate;

            // Đặt anchor để stretch theo chiều ngang và định vị ở dưới cùng
            rectTransform.anchorMin = new Vector2(0, 0); // bottom-left corner
            rectTransform.anchorMax = new Vector2(1, 0); // bottom-right corner

            // Đặt pivot ở giữa để dễ xử lý
            rectTransform.pivot = new Vector2(0.5f, 0.5f);

            // Đặt chiều cao của hình ảnh và offset
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, imageHeight);
            rectTransform.anchoredPosition = new Vector2(0, imageHeight / 2f); // Đặt vị trí y để phù hợp
        }
        else
        {
            Debug.LogError("RectTransform chưa được gán.");
        }
    }
}
