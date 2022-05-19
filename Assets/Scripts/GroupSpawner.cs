using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupSpawner : MonoBehaviour
{
    [Header("What to spawn")]
    public GameObject spawnItem;

    [Header("Where to spawn from. If empty will use RANDOM SPHERE")]
    public Transform[] spawnPoints;

    [Header("Max number of objects to ever spawn")]
    public int maxSpawnTotal;

    [Header("Max number of objects in scene at any time")]
    public int maxSpawnConcurrently;

    [Header("Time between each spawn")]
    public float spawnDelay = 3;

    [Header("What is the TAG string of the spawned object?")]
    public string tagName;

    [Header("What is the UI message to display?")]
    public GameObject uiObject;

    [Header("Is this spawn enabled?")]
    public bool isEnabled;

    // internals
    int currentNumberSpawned;
    float spawnTimer;
    bool CanSpawn { get; set; } = false;

    float uiTimer;
    float uiDelay =3f;

    bool hasADisplayMessage;
    bool hasDisplayedMessage;

    private void Start()
    {
        // hide spawner
        foreach (Transform child in spawnPoints)
        {
            foreach (Transform gchild in child)
            {
                gchild.GetComponent<MeshRenderer>().enabled = false;
            }
        }
        if (uiObject != null) hasADisplayMessage = true;
    }


    // Update is called once per frame
    void Update()
    {
        if (!isEnabled) return;

        if (hasADisplayMessage)
        {
            // switch on message
            if (!hasDisplayedMessage)
            {
                // handle ui timer
                uiObject.SetActive(true);
                uiTimer = Time.time + uiDelay;
                hasDisplayedMessage = true;
            }

            // switch off ui message and start spawning
            if (Time.time > uiTimer)
            {
                uiObject.SetActive(false);
                CanSpawn = true;
            }
        }
        else CanSpawn = true;



        if (CanSpawn)
        if (Time.time > spawnTimer)
        {
            if (currentNumberSpawned < maxSpawnTotal)
                if (CountObjectsInScene() < maxSpawnConcurrently)
                {
                    int randomSpawnPoint = Random.Range(0, spawnPoints.Length);
                    currentNumberSpawned++;
                    GameObject spawn = Instantiate(
                        spawnItem,
                        spawnPoints[randomSpawnPoint].position,
                        spawnPoints[randomSpawnPoint].localRotation);
                    //spawn.GetComponent<Rigidbody>().freezeRotation = true;
                    ResetTimer();
                }
        }
    }

    void ResetTimer()
    {
        spawnTimer = Time.time + spawnDelay + Random.Range(1, 2);
    }

    int CountObjectsInScene()
    {
        //List<GameObject> enemies = new List<GameObject>();
        GameObject[] pickups;
        pickups = GameObject.FindGameObjectsWithTag(tagName);
        if (pickups == null) print("ERROR: wrong tag");
        //print("Number spawned items" + pickups.Length);
        return pickups.Length;
    }


}
