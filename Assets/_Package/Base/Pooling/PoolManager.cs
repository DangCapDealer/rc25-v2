using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;



#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

public enum PoolName
{
    None,
    Door,
    Apple,
    Player,
    Cabinet,
    ElectricOven,
    Fridge,
    Rubix,
    Painting,
    Safe,
    Key,
    KeyGarage,
    Slander,
    Alarm,
    Pendulum,
    Flash,
    Battery,
    Computer,
    Paper,
    Charger,
    Laptop,
    Phone,
    DogFood,
    Knife,
    ChiliPowder,
    Lighter,
    ScrewDriver,
    PaintingKitchen,
    PaintingPiece,
    SafeBedRoom,
    KeyGold,
    KeySliver,
    Padlock,
    MedicineCabinet,
    SleepingPills,
    WashingMachine,
    TVRemote,
    Medicine,
    Wine,
    Cocacola,
    RedGem,
    BlueGem,
    ChestGarret,
    CrowBar,
    Window,
    PaperBox,
    WoodenBox,
    CandleSort,
    Cat,
    KeyGate,
    Gate,
    Switch,
    PuzzleCabinet,
    KeyBedRoom,
    WeddingPicture,
    DoorRing
}

[System.Serializable]
public struct PoolNameAndNamePrefab
{
    public PoolName poolName;
    public GameObject poolRef;

    public PoolNameAndNamePrefab(PoolName _poolName, GameObject _poolRef)
    {
        poolName = _poolName;
        poolRef = _poolRef;
    }
}

public class PoolManager : MonoSingleton<PoolManager>
{
    public PoolNameAndNamePrefab[] effectObjects = null;
    public PoolNameAndNamePrefab[] otherObjects = null;

    private PoolNameAndNamePrefab[] poolNameAndNamePrefab = null;
    private IDictionary<PoolName, Queue<GameObject>> pools;

    protected override void Awake()
    {
        base.Awake();
        pools = new Dictionary<PoolName, Queue<GameObject>>();

        var effectPoolNumber = effectObjects.Length;
        var otherPoolNumber = otherObjects.Length;
        var poolNumber = effectPoolNumber + otherPoolNumber;

        poolNameAndNamePrefab = new PoolNameAndNamePrefab[poolNumber];
        //poolSizes = new PoolSize[poolNumber];
        for (int i = 0; i < poolNumber; i++)
        {
            if (i < effectPoolNumber)
            {
                poolNameAndNamePrefab[i] = effectObjects[i];
                continue;
            }
            if (i - effectPoolNumber < otherPoolNumber)
            {
                poolNameAndNamePrefab[i] = otherObjects[i - effectPoolNumber];
                continue;
            }
        }
    }

    public GameObject PopPool(PoolName poolName, Vector3 pos = new Vector3(), Quaternion rotate = new Quaternion(), bool isParent = false)
    {
        GameObject obj = null;
        if (pools.ContainsKey(poolName) && pools[poolName].Count > 0)
        {
            obj = pools[poolName].Dequeue();
        }
        else
        {
            Debug.Log("Spawn new item");
            obj = Instantiate(GetPrefabByName(poolName), pos, rotate) as GameObject;
        }

        obj.SetActive(true);
        if (isParent == false)
            obj.transform.parent = null;
        obj.transform.position = pos;
        obj.transform.rotation = rotate;

        //PlusPool(poolName);
        obj.name = poolName.ToString();

        if (!IsEffect(poolName))
            GameEvent.OnCreateObjectMethod(obj);

        return obj;
    }

    public GameObject ActiveObjectInPools(PoolName poolName)
    {
        GameObject obj = null;
        if (pools.ContainsKey(poolName) && pools[poolName].Count > 0)
            obj = pools[poolName].Dequeue();
        else
            return null;
        obj.SetActive(true);
        obj.name = poolName.ToString();
        return obj;
    }    

    public GameObject PopPool(PoolName poolName, Transform parent, Vector3 pos = new Vector3(), Quaternion rotate = new Quaternion())
    {
        GameObject obj = null;
        if (pools.ContainsKey(poolName) && pools[poolName].Count > 0)
        {
            obj = pools[poolName].Dequeue();
        }
        else
        {
            obj = Instantiate(GetPrefabByName(poolName), pos, rotate, parent) as GameObject;
        }

        obj.SetActive(true);
        obj.transform.parent = parent;
        obj.transform.position = pos;
        obj.transform.rotation = rotate;

        //PlusPool(poolName);
        obj.name = poolName.ToString();

        if (!IsEffect(poolName))
            GameEvent.OnCreateObjectMethod(obj);

        return obj;
    }

