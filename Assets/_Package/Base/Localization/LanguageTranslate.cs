using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Language", menuName = "Language/Translate", order = 2)]
public class LanguageTranslate : ScriptableObject
{
    [System.Serializable]
    public class Translate
    {
        public int ID;
        public string defaultText;
        public Lanaguage Lanaguage;
        
    }

    [System.Serializable]
    public class Lanaguage
    {
        public string English;
        public string China;
        public string France;
        public string Japan;
        public string Portuguese;
        public string Spanish;
    }

    public Translate[] translates;
}
