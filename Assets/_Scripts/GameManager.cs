using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    static public GameManager Instance;
    public static int playersDead;

    public Button returnButton;
    public GameObject minimap;
    public GameObject infoScreen;
    public GameObject menu;
    public int pDead;

    private EnemySpawner spawner;
    private int numberOfPlayers;
    private float gameLengthInTime;
    public bool gameOver;
    public PlayerEndStats[] playerStats;

    private bool menuOpen;

    // Start is called before the first frame update
    void Start()
    {
        playersDead = 0;
        minimap.SetActive(true);
        infoScreen.SetActive(false);
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


        if (Input.GetKeyDown(KeyCode.Escape) && !gameOver)
        {
            menuOpen = !menuOpen;
        }

        if (menuOpen && !gameOver)
        {
            menu.SetActive(true);
        }
        else
        {
            menu.SetActive(false);
        }

    }

    void GameOver()
    {
        minimap.SetActive(false);
        spawner.spawnerState = EnemySpawner.SpawnerState.GAMEOVER;
        gameLengthInTime = Time.deltaTime;
        StartCoroutine(InformationScreen());
    }

    [PunRPC]
    public void UpdateStats(string n, float _deathTime, int _zombiesKilled, int _waveNumber, float _accuracy, int _numOfReloads, int _shotsFired, float _damageTaken, float _friendlyTaken, float _friendlyCaused, float _damageHealed)
    {
        string convertedTime = ConvertTime(_deathTime);
        foreach (PlayerEndStats slot in playerStats)
        {
            if (slot.available)
            {
                slot.available = false;
                slot.PlayerName = n;
                slot.DeathTime = convertedTime;
                slot.ZombiesKilled = _zombiesKilled;
                slot.WaveNumber = _waveNumber;
                slot.Accuracy = _accuracy.ToString() + "%";
                slot.NumOfReloads = _numOfReloads;
                slot.ShotsFired = _shotsFired;
                slot.DamageTaken = Mathf.RoundToInt(_damageTaken);
                slot.FriendlyFireTaken = Mathf.RoundToInt(_friendlyTaken);
                slot.FriendlyFireCaused = Mathf.RoundToInt(_friendlyCaused);
                slot.DamageHealed = Mathf.RoundToInt(_damageHealed);
                slot.Populate();
                break;
            }
        }
    }

    string ConvertTime(float t)
    {
        string hours = Mathf.Floor(t / 3600).ToString("00");
        string minutes = Mathf.Floor(t / 60).ToString("00");
        string seconds = Mathf.Floor(t % 60).ToString("00");
        string convertedTime = hours + ":" + minutes + ":" + seconds;

        return convertedTime;
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