    public T PopPoolWithComponent<T>(PoolName poolName, Vector3 pos = new Vector3(), Quaternion rotate = new Quaternion())
    {
        GameObject obj = null;
        if (pools.ContainsKey(poolName) && pools[poolName].Count > 0)
        {
            obj = pools[poolName].Dequeue();
        }
        else
        {
            obj = Instantiate(GetPrefabByName(poolName), pos, rotate) as GameObject;
        }

        obj.SetActive(true);
        obj.transform.parent = null;
        obj.transform.position = pos;
        obj.transform.rotation = rotate;

        //PlusPool(poolName);
        obj.name = poolName.ToString();

        if (!IsEffect(poolName))
            GameEvent.OnCreateObjectMethod(obj);

        return obj.GetComponent<T>();
    }

    public T PopPoolWithComponent<T>(PoolName poolName, Transform parent, Vector3 pos = new Vector3(), Quaternion rotate = new Quaternion())
    {
        GameObject obj = null;
        if (pools.ContainsKey(poolName) && pools[poolName].Count > 0)
        {
            obj = pools[poolName].Dequeue();
        }
        else
        {
            obj = Instantiate(GetPrefabByName(poolName), pos, rotate, parent) as GameObject;
        }

        obj.SetActive(true);
        obj.transform.parent = parent;
        obj.transform.position = pos;
        obj.transform.rotation = rotate;

        //PlusPool(poolName);
        obj.name = poolName.ToString();

        if (!IsEffect(poolName))
            GameEvent.OnCreateObjectMethod(obj);

        return obj.GetComponent<T>();
    }

    public void PushPool(GameObject obj, PoolName poolName, bool isParent = false)
    {
        if (obj == null)
            return;

        if (isParent == false)
            obj.transform.parent = this.transform;

        if (!pools.ContainsKey(poolName))
            pools.Add(poolName, new Queue<GameObject>()); 

        if (obj.activeSelf)
        {
            obj.transform.DOKill();
            obj.SetActive(false);
        }
        pools[poolName].Enqueue(obj);

        //MinusPool(poolName);
        if (!IsEffect(poolName))
            GameEvent.RemoveObjectMethod(obj);
    }

    public void PushPool(GameObject go)
    {
        var id = (PoolName)Enum.Parse(typeof(PoolName), go.name);
        PushPool(go, id);
    }    

    private GameObject GetPrefabByName(PoolName name)
    {
        for(int i = 0; i < poolNameAndNamePrefab.Length; i++)
        {
            if (poolNameAndNamePrefab[i].poolName == name)
                return poolNameAndNamePrefab[i].poolRef;
        }
        return null;
    }

    private bool IsPrefabByName(PoolName name)
    {
        for (int i = 0; i < poolNameAndNamePrefab.Length; i++)
        {
            if (poolNameAndNamePrefab[i].poolName == name)
                return true;
        }
        return false;
    }    

    public void UnloadAllResource()
    {

    }

    public void PushToPushAfter(GameObject obj, PoolName poolname, float time)
    {
        CoroutineHandler.StartStaticCoroutine(PushAfter(obj, poolname, time));
    }

    public void PushToPushAfter(GameObject obj, PoolName poolname, float time, UnityAction callback)
    {
        CoroutineHandler.StartStaticCoroutine(PushAfter(obj, poolname, time, callback));
    }

    private IEnumerator PushAfter(GameObject obj, PoolName poolname, float time)
    {
        yield return WaitForSecondCache.GetWFSCache(time);

        PushPool(obj, poolname);
    }

    private IEnumerator PushAfter(GameObject obj, PoolName poolname, float time, UnityAction callback)
    {
        yield return WaitForSecondCache.GetWFSCache(time);

        callback?.Invoke();
        yield return WaitForSecondCache.WAIT_TIME_ONE;
        PushPool(obj, poolname);
    }

    public bool IsEffect(PoolName poolName)
    {
        for(int i = 0; i < effectObjects.Length; i++)
        {
            if (effectObjects[i].poolName == poolName)
                return true;
        }    
        return false;
    }
}