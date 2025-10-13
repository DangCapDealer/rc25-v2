using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeUICanvas : MonoBehaviour
{
    public Transform _btnProductRemoveAd;
    public Transform[] _btnModes;

    private void Start()
    {
#if INAPP
        OnIAPurechase("", "");
#endif
    }

    private void OnEnable()
    {
        if (Manager.Instance != null)
            Manager.Instance.IngameScreenID = "HomeUICanvas";

        GameEvent.OnIAPurchase += OnIAPurechase;
    }

    private void OnDisable()
    {
        GameEvent.OnIAPurchase -= OnIAPurechase;
    }

    private void OnIAPurechase(string productID, string action)
    {
        if (RuntimeStorageData.Player.IsProductId(InappController.Instance.GetProductIdByIndex(0)) &&
            RuntimeStorageData.Player.IsProductId(InappController.Instance.GetProductIdByIndex(1)))
        {
            _btnProductRemoveAd.SetActive(false);
        }
        else
        {
            _btnProductRemoveAd.SetActive(true);
        }
    }

    //vào game thôi
    private void StartGame(GameManager.GameStyle style, int numberOfCharacters, string logEvent, string modeName)
    {
        StaticVariable.ClearLog();

        //CanvasSystem.Instance._loadingUICanvas.ShowLoading();

        MusicManager.Instance.PauseSound();
        GameManager.Instance.Style = style;
        BackgroundDetection.Instance.SettingBackground();
        TutorialSystem.Instance?.DisableTutorial();

        CanvasSystem.Instance._loadingUICanvas.ShowLoading(() =>
        {
            AdManager.Instance.ShowInterstitialHomeAd(() =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    GameManager.Instance.NumberOfCharacter = numberOfCharacters;
                    GameManager.Instance.GameCreate();
                    SoundSpawn.Instance.CreateSound();
                    SoundSpawn.Instance.Reload();
                    CanvasSystem.Instance.ChooseScreen("GameUICanvas");
                    CanvasSystem.Instance._gameUICanvas.CreateGame();
                    CanvasSystem.Instance.ShowNativeCollapse();
                });
            }, () =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(AdManager.Instance.ShowNativeOverlayAd);
            });
        });

        FirebaseManager.Instance.LogEvent(logEvent);
        RuntimeStorageData.Player.Modes.Add(modeName);
    }

    // sự kiện các button
    public void BtnSingle() => StartGame(GameManager.GameStyle.Normal, 8, "Mode_Beat_1", "SingleNormalBeat - 1");
    public void BtnSingleHorror() => StartGame(GameManager.GameStyle.Horror, 8, "Mode_Beat_2", "SingleHorrorBeat - 2");
    public void BtnSingleHuman() => StartGame(GameManager.GameStyle.Battle_Single, 8, "Mode_Beat_3", "SingleHumanBeat - 3");
    public void BtnBatteBeat() => StartGame(GameManager.GameStyle.Battle, 10, "Mode_Battle", "BattleBeat_Solo");
    public void BtnSingleMonster() => StartGame(GameManager.GameStyle.Monster, 8, "Mode_Beat_4", "SingleMonsterBeat - 4");
    public void BtnSingleMonstrous() => StartGame(GameManager.GameStyle.Monstrous, 8, "Mode_Beat_5", "SingleMonsterBeat - 5");
    public void BtnSingleItalianBrainrot() => StartGame(GameManager.GameStyle.ItalianBrainrot, 8, "Mode_Beat_5", "SingleMonsterBeat - 6");
    public void BtnSingleKpop() => StartGame(GameManager.GameStyle.Kpop, 8, "Mode_Beat_6", "SingleKpopBeat - 7");
    public void BtnSetting() { CanvasSystem.Instance._popupUICanvas.ShowPopup(Popup.Setting); }    
    public void BtnCheckin() { CanvasSystem.Instance._popupUICanvas.ShowPopup(Popup.Checkin); }    
    public void BtnNoAds() { CanvasSystem.Instance.ShowNoAd(); }

    public void BtnRate()
    {
#if UNITY_ANDROID
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.sprank.horror.beats.studio.battle");
#elif UNITY_IOS
            Application.OpenURL("itms-apps://itunes.apple.com/app/id1234567890");
#else
            Debug.Log("Rate Us is not supported on this platform.");
#endif
    }
}
