using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpawn : MonoSingleton<GameSpawn>
{
    public CharacterDataSO CharacterData;
    public GameObject WhitePrefab;
    public GameObject GrayPrefab;

    public List<GameObject> BaseObjects = new List<GameObject>();
    public List<GameObject> CreateObjects = new List<GameObject>();

    public CharacterDataSO.CharacterSO FindCharacterData(string msg)
    {
        var dataFromSO = CharacterData.Find(msg);
        return dataFromSO;
    }

    public Transform CheckingNearCharacterInPool(Vector2 target)
    {
        float min = 2.0f;
        Transform targetObject = null;
        for (int i = 0; i < BaseObjects.Count; i++)
        {
            if (BaseObjects[i].IsActive() == false)
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

    public void SpawnCharacterIntoPosition(string msg, Transform target)
    {
        var dataFromSO = FindCharacterData(msg);
        if (dataFromSO == null)
        {
            Debug.Log($"Data {msg} not found");
            return;
        }

        PoolByID.Instance.PushToPool(target.gameObject);
        var character = PoolByID.Instance.GetPrefab(dataFromSO.Prefab, target.position, Quaternion.identity, this.transform);
        CreateObjects.Add(character);
        var script = character.GetComponent<Character>();
        script.CreateCharacter();
    }    


    private void OnEnable()
    {
        GameEvent.OnThemeStype += OnThemeStype;
    }

    private void OnDisable()
    {
        GameEvent.OnThemeStype -= OnThemeStype;
    }

    private void OnThemeStype(string msg)
    {
        RemoveAllCharacter();

        for (int i = 0; i < GameManager.Instance.NumberOfCharacter; i++)
        {
            var position = GridInCamera.Instance.GetPosition(i).WithZ(0);
            if (msg == "Normal")
            {
                var sponky = PoolByID.Instance.GetPrefab(WhitePrefab, position, Quaternion.identity, this.transform);
                BaseObjects.Add(sponky);
            }
            else if (msg == "Horror")
            {
                var sponky = PoolByID.Instance.GetPrefab(GrayPrefab, position, Quaternion.identity, this.transform);
                BaseObjects.Add(sponky);
            }
        }
    }

    public void RemoveAllCharacter()
    {
        BaseObjects.ForEach(x => PoolByID.Instance.PushToPool(x));
        BaseObjects.Clear();
        CreateObjects.ForEach(x => PoolByID.Instance.PushToPool(x));
        CreateObjects.Clear();
    }    
}
