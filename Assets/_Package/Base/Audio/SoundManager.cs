using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public enum Sound
{
    None,
    OpenDoor,
    CloseDoor,
    OpenCabinet,
    CloseCabinet,
    SwitchOn,
    SwitchOff,
    TomFootstep,
    MomFootstep,
    DadFootstep,
    MomOrDadRun,
    MomOrDadSlap,
    Click,
    RubixRotation,
    PaitingMove,
    ElictricSafeButton,
    StartComputer,
    StartWindows,
    ClockRotation,
    ClockRing,
    MosaicSlide,
    ScrewPattern,
    OpenChest,
    CallSuccessed,
    CallFailed,
    PizzaSuccessed,
    DoorRing,
    DoorFailed,
    PizzaRing,
    SeassionFlow,
    DogBraking,
    Claim,
    OPEN_StartStories,
    OPEN_EndStories,
    OPEN_FriendTalk01,
    OPEN_FriendTalk02,
    OPEN_TomTalk01,
    OPEN_TomTalk02,
    OPEN_MonTalk01,
    OPEN_MonTalk02,
    OPEN_DadTalk01,
    Pull_Curtain,
    Open_Save,
    Close_Save,
    Creaking_Windows
}

public class SoundManager : MonoSingletonGlobal<SoundManager>
{
    [System.Serializable]
    public class SoundTable
    {
        public Sound sound;
        public AudioClip clip;
    }

    //private Coroutine corChangeVolume;
    [SerializeField] AudioSource audioSourceNormal;
    [SerializeField] AudioSource audioSourceSpecial;

    public float _audioVolume = 1.0f;

    [SerializeField] private SoundTable[] sounds;
    private Dictionary<Sound, AudioClip> soundDics = new Dictionary<Sound, AudioClip>();
    private Queue<Audio3D> queue3d = new Queue<Audio3D>();

    protected override void Awake()
    {
        base.Awake();
        foreach (var _s in sounds)
        {
            if (soundDics.ContainsKey(_s.sound) == false)
                soundDics.Add(_s.sound, _s.clip);
            else
                Debug.Log($"sound dics container keys: {_s.sound}");
        }
    }

    private void Start()
    {
        _audioVolume = 1.0f;
        audioSourceNormal.volume = _audioVolume;
        audioSourceSpecial.volume = _audioVolume;

        queue3d.Clear();
    }

    public void Play(AudioClip audioClip, bool isActive = true, bool isLoop = false)
    {
        if (isActive)
        {
            DisableLoopSound();
            if (audioSourceNormal.isPlaying)
                audioSourceNormal.Stop();

            audioSourceNormal.time = 0;
            audioSourceNormal.loop = isLoop;
            audioSourceNormal.clip = audioClip;
            audioSourceNormal.Play();
        }
        else
            Stop();
    }

    public void PlaySpecial(AudioClip audioClip, bool isActive = true, bool isLoop = false)
    {
        if (isActive)
        {
            DisableLoopSound();
            if (audioSourceSpecial.isPlaying)
                audioSourceSpecial.Stop();

            audioSourceSpecial.time = 0;
            audioSourceSpecial.loop = isLoop;
            audioSourceSpecial.clip = audioClip;
            audioSourceSpecial.Play();
        }
        else
            audioSourceSpecial.Stop();
    }

    public void VolumeChange(float toVolume)
    {
        float volume = audioSourceNormal.volume;
        _audioVolume = toVolume;
        DOTween.To(() => volume, x => volume = x, toVolume, 1f)
        .OnUpdate(() => {
            audioSourceNormal.volume = volume;
            audioSourceSpecial.volume = volume;
        });
    }    


    private Coroutine LoopSound;

    private void DisableLoopSound()
    {
        if (LoopSound != null)
            StopCoroutine(LoopSound);
    }

    public void PlaySoundLoopWithTime(AudioClip audioClip, float startTime = 2f, float endTime = 4f)
    {
        DisableLoopSound();
        audioSourceNormal.loop = false;
        audioSourceNormal.clip = audioClip;
        audioSourceNormal.time = startTime;
        audioSourceNormal.Play();

        LoopSound = CoroutineUtils.PlayCoroutine(() =>
        {
            PlaySoundLoopWithTime(audioClip, startTime, endTime);
        }, endTime);
    }

    public void PlayOnShot(Sound sound, float volume = 1f)
    {
        AudioClip clip = ConvertToClip(sound);
        audioSourceNormal.PlayOneShot(clip);
    }

    public void PlayOnShot(AudioClip sound, float volume = 1f)
    {
        audioSourceNormal.PlayOneShot(sound);
    }

    public void PlayOneShotSpecial(Sound sound, float volume = 1f)
    {
        AudioClip clip = ConvertToClip(sound);
        audioSourceSpecial.PlayOneShot(clip);
    }
        

