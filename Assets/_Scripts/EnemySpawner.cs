using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemySpawner : MonoBehaviourPunCallbacks
{
    public GameObject enemy;
    public int numToSpawn = 6;
    public float SpawnRadius;

    public enum SpawnerState { SPAWNING, IDLE, COUNTINGDOWN };
    public SpawnerState spawnerState = SpawnerState.IDLE;

    private float timerLength;
    private float timer;
    private float waveTimer;

    private void Update()
    {
        //if (!PhotonNetwork.IsMasterClient)
        //{
            if (spawnerState == SpawnerState.COUNTINGDOWN)
            {
                Debug.Log("CountDown");
                timer -= Time.deltaTime;
            }

            if (spawnerState == SpawnerState.IDLE)
            {
                Debug.Log("Idle");
                waveTimer -= Time.deltaTime;
                if (waveTimer <= 0)
                {
                    spawnerState = SpawnerState.COUNTINGDOWN;
                }
            }

            if (timer < 0)
            {
                spawnerState = SpawnerState.SPAWNING;
                timer = timerLength;
                StartCoroutine(SpawnEnemies());
            }
        //}
    }


    IEnumerator SpawnEnemies()
    {
        while (spawnerState == SpawnerState.SPAWNING)
        {
            for(int i = 0; i < numToSpawn; i++)
            {
                float randX = Random.Range(-SpawnRadius, SpawnRadius);
                float randY = Random.Range(-SpawnRadius, SpawnRadius);
                Vector2 selectedPos = new Vector2(randX, randY);
                GameObject go = PhotonNetwork.Instantiate(enemy.name, selectedPos, Quaternion.identity);
                go.GetComponent<Enemy>().WalkTheEarth();
                yield return new WaitForSeconds(.5f);
            }

            waveTimer = numToSpawn * .5f;
            numToSpawn += numToSpawn;
            spawnerState = SpawnerState.IDLE;
        }
    }
}
