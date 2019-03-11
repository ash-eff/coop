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

    public void Stats(string n, float _deathTime, int _zombiesKilled, float _accuracy, int _numOfReloads, int _shotsFired, float _damageTaken, float _damageHealed)
    {
        // get first available slot
        PlayerEndStats workingSlot = null;
        foreach(PlayerEndStats slot in playerStats)
        {
            if (slot.available)
            {
                // skip
            }
            else
            {
                slot.available = true;
                workingSlot = slot;
                break;
            }
        }
        // populate stats
        if(workingSlot != null)
        {
            workingSlot.PlayerName = n;
            workingSlot.DeathTime = _deathTime;
            workingSlot.ZombiesKilled = _zombiesKilled;
            workingSlot.Accuracy = _accuracy;
            workingSlot.NumOfReloads = _numOfReloads;
            workingSlot.ShotsFired = _shotsFired;
            workingSlot.DamageTaken = _damageTaken;
            workingSlot.DamageHealed = _damageHealed;
            workingSlot.Populate();
        }
    }

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
