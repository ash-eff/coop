using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerHub : MonoBehaviourPunCallbacks {

    public ColorBlock readyColors;
    public ColorBlock notReadyColors;
    public bool playerInSlot;
    public bool playerIsReady;
    private PhotonView pView;
    public PlayerConnectionManager playerConnection;

    public GameObject player;

    [Tooltip("UI Image for indicating the selection of Player's Hub")]
    [SerializeField]
    private Image selection;

    [Tooltip("UI Button for the ready check in the Player's Hub")]
    [SerializeField]
    private Button readyButton;

    [Tooltip("UI Panel that hold everything")]
    [SerializeField]
    public GameObject panel;

    private void Awake()
    {
        pView = GetComponent<PhotonView>();
        readyButton.colors = notReadyColors;
        transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);

        if (pView.IsMine)
        {
            OnPlayerInSlot();
        }
    }

    private void OnDestroy()
    {

        // TODO causes an error when the player disconnects because it's trying to access the lobby manager after it's destroyed?
        //PhotonView pv = PhotonView.Get(lm);

        //pv.RPC("RemoveFromPosition", RpcTarget.AllBuffered, (int)pView.ViewID);
    }

    public void OnPlayerInSlot()
    {
        playerInSlot = true;
        selection.GetComponent<Image>().enabled = true;
        readyButton.interactable = true;
    }

    public void OnReadyClick()
    {
        pView.RPC("ReadyClick", RpcTarget.AllBuffered, null);
    }

    [PunRPC]
    public void ReadyClick()
    {
        playerIsReady = !playerIsReady;
        if (playerIsReady)
        {
            readyButton.colors = readyColors;
        }
        else
        {
            readyButton.colors = notReadyColors;
        }
    }
}
