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
        [HideInInspector] public Sprite Icon;

        public Sprite[] Icons;
        public Sprite GetIcon(string _mode)
        {
            if (_mode == "ItalianBrainrot") return Icons[1];
            else return Icons[0];
        }

        public GameObject Prefab;
        public AudioClip AudioClipNormal;
        public AudioClip AudioClipHorror;
        public AudioClip AudioClipBattle;
        public AudioClip AudioClipMonster;
        public AudioClip AudioClipMonstrous;
        public AudioClip AudioClipItalianBrainrot;

        public PayType PayType;

        // tuỳ chỉnh data để trả về đúng mode
        public AudioClip GetAudioClip(GameManager.GameStyle gameStyle)
        {
            if (gameStyle == GameManager.GameStyle.Normal) return AudioClipNormal;
            else if (gameStyle == GameManager.GameStyle.Horror) return AudioClipHorror;
            else if (gameStyle == GameManager.GameStyle.Battle) return AudioClipBattle;
            else if (gameStyle == GameManager.GameStyle.Monster) return AudioClipMonster;
            else if (gameStyle == GameManager.GameStyle.Monstrous) return AudioClipMonstrous;
            else if (gameStyle == GameManager.GameStyle.ItalianBrainrot) return AudioClipItalianBrainrot;
            else return null;
        }
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

                var monsterPath = path.Replace("Horror", "Monster");
                AudioClip monsterClip = AssetDatabase.LoadAssetAtPath<AudioClip>(monsterPath);
                if (monsterClip != null)
                    Characters[i].AudioClipMonster = monsterClip;
                else
                    Debug.Log($"Missing monster audio {audioClip.name}");
                var MonstrousPath = path.Replace("Horror", "Monstrous");
                AudioClip MonstrousClip = AssetDatabase.LoadAssetAtPath<AudioClip>(MonstrousPath);
                if (MonstrousClip != null)
                    Characters[i].AudioClipMonstrous = MonstrousClip;
                else
                    Debug.Log($"Missing Monstrous audio {audioClip.name}");
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
    [Button]
    private void importSound()
    {
        // dùng nhạc bài horror
        //for (int i = 0; i < Characters.Length; i++)
        //{
        //    Characters[i].AudioClipItalianBrainrot = Characters[i].AudioClipHorror;
        //}

        //EditorUtility.SetDirty(this);
        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();
        //Debug.Log("Saved " + this.name);
    }

    [Button]
    private void importIcon()
    {
        //for (int i = 0; i < Characters.Length; i++)
        //{
        //    Characters[i].Icons = new Sprite[0];
        //    Characters[i].Icons = Characters[i].Icons.Add(Characters[i].Icon);
        //    var characterObject = Characters[i].Prefab;
        //    GameObject prefabInstance = PrefabUtility.LoadPrefabContents(AssetDatabase.GetAssetPath(characterObject));
        //    //Debug.Log(prefabInstance.transform.childCount);
        //    var spriteObject = prefabInstance.FindChildByParent("ItalianBrainrot");
        //    if (spriteObject == null)
        //    {
        //        Debug.Log($"{prefabInstance.name} ERROR");
        //        continue;
        //    }

        //    var sr = spriteObject.GetComponent<SpriteRenderer>();
        //    //Debug.Log(sr.sprite.name);
        //    var iconPath = $"Assets/Texture2D/Icon-1/{sr.sprite.name}.png";
        //    var iconSprite = AssetDatabase.LoadAssetAtPath<Sprite>(iconPath);
        //    if (iconSprite == null)
        //    {
        //        Debug.Log($"ICON {iconPath} ERROR");
        //        continue;
        //    }
        //    Characters[i].Icons = Characters[i].Icons.Add(iconSprite);
        //}

        //EditorUtility.SetDirty(this);
        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();
        //Debug.Log("Saved " + this.name);
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
