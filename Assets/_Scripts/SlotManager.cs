using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class SlotManager : MonoBehaviourPunCallbacks {

    public bool occupied = false;
    private Button button;
    private PhotonView pView;
    private LobbyManager lobbyManager;

    private void Start()
    {
        lobbyManager = FindObjectOfType<LobbyManager>();
        pView = GetComponent<PhotonView>();
        button = GetComponentInChildren<Button>();
    }

    public void OnButtonClick()
    {
        PhotonView pv = PhotonView.Get(lobbyManager);
        pv.RPC("ButtonClick", RpcTarget.AllBuffered, null);
        pView.RPC("ButtonClick", RpcTarget.AllBuffered, null);
    }

    [PunRPC]
    public void ButtonClick()
    {
        occupied = true;
        button.gameObject.SetActive(false);
    }
}
