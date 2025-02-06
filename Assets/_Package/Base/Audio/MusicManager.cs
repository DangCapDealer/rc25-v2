using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Music
{
    None,
    Main
}

public class MusicManager : MonoSingletonGlobal<MusicManager>
{
    [System.Serializable]
    public class MusicTable
    {
        public Music music;
        public AudioClip clip;
    }

    [SerializeField] private MusicTable[] musics;
    [SerializeField] AudioSource audioSource;
    private Dictionary<Music, AudioClip> musicDics = new Dictionary<Music, AudioClip>();

    public float _musicVoluime = 1;

    protected override void Awake()
    {
        base.Awake();
        foreach (var _s in musics)
        {
            musicDics.Add(_s.music, _s.clip);
        }
    }

    private IEnumerator Start()
    {
        if (musics.Length == 0)
            yield break;
    }

    public void PlaySound(Music sound, float _volume = 1.0f)
    {
        _volume *= _musicVoluime;

        PauseSound();
        audioSource.clip = ConverToClip(sound);
        audioSource.loop = true;
        audioSource.volume = _volume;
        audioSource.Play();
    }

    public void PauseSound()
    {
        audioSource.Pause();
    }

    public void UnPauseSound()
    {
        audioSource.UnPause();
    }

    AudioClip ConverToClip(Music sound)
    { 
        if (musicDics.ContainsKey(sound))
            return musicDics[sound];
        return null;
    }

    public void Turn(bool isEnble)
    {
        audioSource.mute = !isEnble;
    }

    public void VolumeChange(float toVolume)
    {
        float volume = audioSource.volume;
        _musicVoluime = toVolume;
        DOTween.To(() => volume, x => volume = x, toVolume, 3f)
        .OnUpdate(() => {
            audioSource.volume = volume;
        });
    }
}
