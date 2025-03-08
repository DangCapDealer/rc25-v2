using UnityEngine;

public class AdjustImagePosition : MonoBehaviour
{
    public float gameCameraPositionY = 0.85f;
    public RectTransform rectTransform;
    public Transform _target;
    public RectTransform canvasRect;

    private void OnEnable()
    {
        if (GameManager.Instance.NumberOfCharacter > 10)
            _target.position = _target.position.WithY(-1.75f);
        else
            _target.position = _target.position.WithY(0.0f);

        onCaculateTabHeight();
    }

    private void onCaculateTabHeight()
    {
        if (rectTransform != null)
        {
            var screenPos = Camera.main.WorldToScreenPoint(_target.position.AddY(-gameCameraPositionY));
            Vector2 _targetUIPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, Camera.main, out _targetUIPoint);
            _targetUIPoint = _targetUIPoint.WithX(0);
            Vector2 bottomPosition = new Vector2(0, -canvasRect.sizeDelta.y / 2);
            var distance = Vector2.Distance(bottomPosition, _targetUIPoint);
            rectTransform.sizeDelta = Vector2.zero.WithX(rectTransform.sizeDelta.x).WithY(distance);
        }
        else
        {
            Debug.LogError("RectTransform chưa được gán.");
        }
    }    
}
