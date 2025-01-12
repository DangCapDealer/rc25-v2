using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PoolByID : MonoSingleton<PoolByID>
{
    private IDictionary<int, List<GameObject>> pools;
    private IDictionary<string, GameObject> poolPrefabs;

    protected override void Awake()
    {
        base.Awake();
        pools = new Dictionary<int, List<GameObject>>();
        poolPrefabs = new Dictionary<string, GameObject>();
    }

    public GameObject GetPrefab(string path)
    {
        if(poolPrefabs.ContainsKey(path))
        {
            return GetPrefab(poolPrefabs[path]);
        }
        else
        {
            var _prefab = Resources.Load<GameObject>(path);
            poolPrefabs.Add(path, _prefab);
            return GetPrefab(_prefab);
        }
    }

    public GameObject GetPrefab(GameObject obj)
    {
        var id = obj.GetInstanceID();
        if (pools.ContainsKey(id))
        {
            for (int i = 0; i < pools[id].Count; i++)
            {
                if (!pools[id][i].activeSelf)
                {
                    pools[id][i].SetActive(true);

                    return pools[id][i];
                }
            }
        }
        else
        {
            pools.Add(id, new List<GameObject>());
        }

        var _obj = Instantiate(obj) as GameObject;
        _obj.name = _obj.name.Replace("(Clone)", "");
        pools[id].Add(_obj);
        return _obj;
    }

    public GameObject GetPrefab(GameObject obj, bool isActive)
    {
        var id = obj.GetInstanceID();
        if (pools.ContainsKey(id))
        {
            for (int i = 0; i < pools[id].Count; i++)
            {
                if (!pools[id][i].activeSelf)
                {
                    pools[id][i].SetActive(isActive);

                    return pools[id][i];
                }
            }
        }
        else
        {
            pools.Add(id, new List<GameObject>());
        }

        var _obj = Instantiate(obj) as GameObject;
        _obj.name = _obj.name.Replace("(Clone)", "");
        pools[id].Add(_obj);
        return _obj;
    }

    public GameObject GetPrefab(GameObject obj, Vector3 pos, Quaternion quaternion)
    {
        var id = obj.GetInstanceID();
        if (pools.ContainsKey(id))
        {
            //Find obj from pool
            for (int i = 0; i < pools[id].Count; i++)
            {
                if (!pools[id][i].activeSelf)
                {
                    pools[id][i].transform.position = pos;
                    pools[id][i].transform.rotation = quaternion;
                    pools[id][i].SetActive(true);

                    return pools[id][i];
                }
            }
        }
        else
        {
            //Create pool
            pools.Add(id, new List<GameObject>());
        }

        var _obj = Instantiate(obj, pos, quaternion) as GameObject;
        _obj.SetActive(true);
        _obj.name = _obj.name.Replace("(Clone)", "");
        pools[id].Add(_obj);
        return _obj;
    }

    public GameObject GetPrefab(GameObject obj, Vector3 pos, Quaternion quaternion, Transform parent)
    {
        var id = obj.GetInstanceID();
        if (pools.ContainsKey(id))
        {
            //Find obj from pool
            for (int i = 0; i < pools[id].Count; i++)
            {
                if (!pools[id][i].activeSelf)
                {
                    pools[id][i].transform.position = pos;
                    pools[id][i].transform.rotation = quaternion;
                    pools[id][i].transform.parent = parent;
                    pools[id][i].SetActive(true);

                    return pools[id][i];
                }
            }
        }
        else
        {
            //Create pool
            pools.Add(id, new List<GameObject>());
        }

        
        var _obj = Instantiate(obj, pos, quaternion) as GameObject;
        _obj.SetActive(true);
        _obj.name = _obj.name.Replace("(Clone)", "");
        _obj.transform.parent = parent;
        pools[id].Add(_obj);
        return _obj;
    }

    public GameObject GetPrefab(GameObject obj, Transform parent)
    {
        var id = obj.GetInstanceID();
        if (pools.ContainsKey(id))
        {
            //Find obj from pool
            for (int i = 0; i < pools[id].Count; i++)
            {
                if (!pools[id][i].activeSelf)
                {
                    pools[id][i].transform.parent = parent;
                    pools[id][i].SetActive(true);

                    return pools[id][i];
                }
            }
        }
        else
        {
            //Create pool
            pools.Add(id, new List<GameObject>());
        }

        var _obj = Instantiate(obj, parent) as GameObject;
        _obj.SetActive(true);
        _obj.name = _obj.name.Replace("(Clone)", "");
        pools[id].Add(_obj);
        return _obj;
    }

    public T GetPrefab<T>(GameObject obj, Vector3 pos, Quaternion quaternion)
    {
        var _obj = GetPrefab(obj, pos, quaternion);
        return _obj.GetComponent<T>();
    }

    public T GetPrefab<T>(GameObject obj, Transform parent)
    {
        var _obj = GetPrefab(obj, parent);
        return _obj.GetComponent<T>();
    }

    public T GetPrefab<T>(GameObject obj)
    {
        var _obj = GetPrefab(obj);
        return _obj.GetComponent<T>();
    }

    public void PushToPool(GameObject obj)
    {
        if (obj != null)
        {
            obj.transform.parent = null;
            obj.SetActive(false);
        }    
    }

    public void PushToPool(GameObject obj, bool isParent = false)
    {
        if (obj != null)
        {
            if (isParent == false)
            {
                obj.transform.parent = null;
            }
            obj.SetActive(false);
        }
    }

    public void PushToPool(GameObject obj, float time)
    {
        if (obj != null)
        {
            CoroutineUtils.PlayCoroutine(() =>
            {
                obj.transform.parent = null;
                obj.SetActive(false);
            }, time);
        }
    }    
}
