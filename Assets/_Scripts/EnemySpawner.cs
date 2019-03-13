using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class EnemySpawner : MonoBehaviourPunCallbacks
{
    public static EnemySpawner instance = null;
    public static int deathCount;
    public static int waveNumber;
    ObjectPooler objectPooler;
    public GameObject chopper;
    public int deadCount;
    public int waveCount;
    public GameObject enemy;
    public int numToSpawn;
    public int numberSpawned;
    private float spawnBonus;
    public float spawnX;
    public float spawnY;
    //public Text deathCounterText;
    public GameObject waveIndicator;
    public Text waveText;

    

    public enum SpawnerState { SPAWNING, IDLE, COUNTINGDOWN, WAITING, AIRDROP, GAMEOVER };
    public SpawnerState spawnerState = SpawnerState.IDLE;

    private float timer = 3;
    private bool spawning = true;

    private void Awake()
    {
        deathCount = 0;
        waveNumber = 1;
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(UpdateCount());
        }       
    }

    private void Start()
    { 
        objectPooler = ObjectPooler.Instance;
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && spawning)
        {
            if (spawnerState == SpawnerState.COUNTINGDOWN)
            {
                timer -= Time.deltaTime;
                photonView.RPC("SetWaveIndicator", RpcTarget.All, true, "Incoming Wave " + waveNumber.ToString());

                if (timer < 0)
                {
                    numToSpawn = GetNumToSpawn(waveNumber);
                    photonView.RPC("SetWaveIndicator", RpcTarget.All, false, "");
                    spawnerState = SpawnerState.SPAWNING;
                     
                    timer = 3;
                    StartCoroutine(SpawnEnemies());
                }
            }

            if(spawnerState == SpawnerState.GAMEOVER)
            {
                StopAllCoroutines();
                spawning = false;
                photonView.RPC("SetWaveIndicator", RpcTarget.All, false, "");
            }

            if (spawnerState == SpawnerState.WAITING)
            {
                timer -= Time.deltaTime;
                if(deathCount == numberSpawned || timer <= 0)
                {
                    if(waveNumber % 5 == 0)
                    {
                        StartCoroutine(AirDrop());  
                    }
                    else
                    {
                        PrepCountdown();
                    }           
                }  
            }
        }
    }

    int GetNumToSpawn(int n)
    {
        int numOfPlayers = PhotonNetwork.PlayerList.Length;
        int baseNumber = n * numOfPlayers;
        int totalToSpawn = Mathf.RoundToInt(baseNumber + (baseNumber / 2));
        if (totalToSpawn > 100)
        {
            totalToSpawn = 100;
            spawnBonus += .5f;
        }
        
        return totalToSpawn;
    }

    void PrepCountdown()
    {
        timer = 3;
        spawnerState = SpawnerState.COUNTINGDOWN;
    }

    IEnumerator AirDrop()
    {
        spawnerState = SpawnerState.AIRDROP;
        photonView.RPC("SetWaveIndicator", RpcTarget.All, true, "Get to the Chopper");
        GameObject go = PhotonNetwork.Instantiate(chopper.name, new Vector3(-30, 0, 0), Quaternion.identity);
        float airdropTimer = 10f;

        while(airdropTimer > 0)
        {
            airdropTimer -= Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        photonView.RPC("SetWaveIndicator", RpcTarget.All, false, "");
        PrepCountdown();
    }


    IEnumerator SpawnEnemies()
    {
        waveNumber++;
        photonView.RPC("UpdateWave", RpcTarget.All, waveNumber);
        numberSpawned += numToSpawn;
        while (spawnerState == SpawnerState.SPAWNING)
        {
            for (int i = 0; i < numToSpawn; i++)
            {
                float randX = Random.Range(-spawnX, spawnX);
                float randY = Random.Range(-spawnY, spawnY);
                Vector2 selectedPos = new Vector2(randX, randY);

                GameObject go = PhotonNetwork.Instantiate(enemy.name, selectedPos, Quaternion.identity);
                //GameObject go = objectPooler.SpawnFromPool("Enemy", selectedPos, Quaternion.identity);
                go.GetComponent<Enemy>().Damage = 1 + spawnBonus + (waveNumber / 5);
                go.transform.position = selectedPos;

                yield return new WaitForSeconds(.5f);
            }

            if (numToSpawn < 5)
            {
                timer = 5;
            }
            else
            {
                timer = numToSpawn;
            }
            spawnerState = SpawnerState.WAITING;
        }
    }

    IEnumerator UpdateCount()
    {
        Debug.Log("Update Count");
        while (true)
        {
            photonView.RPC("UpdateCounter", RpcTarget.All, deathCount);
            yield return new WaitForSeconds(.2f);
        }
    }

    [PunRPC]
    void SetWaveIndicator(bool b, string message)
    {
        waveIndicator.SetActive(b);
        waveText.text = message;
    }

    [PunRPC]
    void UpdateCounter(int d)
    {
        deadCount = d;;
    }

    [PunRPC]
    void UpdateWave(int w)
    {
        waveCount = w;
    }
}
