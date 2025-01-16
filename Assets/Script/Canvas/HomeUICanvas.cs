using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeUICanvas : MonoBehaviour
{
    private void OnEnable()
    {
        Manager.Instance.IngameScreenID = "HomeUICanvas";
    }


    public void BtnSingle()
    {
        AdManager.Instance.ShowInterstitialHomeAd(() =>
        {
            CanvasSystem.Instance.ChooseScreen("GameUICanvas");
            CanvasSystem.Instance._gameUICanvas.CreateGame();
            SoundSpawn.Instance.Reload();
        });
    }    
}
