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
        if(Manager.Instance != null)
        {
            yield return new WaitUntil(() => Manager.Instance.IsLoading == false);
            Manager.Instance.HideLoading();
        }
        yield return null;
        Manager.Instance.IsIngame = true;
    }

    public void GameReset()
    {
        Style = GameStyle.Normal;
        NumberOfCharacter = 7;
    }    
}
