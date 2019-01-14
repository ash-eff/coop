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
    public TextMeshProUGUI ownerName;
    public Color charColor;

    public PlayerConnectionManager hubPCM;

    private HubHolder[] hubs;
    private LobbyManager lobbyManager;
    private PlayerConnectionManager[] pcms;

    private int currentCharacterIndex;
    private int maxCharacterIndex;
    private bool joined = false;

    public bool playerIsReady;
    public bool playerInSlot;
    public bool occupied;
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

    public void JoinButtonPressed()
    {
        //foreach(HubHolder hub in hubs)
        //{
        //    if (hub.GetComponent<PhotonView>().IsMine)
        //    {
        //        if(hub.GetComponent<PhotonView>().ViewID == photonView.ViewID)
        //        {
        //            //Debug.Log("SKIP " + hub.gameObject.name);
        //        }
        //        else
        //        {
        //            hub.photonView.RPC("ResetHub", RpcTarget.All, null);
        //        }
        //    }
        //}

        StartCoroutine(RequestTransfer());
    }

    IEnumerator RequestTransfer()
    {
        photonView.RequestOwnership();

        while (photonView.Owner == null)
        {
            yield return new WaitForEndOfFrame();
        }

        //OnPlayerInSlot();
        AddPlayerConnectionManager();
    }

    public void OnPlayerInSlot()
    {
        playerInSlot = true;
        readyButton.interactable = true;
    }

    public void OnRightButtonPressed()
    {
        if(photonView.IsMine)
        {
            if (currentCharacterIndex < maxCharacterIndex)
            {
                photonView.RPC("RightButton", RpcTarget.AllBuffered, null);
            }
        }
    }

    public void OnLeftButtonPressed()
    {
        if(photonView.IsMine)
        {
            if (currentCharacterIndex > 0)
            {
                photonView.RPC("LeftButton", RpcTarget.AllBuffered, null);
            }
        }
    }

    public void OnReadyButtonPressed()
    {
        photonView.RPC("OnReadyClick", RpcTarget.AllBuffered, null);
    }

    public void AddPlayerConnectionManager()
    {
        pcms = FindObjectsOfType<PlayerConnectionManager>();
        foreach (PlayerConnectionManager pcm in pcms)
        {
            if (pcm.GetComponent<PhotonView>().IsMine)
            {
                hubPCM = pcm;
            }
        }

        photonView.RPC("JoinHub", RpcTarget.AllBuffered, null);
    }

    private void UpdateCharacterColor()
    {
        if(photonView.IsMine)
        {
            charColor = characterImages[currentCharacterIndex].GetComponent<Image>().color;
            hubPCM.characterColor = charColor;
        }
    }

    [PunRPC]
    void ResetHub()
    {
        photonView.TransferOwnership(0);
        joinUI.gameObject.SetActive(true);
        playerUi.gameObject.SetActive(false);
        playerIsReady = false;
        playerInSlot = false;
        nameText.text = "";
        hubPCM = null;
        readyButton.colors = notReadyColors;
        photonView.RPC("UpdateHubName", RpcTarget.AllBuffered, null);
    }

    [PunRPC]
    void JoinHub()
    {
        Debug.Log("Joining hub: " + photonView.name);
        joinUI.gameObject.SetActive(false);
        playerUi.gameObject.SetActive(true);
        
        photonView.RPC("UpdateHubName", RpcTarget.AllBuffered, null);
        photonView.RPC("UpdateChracterInfo", RpcTarget.AllBuffered, null);
        UpdateCharacterColor();
    }

    [PunRPC]
    void RightButton()
    {
        characterImages[currentCharacterIndex].gameObject.SetActive(false);
        currentCharacterIndex++;
        characterImages[currentCharacterIndex].gameObject.SetActive(true);
        photonView.RPC("UpdateChracterInfo", RpcTarget.AllBuffered, null);
        UpdateCharacterColor();
    }

    [PunRPC]
    void LeftButton()
    {
        characterImages[currentCharacterIndex].gameObject.SetActive(false);
        currentCharacterIndex--;
        characterImages[currentCharacterIndex].gameObject.SetActive(true);
        photonView.RPC("UpdateChracterInfo", RpcTarget.AllBuffered, null);
        UpdateCharacterColor();
    }

    [PunRPC]
    void UpdateHubName()
    {
        //if (photonView.Owner == null)
        //{
        //    nameText.text = "Join";
        //    ownerName.text = "Scene";
        //}
        //if (photonView.Owner != null)
        //{
            nameText.text = photonView.Owner.NickName;
            ownerName.text = photonView.Owner.NickName;
        //}
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
