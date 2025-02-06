using System;
using UnityEngine;

public class SoundSpawn : MonoSingleton<SoundSpawn>
{
    public GameObject SoundPrefab;

    //void Start()
    //{
    //    var totalCharacter = GameSpawn.Instance.CharacterData.Characters;
    //    for (int i = 0; i < totalCharacter.Length; i++)
    //    {
    //        if (totalCharacter[i].AudioClip == null)
    //        {
    //            Debug.LogError($"Audio clip missing {totalCharacter[i].ID}");
    //            continue;
    //        }

    //        var soundObject = PoolByID.Instance.GetPrefab(SoundPrefab, this.transform);
    //        soundObject.name = totalCharacter[i].ID;
    //        var script = soundObject.GetComponent<SoundPrefab>();
    //        script.Create(totalCharacter[i].AudioClip);
    //        var eventSound = soundObject.GetComponent<BeatDetection>();
    //        eventSound.CallBackFunction += CallBackFunction;
    //    }
    //}

    public void CreateSound()
    {
        CoroutineUtils.PlayCoroutineHaftSecond(() =>
        {
            var totalCharacter = GameSpawn.Instance.CharacterData.Characters;
            for (int i = 0; i < totalCharacter.Length; i++)
            {
                if (totalCharacter[i].AudioClip == null)
                {
                    Debug.LogError($"Audio clip missing {totalCharacter[i].ID}");
                    continue;
                }

                var soundName = $"{totalCharacter[i].ID}_{GameManager.Instance.Style}";
                if (this.transform.FindChildByParent(soundName))
                    continue;
                var soundObject = PoolByID.Instance.GetPrefab(SoundPrefab, this.transform);
                soundObject.name = soundName;
                var script = soundObject.GetComponent<SoundPrefab>();
                switch (GameManager.Instance.Style)
                {
                    case GameManager.GameStyle.Normal:
                        script.Create(totalCharacter[i].AudioClip);
                        break;
                    case GameManager.GameStyle.Horror:
                        script.Create(totalCharacter[i].AudioClipHorror);
                        break;
                }

                var eventSound = soundObject.GetComponent<BeatDetection>();
                eventSound.CallBackFunction += CallBackFunction;
            }
        });
    }

    public SoundPrefab Find(string name)
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            var child = this.transform.GetChild(i);
            if (child.name == name)
                return child.GetComponent<SoundPrefab>();
        }
        return null;
    }

    public void MuteAll()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            var child = this.transform.GetChild(i);
            var script = child.GetComponent<SoundPrefab>();
            script.Mute = true;
            script.Reload();
        }
    }

    public void Reload()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            var child = this.transform.GetChild(i);
            var script = child.GetComponent<SoundPrefab>();
            script.Reload();
        }
    }

    public event Action<BeatDetection.EventInfo> _OnBeatDetection;
    public void CallBackFunction(BeatDetection.EventInfo eventInfo)
    {
        if (_OnBeatDetection != null)
            _OnBeatDetection?.Invoke(eventInfo);
    }
}
