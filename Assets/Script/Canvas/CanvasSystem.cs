using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSystem : MonoSingleton<CanvasSystem>
{
    public HomeUICanvas _homeUICanvas;
    public GameUICanvas _gameUICanvas;
    public PopupUICanvas _popupUICanvas;

    public GameObject[] _screenUICanvas;

    private void Start()
    {
        ChooseScreen("HomeUICanvas");
    }

    public void ChooseScreen(string name)
    {
        _screenUICanvas.SimpleForEach(x =>
        {
            if(x.name == name)
            {
                x.SetActive(true);
            }
            else
            {
                x.SetActive(false);
            } 
               
        });
    }      
}
