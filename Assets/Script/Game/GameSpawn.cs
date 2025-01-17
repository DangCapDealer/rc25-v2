using System.Collections;
using System.Collections.Generic;
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
    public GameObject WhitePrefab;
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

    public Transform GetOncePositionInPool()
    {
        for (int i = 0; i < BaseObjects.Count; i++)
        {
            if (BaseObjects[i].IsActive() == false)
                continue;
            return BaseObjects[i].transform;
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

        PoolByID.Instance.PushToPool(target.gameObject);
        var character = PoolByID.Instance.GetPrefab(dataFromSO.Prefab, target.position, Quaternion.identity, this.transform);
        CreateObjects.Add(character);
        character.name = msg;
        var script = character.GetComponent<Character>();
        script.CreateCharacter(GameCamera);

        NumberofCharacter += 1;
        if (NumberofCharacter == 1)
            SoundSpawn.Instance.Reload();

        ChangeRuntimeObject(target, character.transform);
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
        NumberofCharacter = 0;
        RemoveAllCharacter();
        for (int i = 0; i < GameManager.Instance.NumberOfCharacter; i++)
        {
            var position = GridInCamera.Instance.GetPosition(i).WithZ(0);
            if (msg == "Normal")
            {
                var sponky = PoolByID.Instance.GetPrefab(WhitePrefab, position, Quaternion.identity, this.transform);
                BaseObjects.Add(sponky);
                RuntimeDataObjects.Add(sponky);
            }
            else if (msg == "Horror")
            {
                var sponky = PoolByID.Instance.GetPrefab(GrayPrefab, position, Quaternion.identity, this.transform);
                BaseObjects.Add(sponky);
                RuntimeDataObjects.Add(sponky);
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
        CreateObjects.Remove(character);
        PoolByID.Instance.PushToPool(character);

        CanvasSystem.Instance._gameUICanvas.ReloadCharacterUIButton(character.name);
        if (GameManager.Instance.Style == GameManager.GameStyle.Normal)
        {
            var sponky = PoolByID.Instance.GetPrefab(WhitePrefab, character.transform.position, Quaternion.identity, this.transform);
            BaseObjects.Add(sponky);

            ChangeRuntimeObject(character.transform, sponky.transform);

            NumberofCharacter -= 1;
        }
        else if (GameManager.Instance.Style == GameManager.GameStyle.Horror)
        {
            var sponky = PoolByID.Instance.GetPrefab(GrayPrefab, character.transform.position, Quaternion.identity, this.transform);
            BaseObjects.Add(sponky);

            ChangeRuntimeObject(character.transform, sponky.transform);

            NumberofCharacter -= 1;
        }
    }   
    
    //public void CreateBaseCharacter(Transform character)
    //{
    //    CreateBaseCharacter(character.position);
    //} 
    
    public void CreateBaseCharacter(Vector3 position)
    {
        if (GameManager.Instance.Style == GameManager.GameStyle.Normal)
        {
            var sponky = PoolByID.Instance.GetPrefab(WhitePrefab, position, Quaternion.identity, this.transform);
            BaseObjects.Add(sponky);
            RuntimeDataObjects.Add(sponky);
        }
        else if (GameManager.Instance.Style == GameManager.GameStyle.Horror)
        {
            var sponky = PoolByID.Instance.GetPrefab(GrayPrefab, position, Quaternion.identity, this.transform);
            BaseObjects.Add(sponky);
            RuntimeDataObjects.Add(sponky);
        }
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
}
