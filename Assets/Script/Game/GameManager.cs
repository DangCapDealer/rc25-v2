using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public enum GameStyle
    {
        Normal,
        Horror,
        Battle,
        Battle_Single,
        Monster,
        Monstrous
    }

    public enum GameState
    {
        Playing,
        Pause,
        Stop
    }    

    public GameStyle Style = GameStyle.Normal;
    public int NumberOfCharacter = 7;
    public GameSupport GameSupport;
    public GameState State = GameState.Stop;

    public GameStyle[] GameDefaults;
    public GameStyle[] GameCustoms;

    public bool IsGameDefault(GameStyle style) => GameDefaults.IsFound(style);
    public bool IsGameDefault() => IsGameDefault((GameStyle)Style);
    public bool IsGameCustom(GameStyle style) => GameCustoms.IsFound(style);
    public bool IsGameCustom() => IsGameCustom((GameStyle)Style);

    private IEnumerator Start()
    {
        if(Manager.Instance != null)
        {
            yield return new WaitUntil(() => Manager.Instance.IsLoading == false);
            Manager.Instance.IsIngame = true;
        }
        yield return null;

        MusicManager.Instance.PlaySound(Music.Main);
    }

    public void GameCreate()
    {
        GameSupport.OnCreateGame();
        State = GameState.Playing;
    }

    public void GameReset()
    {
        //State = GameState.Stop;
    }       
}
