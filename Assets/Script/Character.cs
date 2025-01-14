using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public AudioClip AudioClip;
    public AudioSource AudioSource;

    public void CreateCharacter()
    {
        AudioSource.clip = AudioClip;
        AudioSource.loop = true;
        AudioSource.Play();
    }
}
