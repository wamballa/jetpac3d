using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerOnSphere : MonoBehaviour
{
    [Header("What to spawn")]
    public GameObject spawnItem;

    [Header("Radius of sphere")]
    public float radius = 50f;

    [Header("Max number of objects to ever spawn")]
    public int maxSpawnTotal;

    [Header("Max number of objects in scene at any time")]
    public int maxSpawnConcurrently;

    [Header("Time between each spawn")]
    public float spawnDelay = 3;

    string tagName;

    [Header("What is the UI message to display?")]
    public GameObject uiObject;

    [Header("Is this spawn enabled?")]
    public bool isEnabled;

    // internals
    int currentNumberSpawned;
    float spawnTimer;
    bool CanSpawn { get; set; } = false;

    float uiTimer;
    float uiDelay = 3f;

    bool hasADisplayMessage;
    bool hasDisplayedMessage;

    // Start is called before the first frame update
    void Start()
    {
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
                        GameObject spawn = Instantiate(
                            spawnItem,
                            Random.onUnitSphere * radius,
                            Quaternion.identity
                            );
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
        pickups = GameObject.FindGameObjectsWithTag(spawnItem.tag);
        if (pickups == null) print("ERROR: wrong tag");
        //print("Number spawned items" + pickups.Length);
        return pickups.Length;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
