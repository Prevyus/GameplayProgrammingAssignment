using Custom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ObjectToSpawn
{ // STRUCT THAT STORES A PREFAB AND HOW MANY OF THEM YOU WANT SPAWNED
    public GameObject prefab;
    public int amount;
    [Tooltip("If this is an Item, should it be stacked or individual?")] public bool stacked;
}

public class Spawner : MonoBehaviour
{// SPAWNS ALL PREFABS IN THE OBJECTSTOSPAWN LIST, INSTANTLY OR WITH TIME IN BETWEEN EACH SPAWN
    public Transform Parent;
    public float timeBetweenSpawns = 0;
    public List<ObjectToSpawn> objectsToSpawn = new List<ObjectToSpawn>();
    bool paused = false;

    int index = 0;
    int left = 0;

    public void PauseSpawning()
    {
        paused = true;
    }

    public void ResumeSpawning()
    {
        paused = false;
    }

    private void Start()
    {
        SpawnObjects();
    }

    void Spawn(ObjectToSpawn obj)
    { // THE ACTUAL INSTANTIATION OF THE PREFABS
        GameObject spawnedObj = Instantiate(obj.prefab, Parent ? Parent : transform.parent, true);
        spawnedObj.transform.position = transform.position;
        spawnedObj.transform.rotation = transform.rotation;

        if (obj.stacked)
        { // IF THE OBJECT IS A GROUND ITEM, YOU CAN CHOOSE TO SPAWN THIS ITEM STACKED IN THE AMOUNT IT WAS SET TO IN THE STRUCT
            GroundItem Item = spawnedObj.GetComponent<GroundItem>();
            if (Item) Item.item.amount = obj.amount;
        }
    }

    void SpawnObjects()
    {
        if (objectsToSpawn.Count <= 0) return;

        if (timeBetweenSpawns > 0) // IF THERE IS A DELAY BETWEEN SPAWNS, THEN START A COROUTINE
        {
            left = objectsToSpawn[index].amount;
            StartCoroutine(SpawnObjectsWithDelay());
        }
        else // IF NOT, THEN JUST SPAWN ALL OBJECTS IN THE LIST INSTANTLY
        {
            foreach (ObjectToSpawn obj in objectsToSpawn)
            {
                if (obj.stacked)
                {
                    Spawn(obj);
                }
                else
                {
                    for (int i = 0; i < obj.amount; i++)
                    {
                        Spawn(obj);
                    }
                }
            }
            Destroy(gameObject);
        }
    }

    void NewSpawn()
    {
        index++;
        if (index < objectsToSpawn.Count) left = objectsToSpawn[index].amount;
    }

    IEnumerator SpawnObjectsWithDelay()
    { // COROUTINE FOR SPAWNING WITH A DELAY, IT ALSO GETS PAUSED WHEN THE GAME IS PAUSED
        float timer = 0f;

        while (true)
        {
            if (paused)
            {
                yield return null;
                continue;
            }

            timer += Time.deltaTime;

            if (timer < timeBetweenSpawns)
            {
                yield return null;
                continue;
            }

            timer = 0f;

            if (objectsToSpawn[index].stacked)
            {
                Spawn(objectsToSpawn[index]);
                left = 0;
                NewSpawn();
            }
            else if (left > 0)
            {
                Spawn(objectsToSpawn[index]);
                left--;
            }
            else
            {
                NewSpawn();
            }

            if (index >= objectsToSpawn.Count && left <= 0)
            {
                Destroy(gameObject);
                yield break;
            }

            yield return null;
        }
    }
}
