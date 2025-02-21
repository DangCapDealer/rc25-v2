using UnityEngine;
using UnityEngine.EventSystems;

public class DragUI : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private bool isSelect = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isSelect = true;
        if (SoundSpawn.Instance.IsReady() == false)
            return;
        canvasGroup.alpha = 0.6f;
        GameEvent.OnUIDragDownMethod(this.transform.parent.name);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(isSelect == false) return;
        if (SoundSpawn.Instance.IsReady() == false)
            return;
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        GameEvent.OnUIDragMethod(this.transform.parent.name);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isSelect == false) return;
        isSelect = false;
        if (SoundSpawn.Instance.IsReady() == false)
            return;
        canvasGroup.alpha = 1.0f;
        GameEvent.OnUIDragUpMethod(this.transform.parent.name);
    }
}
