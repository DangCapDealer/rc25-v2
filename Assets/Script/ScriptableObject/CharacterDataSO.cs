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
    }

    public CharacterSO[] Characters;
}
