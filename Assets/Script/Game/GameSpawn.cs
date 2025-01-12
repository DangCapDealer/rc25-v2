using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpawn : MonoSingleton<GameSpawn>
{
    public CharacterDataSO CharacterData;
    public GameObject WhitePrefab;
    public GameObject GrayPrefab;

    public List<GameObject> BaseObjects = new List<GameObject>();

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
        target.SetActive(false);
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
        BaseObjects.ForEach(x => PoolByID.Instance.PushToPool(x));
        BaseObjects.Clear();

        for (int i = 0; i < GameManager.Instance.NumberOfCharacter; i++)
        {
            var position = GridInCamera.Instance.GetPosition(i).WithZ(0);
            if (msg == "Normal")
            {
                var sponky = PoolByID.Instance.GetPrefab(WhitePrefab, position, Quaternion.identity, transform);
                BaseObjects.Add(sponky);
            }
            else if (msg == "Horror")
            {
                var sponky = PoolByID.Instance.GetPrefab(GrayPrefab, position, Quaternion.identity, transform);
                BaseObjects.Add(sponky);
            }
        }
    }
}
