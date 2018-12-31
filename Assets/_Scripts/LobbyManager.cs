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
    public GameObject[] lobbySlots;
    public Button startButton;

    private GameConnectionManager gameConnection;
    private PhotonView pView;
    
    public int PlayersReady
    {
        get { return playersReady; }
        set { playersReady = value; CheckForReady(); }
    }

    [PunRPC]
    public void CheckForAvailablePosition(int id)
    {
        foreach (GameObject lobbySlot in lobbySlots)
        {
            if (lobbySlot.GetComponent<HubHolder>().occupied)
            {
                Debug.Log("Skipping " + lobbySlot.name);
                continue;
            }
            else
            {
                lobbySlot.GetComponent<HubHolder>().Occupied = true;
                lobbySlot.GetComponent<HubHolder>().viewId = id;
                break;
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PlayerConnectionManager[] pcms = FindObjectsOfType<PlayerConnectionManager>();
        bool numberInList = false;
        foreach (GameObject lobbySlot in lobbySlots)
        {
            foreach (PlayerConnectionManager pcm in pcms)
            {
                numberInList = false;

                if(lobbySlot.GetComponent<HubHolder>().viewId !=0)
                {
                    if (lobbySlot.GetComponent<HubHolder>().viewId == pcm.viewToSend)
                    {
                        numberInList = true;
                        break;
                    }

                }
            }

            if (!numberInList)
            {
                lobbySlot.GetComponent<HubHolder>().Occupied = false;
                lobbySlot.GetComponent<HubHolder>().viewId = 0;
            }

        }
    }

    private void CheckForReady()
    {
        // FOR TESTING
        // startButton.interactable = true;
        if (PhotonNetwork.IsMasterClient)
        {
            if(PhotonNetwork.PlayerList.Length >= 2 && playersReady == PhotonNetwork.PlayerList.Length)
            {
                startButton.interactable = true;
            }
            else
            {
                startButton.interactable = false;
            }
        }
    }

    public void LoadMainLevel()
    {
        PhotonNetwork.LoadLevel("Main");
    }
}
