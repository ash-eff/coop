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

    [Tooltip("UI Button used to start the game after a ready check.")]
    [SerializeField]
    private Button startButton;

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
        DontDestroyOnLoad(this);
        foreach(GameObject character in characters)
        {
            character.GetComponent<CharacterInfo>().WakeUp();
        }
    }

    private void CheckForReady()
    {
        // FOR TESTING
        startButton.interactable = true;
        //if (PhotonNetwork.IsMasterClient)
        //{
        //    if(PhotonNetwork.PlayerList.Length >= 2 && playersReady == PhotonNetwork.PlayerList.Length)
        //    {
        //        startButton.interactable = true;
        //    }
        //    else
        //    {
        //        startButton.interactable = false;
        //    }
        //}
    }

    public void LoadMainLevel()
    {
        PhotonNetwork.LoadLevel("Main");
    }
}
