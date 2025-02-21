using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using static GameEvent;
using static UnityEngine.GraphicsBuffer;

public class GameSpawn : MonoSingleton<GameSpawn>
{
    public Camera MainCamera;
    public Camera GameCamera;

    public CharacterDataSO CharacterData;
    //public GameObject WhitePrefab;
    public GameObject GrayPrefab;

    public int NumberofCharacter = 0;

    public List<GameObject> BaseObjects = new List<GameObject>();
    public List<GameObject> CreateObjects = new List<GameObject>();

    public List<GameObject> RuntimeDataObjects = new List<GameObject>();

    public CharacterDataSO.CharacterSO[] GetAllCharacter()
    {
        return CharacterData.Characters;
    }    

    public CharacterDataSO.CharacterSO FindCharacterData(string msg)
    {
        var dataFromSO = CharacterData.Find(msg);
        return dataFromSO;
    }

    public Transform CheckingNearPositionInPool(Vector2 target)
    {
        float min = 1.3f;
        Transform targetObject = null;
        for (int i = 0; i < BaseObjects.Count; i++)
        {
            if (BaseObjects[i].IsActive() == false || BaseObjects[i].name.EndsWith("_animation"))
                continue;
            var distance = Vector2.Distance(BaseObjects[i].position(), target);
            if (min > distance)
            {
                min = distance;
                targetObject = BaseObjects[i].transform;
            }
        }
        return targetObject;
    }    

    public Transform GetOncePositionInPool()
    {
        if(GameManager.Instance.Style == GameManager.GameStyle.Battle)
        {
            int numberCharacterLeft = 0, numberCharacterRight = 0;
            Transform characterLeft = null, characterRight = null;

            for (int  i = 0; i < CreateObjects.Count; i++)
            {
                if (CreateObjects[i].position().x > 0.0f) { numberCharacterRight += 1; }
                else if (CreateObjects[i].position().x < 0.0f) { numberCharacterLeft += 1; }
            }

            for (int i = 0; i < BaseObjects.Count; i++)
            {
                if (BaseObjects[i].IsActive() == false || BaseObjects[i].name.EndsWith("_animation"))
                    continue;
                if (BaseObjects[i].position().x > 0.0f)
                {
                    if (characterRight == null)
                        characterRight = BaseObjects[i].transform;
                }
                else if (BaseObjects[i].position().x < 0.0f)
                {
                    if (characterLeft == null)
                        characterLeft = BaseObjects[i].transform;
                }
            }

            if (numberCharacterLeft == numberCharacterRight)
            {
                if (characterLeft != null) return characterLeft;
                else return characterRight;
            }
            else if (numberCharacterLeft < numberCharacterRight)
            {
                if (characterLeft != null) return characterLeft;
                else return characterRight;
            }
            else
            {
                if (characterRight != null) return characterRight;
                else return characterLeft;
            }
        }
        else
        {
            for (int i = 0; i < BaseObjects.Count; i++)
            {
                if (BaseObjects[i].IsActive() == false || BaseObjects[i].name.EndsWith("_animation"))
                    continue;
                return BaseObjects[i].transform;
            }
        }
        return null;
    }    

    public void SpawnCharacterIntoPosition(string msg, Transform target)
    {
        var dataFromSO = FindCharacterData(msg);
        if (dataFromSO == null)
        {
            Debug.Log($"Data {msg} not found");
            return;
        }

        var character = PoolByID.Instance.GetPrefab(dataFromSO.Prefab, target.position, Quaternion.identity, this.transform);
        if (character != null)
        {
            target.name = $"{target.name}_animation";
            character.SetActive(false);
            character.name = msg;
            var script = character.GetComponent<Character>();
            script.CreateCharacter(GameCamera);
            NumberofCharacter += 1;
            if (NumberofCharacter == 1)
                SoundSpawn.Instance.Reload();
            CreateObjects.Add(character);
            ChangeRuntimeObject(target, character.transform);
        }
        var _characterBase = target.FindChildByParent("Horror");
        if (_characterBase.IsActive() == false)
            _characterBase = target.FindChildByParent("Normal");
        _characterBase.DOKill();
        _characterBase.DOScaleY(0, 0.3f).OnComplete(() =>
        {
            PoolByID.Instance.PushToPool(target.gameObject);
            target.name = target.name.Split("_")[0];
            _characterBase.localScale = _characterBase.localScale.WithY(_characterBase.localScale.x);
            character.SetActive(true);
        });
    }    


