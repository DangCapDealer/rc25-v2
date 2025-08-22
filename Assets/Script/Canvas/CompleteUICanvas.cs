using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteUICanvas : PopupCanvas
{
    public void BtnHome()
    {
        CanvasSystem.Instance._loadingUICanvas.ShowLoading(() =>
        {
            AdManager.Instance.ShowInterstitialHomeAd(() =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    CanvasSystem.Instance.ChooseScreen("HomeUICanvas");
                    CanvasSystem.Instance.AutoNoAd();
                    GameManager.Instance.GameReset();
                    GameSpawn.Instance.RemoveAllCharacter();
                    SoundSpawn.Instance.MuteAll();
                    MusicManager.Instance.PlaySound(Music.Main);
                    Hide();
                });
            }, () =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(AdManager.Instance.ShowNativeOverlayAd);
            });
        });
    }

    public void BtnReplay()
    {
        CanvasSystem.Instance._loadingUICanvas.ShowLoading(() =>
        {
            AdManager.Instance.ShowInterstitialHomeAd(() =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    GameSpawn.Instance.RemoveAllCharacter();
                    SoundSpawn.Instance.MuteAll();
                    GameManager.Instance.State = GameManager.GameState.Playing;
                    GridInCamera.Instance.CreatePosition();
                    CanvasSystem.Instance._gameUICanvas.Mode3UIReset();
                    GameEvent.OnUIThemeMethod(GameManager.Instance.Style.ToString());
                    Hide();
                });
            }, () =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(AdManager.Instance.ShowNativeOverlayAd);
            });
        });
    }
}
