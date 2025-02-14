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
        public Transform Light;
    }

    public BackgroundData[] backgroundDatas;
    public SpriteRenderer backgroudBase;
    public SpriteRenderer backgroudEffect;
    public Transform lightRay2D;

    [Header("Speaker")]
    public Transform[] speakers;

    public void SettingBackground()
    {
        backgroundDatas.ForEach(background => {
            if(background.Style == GameManager.Instance.Style)
            {
                backgroudBase.sprite = background.Normal;
                backgroudEffect.sprite = background.Kick;
                background.Light.SetActive(true);
            }
            else
            {
                background.Light.SetActive(false);
            }    
        });

        if (GameManager.Instance.Style == GameManager.GameStyle.Normal ||
            GameManager.Instance.Style == GameManager.GameStyle.Horror)
            speakers.ForEach(speaker => speaker.SetActive(true));
        else if (GameManager.Instance.Style == GameManager.GameStyle.Battle)
            speakers.ForEach(speaker => speaker.SetActive(true));
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
