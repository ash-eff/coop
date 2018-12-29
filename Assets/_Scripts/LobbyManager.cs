using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public GameObject[] lobbySlots;
    private GameConnectionManager gameConnection;
    private PhotonView pView;

    //private void Start()
    //{
    //    pView = GetComponent<PhotonView>();
    //}

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
}
