using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    public GameObject[] pickups;

    float spawnTimer;
    float spawnDelay = 3;


    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

        if (Time.time > spawnTimer)
        {
            if (PickupCount() < 10)
            {
                Instantiate(pickups[0], transform.position, Quaternion.identity);
                ResetTimer();
            }
        }
    }

    void ResetTimer()
    {
        spawnDelay += Random.Range(1, 5);
        spawnTimer = spawnDelay;
    }

    int PickupCount()
    {
        //List<GameObject> enemies = new List<GameObject>();
        GameObject[] pickups;
        pickups = GameObject.FindGameObjectsWithTag("Pickup");
        //print("Number enemies " + pickups.Length);
        return pickups.Length;
    }
}
