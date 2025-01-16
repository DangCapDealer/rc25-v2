using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundDetection : MonoBehaviour
{
    public SpriteRenderer BackgroundEffect;
    public Animation LightAnimation;

    public int numberOfEnergy = 0;
    public int numberOfHitHat = 0;
    public int numberOfKick = 0;
    public int numberOfSnare = 0;


    public void Start()
    {
        SoundSpawn.Instance._OnBeatDetection += OnBeatDetection;
    }

    private void OnBeatDetection(BeatDetection.EventInfo eventInfo)
    {
        switch (eventInfo.messageInfo)
        {
            case BeatDetection.EventType.Energy:
                numberOfEnergy += 1;
                lightRotation();
                break;
            case BeatDetection.EventType.HitHat:
                numberOfHitHat += 1;
                break;
            case BeatDetection.EventType.Kick:
                numberOfKick += 1;
                backgroundEffect();
                break;
            case BeatDetection.EventType.Snare:
                numberOfSnare += 1;
                break;
        }
    }

    private void backgroundEffect()
    {
        BackgroundEffect.DOKill();
        BackgroundEffect.DOColor(BackgroundEffect.color.WithAlpha(0.8f), 0.05f).OnComplete(() =>
        {
            BackgroundEffect.DOColor(BackgroundEffect.color.WithAlpha(0.0f), 0.1f);
        });
    }   
    
    private void lightRotation()
    {
        LightAnimation.Play();
    }
}