    private void OnEnable()
    {
        GameEvent.OnUITheme += OnUITheme;
    }

    private void OnDisable()
    {
        GameEvent.OnUITheme -= OnUITheme;
    }

    private void OnUITheme(string msg)
    {
        Debug.Log("UI Theme Character");
        NumberofCharacter = 0;
        RemoveAllCharacter();
        for (int i = 0; i < GameManager.Instance.NumberOfCharacter; i++)
        {
            var position = GridInCamera.Instance.GetPosition(i).WithZ(0);
            GameObject sponky = PoolByID.Instance.GetPrefab(GrayPrefab, position, Quaternion.identity, this.transform);

            if (sponky != null)
            {
                BaseObjects.Add(sponky);
                RuntimeDataObjects.Add(sponky);

                var _target = sponky.FindChildByParent("Normal");
                if (_target != null) _target.SetActive(false);
                var _untarget = sponky.FindChildByParent("Horror");
                if (_untarget != null) _untarget.SetActive(true);

                _untarget.localScale = _untarget.localScale.WithY(0);
                _untarget.DOKill();
                _untarget.DOScaleY(_untarget.localScale.x, 0.3f);
            }
        }
    }

    public void CreateNewPositionCharacter()
    {
        GridInCamera.Instance.CreatePosition();
        for (int i = 0; i < RuntimeDataObjects.Count; i++)
        {
            RuntimeDataObjects[i].transform.position = GridInCamera.Instance.GetPosition(i).WithZ(0);
        }
        CreateBaseCharacter(GridInCamera.Instance.GetLastPosition().WithZ(0));
    }

    public void RemoveAllCharacter()
    {
        BaseObjects.ForEach(x => PoolByID.Instance.PushToPool(x));
        BaseObjects.Clear();
        CreateObjects.ForEach(x => PoolByID.Instance.PushToPool(x));
        CreateObjects.Clear();
        RuntimeDataObjects.Clear();
    }    

    public void RemoveCharacter(GameObject character)
    {
        Debug.Log("Remove Character");
        CreateObjects.Remove(character);
        PoolByID.Instance.PushToPool(character);

        CanvasSystem.Instance._gameUICanvas.ReloadCharacterUIButton(character.name);
        var sponky = PoolByID.Instance.GetPrefab(GrayPrefab, character.transform.position, Quaternion.identity, this.transform);
        if (BaseObjects.Contains(sponky) == false)
            BaseObjects.Add(sponky);

        var _target = sponky.FindChildByParent("Normal");
        if (_target != null) _target.SetActive(false);
        var _untarget = sponky.FindChildByParent("Horror");
        if (_untarget != null) _untarget.SetActive(true);

        _untarget.localScale = _untarget.localScale.WithY(0);
        _untarget.DOKill();
        _untarget.DOScaleY(_untarget.localScale.x, 0.3f);

        ChangeRuntimeObject(character.transform, sponky.transform);
        NumberofCharacter -= 1;
    }   
    
    public void CreateBaseCharacter(Vector3 position)
    {
        Debug.Log("Create Character");
        var sponky = PoolByID.Instance.GetPrefab(GrayPrefab, position, Quaternion.identity, this.transform);
        BaseObjects.Add(sponky);
        RuntimeDataObjects.Add(sponky);

        var _target = sponky.FindChildByParent("Normal");
        if (_target != null) _target.SetActive(false);
        var _untarget = sponky.FindChildByParent("Horror");
        if (_untarget != null) _untarget.SetActive(true);

        _untarget.localScale = _untarget.localScale.WithY(0);
        _untarget.DOKill();
        _untarget.DOScaleY(_untarget.localScale.x, 0.3f);
    }

    private void ChangeRuntimeObject(Transform _fromObject, Transform _toObject)
    {
        for(int i = 0; i < RuntimeDataObjects.Count; i++)
        {
            if (RuntimeDataObjects[i] == _fromObject.gameObject)
            {
                RuntimeDataObjects[i] = _toObject.gameObject;
            }
        }
    }

    private GameObject GetPrefabs(GameObject _prefab)
    {
        return null;
    }    

    private void RemovePrefabs()
    {

    }    
}
