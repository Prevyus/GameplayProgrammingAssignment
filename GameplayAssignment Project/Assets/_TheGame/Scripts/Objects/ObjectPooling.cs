using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Pool
{
    public GameObject prefab;
    public List<GameObject> pooledObjects;
}

public class ObjectPooling : MonoBehaviour
{// OBJECT POOLING SCRIPT, NOT USED IN THIS PROJECT

    public List<Pool> pools = new List<Pool>();
    [SerializeField] int poolRefillAmount;

    void Start()
    {
        for(int i = 0; i < pools.Count; i++)
        {
            TopUpPool(pools[i].prefab);
        }
    }

    [ContextMenu("Activate")]
    public GameObject GetPooledObject(GameObject prefab)
    {
        Pool pool = new Pool();
        foreach (Pool eachPool in pools) { if (eachPool.prefab == prefab) pool = eachPool; break; }

        if (pool.prefab != null)
        {
            for (int i = 0; i < pool.pooledObjects.Count; i++)
            {
                if (!pool.pooledObjects[i].activeInHierarchy)
                {
                    return pool.pooledObjects[i];
                }
                else if (i == pool.pooledObjects.Count - 1)
                {
                    TopUpPool(pool.prefab);
                    i = 0;
                }
            }
        }

        return null;
    }

    [ContextMenu("TopUp")]
    public void TopUpPool(GameObject prefab)
    {
        Pool pool = new Pool();
        foreach (Pool eachPool in pools) { if (eachPool.prefab == prefab) pool = eachPool; break; }

        if (pool.prefab != null)
        {
            for (int i = 0; i < poolRefillAmount; i++)
            {
                GameObject obj = Instantiate(prefab, transform);
                obj.SetActive(false);
                pool.pooledObjects.Add(obj);
            }
        }
    }
}
