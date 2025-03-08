using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockOnceUICanvas : PopupCanvas
{
    public void ShowAd()
    {
        if (GameManager.Instance.NumberOfCharacter >= 10)
        {
            base.Hide();
        }
        else
        {
            AdManager.Instance.ShowRewardedThridAd(() =>
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    GameManager.Instance.NumberOfCharacter += 1;
                    GameSpawn.Instance.CreateNewPositionCharacter();
                    base.Hide();
                });
            });
        }
    }
}
