using System;
using System.Collections;
using UnityEngine;

public class SoundSpawn : MonoSingleton<SoundSpawn>
{
    private bool isCreateSound = false;
    public bool IsReady() => isCreateSound;
    public GameObject SoundPrefab;

    public void CreateSound()
    {
        isCreateSound = false;
        StartCoroutine(createSoundCorotine());
    }

    private IEnumerator createSoundCorotine()
    {
        yield return WaitForSecondCache.WAIT_TIME_HAFT;
        var totalCharacter = GameSpawn.Instance.CharacterData.Characters;
        for (int i = 0; i < totalCharacter.Length; i++)
        {
            if (totalCharacter[i].AudioClipNormal == null)
            {
                Debug.LogError($"[SoundSpawn] Audio Missing {totalCharacter[i].ID}");
                continue;
            }

            if (GameManager.Instance.Style == GameManager.GameStyle.Normal ||
                GameManager.Instance.Style == GameManager.GameStyle.Horror)
            {
                var soundName = $"{totalCharacter[i].ID}_{GameManager.Instance.Style}";
                if (this.transform.FindChildByParent(soundName))
                    continue;
                var soundObject = PoolByID.Instance.GetPrefab(SoundPrefab, this.transform);
                soundObject.name = soundName;
                var script = soundObject.GetComponent<SoundPrefab>();
                script.Create(totalCharacter[i].GetAudioClip(GameManager.Instance.Style));
                var eventSound = soundObject.GetComponent<BeatDetection>();
                eventSound.CallBackFunction += CallBackFunction;
            }
            else if (GameManager.Instance.Style == GameManager.GameStyle.Battle)
            {
                var soundName1 = $"{totalCharacter[i].ID}_{GameManager.GameStyle.Normal}";
                if (this.transform.IsChild(soundName1) == false)
                {
                    var soundObject = PoolByID.Instance.GetPrefab(SoundPrefab, this.transform);
                    soundObject.name = soundName1;
                    var script = soundObject.GetComponent<SoundPrefab>();
                    script.Create(totalCharacter[i].GetAudioClip(GameManager.GameStyle.Normal));
                    var eventSound = soundObject.GetComponent<BeatDetection>();
                    eventSound.CallBackFunction += CallBackFunction;
                }

                var soundName2 = $"{totalCharacter[i].ID}_{GameManager.GameStyle.Battle}";
                if (this.transform.IsChild(soundName2) == false)
                {
                    var soundObject = PoolByID.Instance.GetPrefab(SoundPrefab, this.transform);
                    soundObject.name = soundName2;
                    var script = soundObject.GetComponent<SoundPrefab>();
                    script.Create(totalCharacter[i].GetAudioClip(GameManager.GameStyle.Battle));
                    var eventSound = soundObject.GetComponent<BeatDetection>();
                    eventSound.CallBackFunction += CallBackFunction;
                }
            }
            else if (GameManager.Instance.Style == GameManager.GameStyle.Battle_Single)
            {
                var soundName = $"{totalCharacter[i].ID}_{GameManager.GameStyle.Battle}";
                if (this.transform.FindChildByParent(soundName))
                    continue;
                var soundObject = PoolByID.Instance.GetPrefab(SoundPrefab, this.transform);
                soundObject.name = soundName;
                var script = soundObject.GetComponent<SoundPrefab>();
                script.Create(totalCharacter[i].GetAudioClip(GameManager.GameStyle.Battle));
                var eventSound = soundObject.GetComponent<BeatDetection>();
                eventSound.CallBackFunction += CallBackFunction;
            }
            else if (GameManager.Instance.Style == GameManager.GameStyle.Monster)
            {
                var soundName = $"{totalCharacter[i].ID}_{GameManager.GameStyle.Monster}";
                if (this.transform.FindChildByParent(soundName))
                    continue;
                var soundObject = PoolByID.Instance.GetPrefab(SoundPrefab, this.transform);
                soundObject.name = soundName;
                var script = soundObject.GetComponent<SoundPrefab>();
                script.Create(totalCharacter[i].GetAudioClip(GameManager.GameStyle.Monster));
                var eventSound = soundObject.GetComponent<BeatDetection>();
                eventSound.CallBackFunction += CallBackFunction;
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