    public IEnumerator PlayOnShotCustom(Sound sound, float volume = 1f, float after = 0, int numberPlay = 1)
    {
        yield return WaitForSecondCache.GetWFSCache(after);
        for (int i = 0; i < numberPlay; i++)
        {
            AudioClip clip = ConvertToClip(sound);
            audioSourceNormal.PlayOneShot(clip);

            yield return WaitForSecondCache.GetWFSCache(clip.length);
        }
    }

    public void PlayLoopInfinity(Sound sound)
    {
        AudioClip clip = ConvertToClip(sound);
        audioSourceNormal.clip = clip;
        audioSourceNormal.Play();
    }

    public void Stop()
    {
        audioSourceNormal.clip = null;
        audioSourceNormal.Stop();
        DisableLoopSound();
    }

    public void PlaySoundWithPlaying(Sound sound)
    {
        if (audioSourceNormal.isPlaying == true)
            return;

        CoroutineUtils.PlayCoroutine(() =>
        {
            AudioClip clip = ConvertToClip(sound);
            audioSourceNormal.clip = clip;
            audioSourceNormal.Play();
        }, 0.2f);
    }    

    public void PlaySoundAsync(Sound sound)
    {
        if (!isPlayingAsync)
        {
            AudioClip clip = ConvertToClip(sound);
            float length = GetSoundLength(sound);
            StartCoroutine(PlaySoundWithUpdate(clip, length));
        }
    }

    public void PlaySoundAsyncWithDelay(Sound sound, float delay)
    {
        if (!isPlayingAsync)
        {
            AudioClip clip = ConvertToClip(sound);
            float length = GetSoundLength(sound);
            StartCoroutine(PlaySoundWithUpdate(clip, length + delay));
        }
    }

    private bool isPlayingAsync = false;
    IEnumerator PlaySoundWithUpdate(AudioClip clip, float length, float volume = 1f)
    {
        isPlayingAsync = true;
        audioSourceNormal.PlayOneShot(clip);
        yield return WaitForSecondCache.GetWFSCache(length);
        isPlayingAsync = false;
    }

    public void PlaySoundWithCounter(Sound sound, int counter)
    {
        AudioClip clip = ConvertToClip(sound);
        float length = GetSoundLength(sound);
        StartCoroutine(PlaySoundWithDelay(clip, length, counter));
    }

    IEnumerator PlaySoundWithDelay(AudioClip clip, float sLength, int counter)
    {
        int t = 0;
        while (t < counter)
        {
            audioSourceNormal.PlayOneShot(clip);
            t += 1;
            yield return WaitForSecondCache.GetWFSCache(sLength);
        }
    }

    public AudioClip ConvertToClip(Sound sound)
    {
        if (soundDics.ContainsKey(sound))
            return soundDics[sound];
        return null;
    }

    public void Turn(bool isEnable)
    {
        audioSourceSpecial.mute = !isEnable;
        audioSourceNormal.mute = !isEnable;
    }

    public float GetSoundLength(Sound sound)
    {
        AudioClip clip = ConvertToClip(sound);
        return clip.length;
    }

    public IEnumerator PlayOnShotSpecial(Sound sound, float volume = 1f, float after = 0, int numberPlay = 1)
    {
        yield return WaitForSecondCache.GetWFSCache(after);
        for (int i = 0; i < numberPlay; i++)
        {
            AudioClip clip = ConvertToClip(sound);
            audioSourceSpecial.PlayOneShot(clip);
            yield return WaitForSecondCache.GetWFSCache(clip.length);
        }
    }

    public void PlayLoopSpecial(Sound sound)
    {
        AudioClip clip = ConvertToClip(sound);
        audioSourceSpecial.clip = clip;
        audioSourceSpecial.loop = false;
        audioSourceSpecial.Play();
    }

    public void PlayLoopSpecial(AudioClip clip)
    {
        audioSourceSpecial.clip = clip;
        audioSourceSpecial.loop = true;
        audioSourceSpecial.Play();
    }

    public void StopSpecial()
    {
        audioSourceSpecial.clip = null;
        audioSourceSpecial.Stop();
    }

    public void PlaySound(Sound id, float volumeMultiply = 1)
    {
        if (id == Sound.None)
            return;
        volumeMultiply *= _audioVolume;
        PlayOnShot(id, volumeMultiply);
    }

    public void PlaySoundAtLocation(Sound id, Vector3 worldPosition, float volumeMultiply = 1)
    {
        if (id == Sound.None)
            return;
        volumeMultiply *= _audioVolume;
        if (queue3d.Count == 0)
        {
            var _obj = new GameObject("AudioSource", typeof(Audio3D), typeof(AudioSource));
            _obj.transform.parent = transform;
            queue3d.Enqueue(_obj.GetComponent<Audio3D>());
        }

        var clip = ConvertToClip(id);
        if (clip == null)
            return;
        var audio3D = queue3d.Dequeue();
        audio3D.SpawnAudio3D(clip, worldPosition, volumeMultiply, () => { queue3d.Enqueue(audio3D); });
    }
}
