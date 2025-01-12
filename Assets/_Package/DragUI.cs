using UnityEngine;
using UnityEngine.EventSystems;

public class DragUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        GameEvent.OnUIDragDownMethod(this.transform.parent.name);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        GameEvent.OnUIDragMethod(this.transform.parent.name);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        canvasGroup.alpha = 1.0f;
        GameEvent.OnUIDragUpMethod(this.transform.parent.name);
    }
}
