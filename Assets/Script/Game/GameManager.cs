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
}
