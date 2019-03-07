using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public static int playersReady;

    static public LobbyManager Instance;
    private LobbyManager instance;

    //[Tooltip("UI Button used to start the game after a ready check.")]
    //[SerializeField]
    public GameObject startButton;

    [Tooltip("The available characters to choose from.")]
    public GameObject[] characters;

    private GameConnectionManager gameConnection;
    private PhotonView pView;

    public int PlayersReady
    {
        get { return playersReady; }
        set { playersReady = value; CheckForReady(); }
    }

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        startButton = GameObject.Find("Start Button");

        foreach (GameObject character in characters)
        {
            character.GetComponent<CharacterInfo>().WakeUp();
        }
    }

    private void CheckForReady()
    {
        // FOR TESTING
        //startButton.GetComponent<Button>().interactable = true;
        if (PhotonNetwork.IsMasterClient)
        {
            if(PhotonNetwork.PlayerList.Length >= 2 && playersReady == PhotonNetwork.PlayerList.Length)
            {
                startButton.GetComponent<Button>().interactable = true;
            }
            else
            {
                startButton.GetComponent<Button>().interactable = false;
            }
        }
    }

    public void LoadMainLevel()
    {
        PhotonNetwork.LoadLevel("Main");
    }
}
