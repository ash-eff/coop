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
    public HubHolder[] hubs;
    public Button startButton;

    private GameConnectionManager gameConnection;
    private PhotonView pView;
    
    public int PlayersReady
    {
        get { return playersReady; }
        set { playersReady = value; CheckForReady(); }
    }

    //[PunRPC]
    //public void CheckForAvailablePosition(int id)
    //{
    //    foreach (HubHolder hub in hubs)
    //    {
    //        if (lobbySlot.GetComponent<HubHolder>().occupied)
    //        {
    //            Debug.Log("Skipping " + lobbySlot.name);
    //            continue;
    //        }
    //        else
    //        {
    //            lobbySlot.GetComponent<HubHolder>().Occupied = true;
    //            lobbySlot.GetComponent<HubHolder>().viewId = id;
    //            break;
    //        }
    //    }
    //}

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        foreach (HubHolder hub in hubs)
        {
            if(hub.photonView.Owner == null)
            {
                print("No Owner");
                hub.photonView.RPC("ResetHub", RpcTarget.AllBuffered, null);
            }
        }
    }

    private void CheckForReady()
    {
        // FOR TESTING
        //startButton.interactable = true;
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
