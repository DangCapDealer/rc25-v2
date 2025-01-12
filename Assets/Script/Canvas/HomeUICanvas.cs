using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeUICanvas : MonoBehaviour
{
    public void BtnSingle()
    {
        CanvasSystem.Instance.ChooseScreen("GameUICanvas");
        CanvasSystem.Instance._gameUICanvas.CreateGame();
    }    
}
