using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    public enum GameStyle
    {
        Normal,
        Horror
    }

    public GameStyle Style = GameStyle.Normal;
    public int NumberOfCharacter = 7;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => Manager.Instance.IsLoading == false);
        yield return WaitForSecondCache.WAIT_TIME_ONE;

        Manager.Instance.HideLoading();
    }

    public void GameReset()
    {
        Style = GameStyle.Normal;
        NumberOfCharacter = 7;
    }    
}
