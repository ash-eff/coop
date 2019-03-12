using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    static public GameManager Instance;
    public static int playersDead = 0;
    
    public GameObject infoScreen;
    public int pDead;

    private EnemySpawner spawner;
    private int numberOfPlayers;
    private float gameLengthInTime;
    public bool gameOver;
    public PlayerEndStats[] playerStats;

    // Start is called before the first frame update
    void Start()
    {
        spawner = FindObjectOfType<EnemySpawner>();

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        numberOfPlayers = PhotonNetwork.PlayerList.Length;
    }

    private void Update()
    {
        pDead = playersDead;
        if(playersDead == numberOfPlayers && !gameOver)
        {
            gameOver = true;
            GameOver();
        }
    }

    void GameOver()
    {
        spawner.spawnerState = EnemySpawner.SpawnerState.GAMEOVER;
        gameLengthInTime = Time.time;
        StartCoroutine(InformationScreen());
    }

    //[PunRPC]
    public void UpdateStats(string n, float _deathTime, int _zombiesKilled, float _accuracy, int _numOfReloads, int _shotsFired, float _damageTaken, float _damageHealed)
    {
        foreach (PlayerEndStats slot in playerStats)
        {
            if (slot.available)
            {
                slot.available = false;
                slot.PlayerName = n;
                slot.DeathTime = _deathTime;
                slot.ZombiesKilled = _zombiesKilled;
                slot.Accuracy = _accuracy;
                slot.NumOfReloads = _numOfReloads;
                slot.ShotsFired = _shotsFired;
                slot.DamageTaken = _damageTaken;
                slot.DamageHealed = _damageHealed;
                slot.Populate();
                break;
            }
        }
    }

    //public PlayerEndStats Stats()
    //{
    //    foreach (PlayerEndStats slot in playerStats)
    //    {
    //        if (!slot.available)
    //        {
    //            // skip
    //        }
    //        else
    //        {
    //            slot.available = false;
    //            return slot;
    //        }
    //    }
    //
    //    return null;
    //}

    IEnumerator InformationScreen()
    {
        float timer = 4f;
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        photonView.RPC("SetInfoActive", RpcTarget.All, null);
    }

    [PunRPC]
    void SetInfoActive()
    {
        infoScreen.SetActive(true);    
    }
}
