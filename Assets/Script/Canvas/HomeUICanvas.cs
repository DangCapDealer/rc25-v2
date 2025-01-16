using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeUICanvas : MonoBehaviour
{
    private void OnEnable()
    {
        if (Manager.Instance != null)
            Manager.Instance.IngameScreenID = "HomeUICanvas";
    }


    public void BtnSingle()
    {
        if(AdManager.Instance == null)
        {
            CanvasSystem.Instance.ChooseScreen("GameUICanvas");
            CanvasSystem.Instance._gameUICanvas.CreateGame();
            SoundSpawn.Instance.Reload();
        }   
        else
        {
            AdManager.Instance.ShowInterstitialHomeAd(() =>
            {
                CanvasSystem.Instance.ChooseScreen("GameUICanvas");
                CanvasSystem.Instance._gameUICanvas.CreateGame();
                SoundSpawn.Instance.Reload();
            });
        }    
    }    
}
