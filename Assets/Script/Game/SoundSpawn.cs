using System;
using UnityEngine;

public class SoundSpawn : MonoSingleton<SoundSpawn>
{
    public GameObject SoundPrefab;

    void Start()
    {
        var totalCharacter = GameSpawn.Instance.CharacterData.Characters;
        for (int i = 0; i < totalCharacter.Length; i++)
        {
            if (totalCharacter[i].AudioClip == null)
            {
                Debug.LogError($"Audio clip missing {totalCharacter[i].ID}");
                continue;
            }

            var soundObject = PoolByID.Instance.GetPrefab(SoundPrefab, this.transform);
            soundObject.name = totalCharacter[i].ID;
            var script = soundObject.GetComponent<SoundPrefab>();
            script.Create(totalCharacter[i].AudioClip);
            var eventSound = soundObject.GetComponent<BeatDetection>();
            eventSound.CallBackFunction += CallBackFunction;
        }
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
