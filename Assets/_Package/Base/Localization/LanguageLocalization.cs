using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageLocalization : MonoBehaviour
{
    public bool IsFontDefault;
    public int IdInt = -1;
    public string IdText;
    private bool IsInitialized = false;
    private Text textObject;

    private void OnEnable()
    {
        GameEvent.OnChangeLanguage += OnChangeLanguage;
        if (!IsInitialized)
        {
            IsInitialized = true;
            textObject = this.GetComponent<Text>();
            IdText = textObject.text;
            OnChangeLanguage();
        }
    }

    private void OnDisable()
    {
        GameEvent.OnChangeLanguage -= OnChangeLanguage;
    }

    private void OnChangeLanguage()
    {
        var _text = IdInt == -1 ? LanguageLocalizationData.Instance.GetText(IdText) : LanguageLocalizationData.Instance.GetText(IdInt);
        textObject.text = _text;
    }
}
