using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingUICanvas : PopupCanvas
{
    public Transform Music;
    public Transform Sound;
    public Transform Vibration;

    private void Start()
    {
        var _childMusic = Music.FindChildByParent(Music.name);
        _childMusic.SetActive(RuntimeStorageData.Sound.isMusic);

        var _childSound = Sound.FindChildByParent(Sound.name);
        _childSound.SetActive(RuntimeStorageData.Sound.isSound);

        var _childVibration = Vibration.FindChildByParent(Vibration.name);
        _childVibration.SetActive(RuntimeStorageData.Sound.isVibrate);
    }
    
    public void BtnMusic()
    {
        RuntimeStorageData.Sound.isMusic = !RuntimeStorageData.Sound.isMusic;

        var _child = Music.FindChildByParent(Music.name);
        _child.SetActive(RuntimeStorageData.Sound.isMusic);
    }

    public void BtnSound()
    {
        RuntimeStorageData.Sound.isSound = !RuntimeStorageData.Sound.isSound;

        var _child = Sound.FindChildByParent(Sound.name);
        _child.SetActive(RuntimeStorageData.Sound.isSound);
    }

    public void BtnVibration()
    {
        RuntimeStorageData.Sound.isVibrate = !RuntimeStorageData.Sound.isVibrate;

        var _child = Vibration.FindChildByParent(Vibration.name);
        _child.SetActive(RuntimeStorageData.Sound.isVibrate);
    }    
}
