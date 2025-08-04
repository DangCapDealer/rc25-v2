using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UI;

public partial class GameUICanvas : MonoBehaviour
{
    [Header("Mode 3")]
    public Transform Mode3ScoreObject;
    public Image Mode3ScoreImage;
    [Range(0, 2)] public float offScaleSpeed = 0.2f;
    public RectTransform Mode3TargetPosition;
    private float parentWidth = 0;
    public Vector2 Mode3GetTargetPosition() => Mode3TargetPosition.position.AddX(-30.0f).AddY(1.0f);

    public void Mode3Create()
    {
        if (GameManager.Instance.IsGameDefault())
        {
            Mode3ScoreObject.SetActive(false);
        }
        else if (GameManager.Instance.IsGameCustom())
        {
            Mode3ScoreObject.SetActive(true);
            parentWidth = Mode3ScoreObject.GetComponent<RectTransform>().sizeDelta.x;
            Mode3ScoreImage.fillAmount = 0.5f;
            //BtnAddTransform.SetActive(false);
        }
    }    

    public void Mode3UIReset()
    {
        Mode3ScoreImage.fillAmount = 0.5f;
    }

    public void Mode3Update()
    {
        if (GameManager.Instance.State != GameManager.GameState.Playing)
            return;
        // Mode battle thì nó sẽ có thêm điểm số show UI
        if (GameManager.Instance.Style == GameManager.GameStyle.Battle)
        {
            var speed = GameManager.Instance.GameSupport.Mode3GetSpeed();
            if (float.IsNaN(speed)) return;
            Mode3ScoreImage.fillAmount += speed * offScaleSpeed * Time.deltaTime;
            Mode3TargetPosition.anchoredPosition = Mode3TargetPosition.anchoredPosition.WithX(parentWidth * Mode3ScoreImage.fillAmount);
            if (Mode3ScoreImage.fillAmount >= 1 ||
                Mode3ScoreImage.fillAmount <= 0)
            {
                GameManager.Instance.State = GameManager.GameState.Pause;
                BackgroundDetection.Instance.Mode3Complete();
                CanvasSystem.Instance._popupUICanvas.ShowPopup(Popup.Complete);
            }
        }
    }    
}
