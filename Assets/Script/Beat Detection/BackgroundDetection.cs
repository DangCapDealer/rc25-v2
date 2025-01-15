using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundDetection : MonoBehaviour
{
    public SpriteRenderer BackgroundEffect;

    public void Start()
    {
        SoundSpawn.Instance._OnBeatDetection += OnBeatDetection;
    }

    private void OnBeatDetection(BeatDetection.EventInfo eventInfo)
    {
        switch (eventInfo.messageInfo)
        {
            case BeatDetection.EventType.Energy:
                break;
            case BeatDetection.EventType.HitHat:
                break;
            case BeatDetection.EventType.Kick:
                BackgroundEffect.DOKill();
                BackgroundEffect.DOColor(BackgroundEffect.color.WithAlpha(0.8f), 0.05f).OnComplete(() =>
                {
                    BackgroundEffect.DOColor(BackgroundEffect.color.WithAlpha(0.0f), 0.1f);
                });
                break;
            case BeatDetection.EventType.Snare:
                break;
        }
    }
}
