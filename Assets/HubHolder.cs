using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class HubHolder : MonoBehaviourPunCallbacks {

    [Tooltip("UI Button for the ready check in the Player's Hub")]
    [SerializeField]
    private Button readyButton;

    public ColorBlock readyColors;
    public ColorBlock notReadyColors;

    public GameObject joinUI;
    public GameObject playerUi;
    public Image[] characterImages;   
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI charName;
    public TextMeshProUGUI charInfo;

    private HubHolder[] hubs;
    private LobbyManager lobbyManager;

    private int currentCharacterIndex;
    private int maxCharacterIndex;
    private bool joined = false;

    private bool playerIsReady;
    private bool playerInSlot;
    private bool occupied;
    private int viewId;

    public bool Occupied
    {
        get { return occupied; }
        set { occupied = value; }
    }

    private void Awake()
    {
        lobbyManager = FindObjectOfType<LobbyManager>();
        readyButton.colors = notReadyColors;
    }

    private void Start()
    {
        hubs = FindObjectsOfType<HubHolder>();
        currentCharacterIndex = 0;
        characterImages[0].gameObject.SetActive(true);
        maxCharacterIndex = characterImages.Length - 1;
    }

    public void JoinSlot()
    {
        foreach(HubHolder hub in hubs)
        {
            if(hub.GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer)
            {
                hub.photonView.TransferOwnership(0);
                hub.photonView.RPC("ResetHub", RpcTarget.AllBuffered, null);
            }
        }

        OnPlayerInSlot();
        photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
        photonView.RPC("JoinButton", RpcTarget.AllBuffered, null);
    }

    public void OnPlayerInSlot()
    {
        playerInSlot = true;
        readyButton.interactable = true;
    }

    public void OnButtonRight()
    {
        if(photonView.IsMine)
        {
            if (currentCharacterIndex < maxCharacterIndex)
            {
                photonView.RPC("RightButton", RpcTarget.AllBuffered, null);
            }
        }
    }

    public void OnButtonLeft()
    {
        if(photonView.IsMine)
        {
            if (currentCharacterIndex > 0)
            {
                photonView.RPC("LeftButton", RpcTarget.AllBuffered, null);
            }
        }
    }

    public void ReadyClick()
    {

        photonView.RPC("OnReadyClick", RpcTarget.AllBuffered, null);
    }

    [PunRPC]
    void ResetHub()
    {
        joinUI.gameObject.SetActive(true);
        playerUi.gameObject.SetActive(false);
        playerIsReady = false;
        playerInSlot = false;
        nameText.text = "";
    }

    [PunRPC]
    void JoinButton()
    {
        joinUI.gameObject.SetActive(false);
        playerUi.gameObject.SetActive(true);
        nameText.text = photonView.Owner.NickName;
        photonView.RPC("UpdateChracterInfo", RpcTarget.AllBuffered, null);
    }

    [PunRPC]
    void RightButton()
    {
        characterImages[currentCharacterIndex].gameObject.SetActive(false);
        currentCharacterIndex++;
        characterImages[currentCharacterIndex].gameObject.SetActive(true);
        photonView.RPC("UpdateChracterInfo", RpcTarget.AllBuffered, null);
    }

    [PunRPC]
    void LeftButton()
    {
        characterImages[currentCharacterIndex].gameObject.SetActive(false);
        currentCharacterIndex--;
        characterImages[currentCharacterIndex].gameObject.SetActive(true);
        photonView.RPC("UpdateChracterInfo", RpcTarget.AllBuffered, null);
    }

    [PunRPC]
    void UpdateChracterInfo()
    {
        charName.text = characterImages[currentCharacterIndex].GetComponent<CharacterInfo>().characterName;
        charInfo.text = characterImages[currentCharacterIndex].GetComponent<CharacterInfo>().characterInfo;
    }

    [PunRPC]
    void OnReadyClick()
    {
        playerIsReady = !playerIsReady;
        if (playerIsReady)
        {
            print("ready");
            readyButton.colors = readyColors;
            lobbyManager.PlayersReady += 1;
        }
        else
        {
            print("not ready");
            readyButton.colors = notReadyColors;
            lobbyManager.PlayersReady -= 1;
        }
    }
}
