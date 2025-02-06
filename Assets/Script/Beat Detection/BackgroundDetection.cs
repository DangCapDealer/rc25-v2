using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BackgroundDetection : MonoSingleton<BackgroundDetection>
{
    public SpriteRenderer BackgroundEffect;
    public Animation LightAnimation;

    public int numberOfEnergy = 0;
    public int numberOfHitHat = 0;
    public int numberOfKick = 0;
    public int numberOfSnare = 0;

    [System.Serializable]
    public class BackgroundData
    {
        public GameManager.GameStyle Style;
        public Sprite Normal;
        public Sprite Kick;
    }

    public BackgroundData[] backgroundDatas;
    public SpriteRenderer backgroudBase;
    public SpriteRenderer backgroudEffect;

    [System.Serializable]
    public class LightData
    {
        public GameManager.GameStyle Style;
        public Transform Light;
    }

    public LightData[] lightDatas;
    public Transform lightRay2D;

    public void SettingBackground()
    {
        for (int i = 0; i < backgroundDatas.Length; i++)
        {
            if (backgroundDatas[i].Style == GameManager.Instance.Style)
            {
                backgroudBase.sprite = backgroundDatas[i].Normal;
                backgroudEffect.sprite = backgroundDatas[i].Kick;
            }
        }

        for (int i = 0; i < lightDatas.Length; i++)
        {
            if (lightDatas[i].Style == GameManager.Instance.Style)
                lightDatas[i].Light.SetActive(true);
            else
                lightDatas[i].Light.SetActive(false);
        }
    }


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
