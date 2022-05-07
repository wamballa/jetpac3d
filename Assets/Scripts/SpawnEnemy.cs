using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject enemy;

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
            if (EnemyCount() < 10)
            {
                Instantiate(enemy, transform.position, Quaternion.identity);
                ResetTimer();
            }
        }
    }

    void ResetTimer()
    {
        spawnDelay += Random.Range(1, 5);
        spawnTimer = spawnDelay;
    }

    int EnemyCount()
    {
        //List<GameObject> enemies = new List<GameObject>();
        GameObject[] enemies;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        //print("Number enemies " + enemies.Length);
        return enemies.Length;
    }

}
