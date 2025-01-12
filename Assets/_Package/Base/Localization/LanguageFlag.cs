using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Language", menuName = "Language/Flag", order = 1)]
public class LanguageFlag : ScriptableObject
{
    [System.Serializable]
    public class Language
    {
        public string ID;
        public string Name;
        public Sprite Flag;
    }

    public Language[] Languages;
}
