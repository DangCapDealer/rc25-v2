using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSpawn : MonoSingleton<SoundSpawn>
{
    private bool isCreateSound = false;
    public bool IsReady() => isCreateSound;
    public GameObject SoundPrefab;

    public void CreateSound()
    {
        isCreateSound = false;
        StartCoroutine(createSoundCoroutine());
    }

    private IEnumerator createSoundCoroutine()
    {
        yield return WaitForSecondCache.WAIT_TIME_HAFT;
        var totalCharacter = GameSpawn.Instance.CharacterData.Characters;
        foreach (var character in totalCharacter)
        {
            if (character.AudioClipNormal == null)
            {
                Debug.LogError($"[SoundSpawn] Audio Missing {character.ID}");
                continue;
            }

            // Cái này không ảnh hưởng khi thêm mode mới
            // Có thể sẽ ảnh hưởng khi thêm 1 mode battle
            var styles = new List<GameManager.GameStyle>();
            if (GameManager.Instance.IsGameDefault())
            {
                if (GameManager.Instance.Style == GameManager.GameStyle.Battle_Single)
                {
                    styles.Add(GameManager.GameStyle.Battle);
                }
                else
                {
                    styles.Add(GameManager.Instance.Style);
                }
            }    
            else if (GameManager.Instance.IsGameCustom())
            {
                styles.AddRange(new[] { GameManager.GameStyle.Normal, GameManager.GameStyle.Battle });
            }
            else Debug.Log("Game chưa được cài đặt sound");

            foreach (var style in styles)
            {
                var soundName = $"{character.ID}_{style}";
                if (!this.transform.IsChild(soundName))
                {
                    var soundObject = PoolByID.Instance.GetPrefab(SoundPrefab, this.transform);
                    soundObject.name = soundName;
                    var script = soundObject.GetComponent<SoundPrefab>();
                    script.Create(character.GetAudioClip(style));
                    var eventSound = soundObject.GetComponent<BeatDetection>();
                    eventSound.CallBackFunction += CallBackFunction;
                }
            }
            yield return null;
        }
        yield return null;
        Reload();
        isCreateSound = true;
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
