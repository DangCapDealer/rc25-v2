using EditorCools;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Data/Character", order = 1)]
public class CharacterDataSO : ScriptableObject
{
    public enum PayType
    {
        None,
        Ads
    }    

    [System.Serializable]
    public class CharacterSO
    {
        public string ID;
        public Sprite Icon;
        public GameObject Prefab;
        public AudioClip AudioClipNormal;
        public AudioClip AudioClipHorror;
        public AudioClip AudioClipBattle;
        public PayType PayType;
    }


    [Button]    
    private void LoadAudio()
    {
#if UNITY_EDITOR
        //Debug.Log("LoadAudio");
        for (int i = 0; i < Characters.Length; i++)
        {
            var audioClip = Characters[i].AudioClipHorror;
            if (audioClip != null)
            {
                string path = AssetDatabase.GetAssetPath(audioClip);
                //Debug.Log("Audio file path: " + path);
                var normalPath = path.Replace("Horror", "Normal");
                AudioClip normalClip = AssetDatabase.LoadAssetAtPath<AudioClip>(normalPath);
                if (normalClip != null)
                    Characters[i].AudioClipNormal = normalClip;
                else
                    Debug.Log($"Missing normal audio {audioClip.name}");
                var battlePath = path.Replace("Horror", "Human (Sprunki song)");
                AudioClip battleClip = AssetDatabase.LoadAssetAtPath<AudioClip>(battlePath);
                if (battleClip != null)
                    Characters[i].AudioClipBattle = battleClip;
                else
                    Debug.Log($"Missing battle audio {audioClip.name}");
            }
            else
            {
                Debug.Log("No AudioClip assigned!");
            }
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Saved " + this.name);
#endif
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
