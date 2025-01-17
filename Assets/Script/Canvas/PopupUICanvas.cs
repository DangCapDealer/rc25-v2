using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupUICanvas : MonoBehaviour
{
    public PopupCanvas[] PopupCanvas;

    public void ShowPopup(Popup popup)
    {
        for (int i = 0; i < PopupCanvas.Length; i++)
        {
            PopupCanvas[i].Show(popup);
        }
    }
}
