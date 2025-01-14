using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPrefab : MonoBehaviour
{
    private AudioClip AudioClip;
    private AudioSource AudioSource;

    public void Create(AudioClip _clip)
    {
        AudioClip = _clip;
        AudioSource = this.gameObject.GetComponent<AudioSource>();

        AudioSource.mute = true;
        AudioSource.clip = _clip;
        AudioSource.loop = true;
        AudioSource.Play();
    }    

    public bool Mute
    {
        get { return AudioSource.mute; }
        set {  AudioSource.mute = value; }
    }
}
