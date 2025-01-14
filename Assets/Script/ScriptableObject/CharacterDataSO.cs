using EditorCools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Data/Character", order = 1)]
public class CharacterDataSO : ScriptableObject
{
    [System.Serializable]
    public class CharacterSO
    {
        public string ID;
        public Sprite Icon;
        public GameObject Prefab;
        public AudioClip AudioClip;
    }

    public CharacterSO[] Characters;

    public CharacterSO Find(string msg)
    {
        for (int i = 0; i < Characters.Length; i++)
        {
            if (Characters[i].ID == msg)
                return Characters[i];
        }
        return null;
    }   
}
