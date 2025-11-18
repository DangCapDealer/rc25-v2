using DG.Tweening;
using System.CodeDom;
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

    // đata background game
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

    [Header("Mode")]
    public Transform ModeObject;

    [Header("Mode3")]
    public Transform Mode3Object;
    public Transform Mode3SplitObject;

    public void Mode3Complete()
    {
        if(Mode3SplitObject.position.x > 0)
        {
            Mode3SplitObject.DOKill();
            Mode3SplitObject.DOMoveX(5.0f, 0.5f);
        }   
        else if(Mode3SplitObject.position.x < 0)
        {
            Mode3SplitObject.DOKill();
            Mode3SplitObject.DOMoveX(-5.0f, 0.5f);
        }    
    }    

    public void SettingBackground()
    {
        backgroundDatas.ForEach(_background => _background.Light.SetActive(false));
        backgroundDatas.ForEach(background => {
            if(background.Style.ToString() == GameManager.Instance.Style.ToString())
            {
                backgroudBase.sprite = background.Normal;
                backgroudEffect.sprite = background.Kick;
                background.Light.SetActive(true);
            }   
        });

        if (GameManager.Instance.IsGameDefault())
        {
            speakers.ForEach(speaker => speaker.SetActive(true));
            ModeObject.SetActive(true);
            Mode3Object.SetActive(false);
        }    
        else if (GameManager.Instance.IsGameCustom())
        {
            speakers.ForEach(speaker => speaker.SetActive(false));
            ModeObject.SetActive(false);
            Mode3Object.SetActive(true);
        }    
    }


    private void Start()
    {
        SoundSpawn.Instance._OnBeatDetection += OnBeatDetection;
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameManager.GameState.Playing)
            return;

        // chỉ dành riêng cho mode 2 bên để chia phe đấm nhau
        if (GameManager.Instance.Style == GameManager.GameStyle.Battle)
        {
            var targetPosition = CanvasSystem.Instance._gameUICanvas.Mode3GetTargetPosition();
            Mode3SplitObject.position = targetPosition.WithY(Mode3SplitObject.position.y);
        }
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
