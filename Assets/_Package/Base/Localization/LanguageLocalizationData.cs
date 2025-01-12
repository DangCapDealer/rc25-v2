using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class LanguageLocalizationData : MonoSingleton<LanguageLocalizationData>
{
    public LanguageFlag languageFlag;

    public LanguageFlag GetLanguageFlag => languageFlag;

    public LanguageTranslate languageTranslate;
    //public Font[] fonts;
    private Dictionary<string, LanguageTranslate.Lanaguage> translate = new Dictionary<string, LanguageTranslate.Lanaguage>();
    public string GetText(string IdText)
    {
        var language = RuntimeStorageData.Player.Language;
        if(translate.ContainsKey(IdText) == false)
        {
            var datas = languageTranslate.translates;
            for(int i = 0; i < datas.Length; i++)
            {
                var data = datas[i];
                if(data.defaultText == IdText)
                {
                    translate.Add(IdText, data.Lanaguage);
                    break;
                }
            }
        }
        if (translate.ContainsKey(IdText) == false)
        {
            Debug.Log($"convert {IdText} to language {language}");
            return "Missing Text";
        }
        return GetTextIntoIDictionary(translate[IdText], language);
    }

    public string GetText(int idText)
    {
        var language = RuntimeStorageData.Player.Language;
        string _text = "";
        var datas = languageTranslate.translates;
        for(int i = 0; i < datas.Length; i++)
        {
            var data = datas[i];
            if(data.ID == idText)
            {
                _text = data.defaultText;
                if (translate.ContainsKey(data.defaultText))
                    break;
                translate.Add(data.defaultText, data.Lanaguage);
                break;
            }
        }

        if (translate.ContainsKey(_text) == false)
        {
            Debug.Log($"convert {_text} to language {language}");
            return "Missing Text";
        }
        return GetTextIntoIDictionary(translate[_text], language);
    }

    public string GetLanguage()
    {
        return RuntimeStorageData.Player.Language; ;
    }

    public string GetTextIntoIDictionary(LanguageTranslate.Lanaguage lanaguages, string language)
    {
        string response = "";
        switch(language)
        {
            case "English":
                response = lanaguages.English;
                break;
            case "China":
                response = lanaguages.China;
                break;
            case "France":
                response = lanaguages.France;
                break;
            case "Japan":
                response = lanaguages.Japan;
                break;
            case "Portuguese":
                response = lanaguages.Portuguese;
                break;
            case "Spanish":
                response = lanaguages.Spanish;
                break;
        }
        return response;
    }
}
