using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class HubHolder : MonoBehaviourPunCallbacks, IPunOwnershipCallbacks
{

    [Tooltip("UI Button for the ready check in the Player's Hub")]
    [SerializeField]
    private Button readyButton;

    public ColorBlock readyColors;
    public ColorBlock notReadyColors;

    public GameObject joinUI;
    public GameObject playerUi; 
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI charName;
    public TextMeshProUGUI charInfo;
    public Image characterImage;

    public GameObject currentChar;

    public PlayerConnectionManager hubPCM;

    private HubHolder[] hubs;
    private LobbyManager lobbyManager;
    private PlayerConnectionManager[] pcms;

    public int currentCharacterIndex;
    private int maxCharacterIndex;
    private bool requesting = false;

    public bool playerIsReady;
    public bool playerInSlot;
    public bool occupied = false;
    private int viewId;

    private void Awake()
    {
        lobbyManager = FindObjectOfType<LobbyManager>();
        readyButton.colors = notReadyColors;
    }

    private void Start()
    {
        hubs = FindObjectsOfType<HubHolder>();
        currentCharacterIndex = 0;
        //characterImages[0].gameObject.SetActive(true);
        maxCharacterIndex = lobbyManager.characters.Length - 1;
    }

    // Join button was pressed, start the process for joingin a hub
    public void JoinButtonPressed()
    {
        Debug.Log("JoinButtonPressed(): Join button pressed on" + photonView.name);
        foreach (HubHolder hub in hubs)
        {
            if (hub.photonView.Owner != null && hub.photonView.IsMine)
            {
                // we have access to the hubs we own
                // if the hub I own is the same as the one I clicked the join but on, then skip it
                if(hub.photonView.ViewID == photonView.ViewID)
                {
                   // SKIP!
                }
                // if it's not the same, then it's my previous hub and needs to be reset
                else
                {
                    Debug.Log("JoinButtonPressed(): " + hub.name + " was my previous hub and is being reset.");
                    hub.photonView.RPC("ResetHub", RpcTarget.All, null);
                }
            }
        }

        // you are now requesting to join a hub
        requesting = true;
        photonView.RequestOwnership();
    }

    // right arrow button clicked
    public void OnRightButtonPressed()
    {
        if(photonView.IsMine && !playerIsReady)
        {
            photonView.RPC("RightButton", RpcTarget.AllBuffered, null);       
        }
    }

    // left arrow button clicked
    public void OnLeftButtonPressed()
    {
        if(photonView.IsMine && !playerIsReady)
        {
            photonView.RPC("LeftButton", RpcTarget.AllBuffered, null);
        }
    }

    // ready check button clicked
    public void OnReadyButtonPressed()
    {
        Debug.Log("OnReadyButtonPressed");
        if (photonView.IsMine)
        {
            photonView.RPC("OnReadyClick", RpcTarget.AllBuffered, null);
        }
    }

    // add your PlayerCharacterManager script to the hub 
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
    }

    // add the chracter on your PlayerCharacterManager script so you can instantiate it later
    private void UpdateCharacter()
    {
        if (photonView.IsMine)
        {
            //charColor = characterImages[currentCharacterIndex].GetComponent<Image>().color;
            hubPCM.playerCharacterPrefab = currentChar;
        }
    }

    // let other players know when you've left a hub
    [PunRPC]
    void ResetHub()
    {
        photonView.TransferOwnership(0);
        joinUI.gameObject.SetActive(true);
        playerUi.gameObject.SetActive(false);
        playerInSlot = false;
        readyButton.interactable = false;
        nameText.text = "";
        hubPCM = null;
        readyButton.colors = notReadyColors;    
        photonView.RPC("UpdateHubName", RpcTarget.AllBuffered, "");
        if(playerIsReady)
        {
            playerIsReady = false;
            lobbyManager.PlayersReady -= 1;
        }
    }

    // let other players known when you've joined a hub
    [PunRPC]
    void JoinHub()
    {
        Debug.Log(photonView.Owner + " has been assign to " + photonView.name);
        requesting = false;
        playerInSlot = true;
        readyButton.interactable = true;
        currentChar = lobbyManager.characters[currentCharacterIndex];
        AddPlayerConnectionManager();
        UpdateCharacter();
        joinUI.gameObject.SetActive(false);
        playerUi.gameObject.SetActive(true);
        photonView.RPC("UpdateHubName", RpcTarget.AllBuffered, photonView.Owner.NickName);
        photonView.RPC("UpdateChracterInfo", RpcTarget.AllBuffered, null);
    }

    // let everyone know when you cycle through characters to the right
    [PunRPC]
    void RightButton()
    {
        bool checkingRight = true;

        while (checkingRight)
        {
            if (currentCharacterIndex + 1 > lobbyManager.characters.Length - 1)
            {
                currentCharacterIndex = 0;
            }

            //Debug.Log("Checking..." + lobbyManager.characters[currentCharacterIndex + 1].name);

            if (lobbyManager.characters[currentCharacterIndex + 1].GetComponent<CharacterInfo>().isSelectable)
            {
                checkingRight = false;
            }
            else
            {
                currentCharacterIndex ++;
                Debug.Log(lobbyManager.characters[currentCharacterIndex].name + " selectable? " + lobbyManager.characters[currentCharacterIndex].GetComponent<CharacterInfo>().isSelectable);
            }
        }

        currentCharacterIndex++;
        currentChar = null;
        currentChar = lobbyManager.characters[currentCharacterIndex];
        UpdateCharacter();
        photonView.RPC("UpdateChracterInfo", RpcTarget.AllBuffered, null);
    }

    // let everyone know when you cycle through characters to the left
    [PunRPC]
    void LeftButton()
    {
        bool checkingLeft = true;

        while (checkingLeft)
        {
            if (currentCharacterIndex - 1 <= 0)
            {
                currentCharacterIndex = lobbyManager.characters.Length;
            }

            Debug.Log("Checking..." + lobbyManager.characters[currentCharacterIndex - 1].name);

            if (lobbyManager.characters[currentCharacterIndex - 1].GetComponent<CharacterInfo>().isSelectable)
            {
                checkingLeft = false;
            }
            else
            {
                currentCharacterIndex--;
                Debug.Log(lobbyManager.characters[currentCharacterIndex].name + " selectable? " + lobbyManager.characters[currentCharacterIndex].GetComponent<CharacterInfo>().isSelectable);
            }
        }

        currentCharacterIndex--;
        currentChar = null;
        currentChar = lobbyManager.characters[currentCharacterIndex];
        UpdateCharacter();
        photonView.RPC("UpdateChracterInfo", RpcTarget.AllBuffered, null);
    }

    // tell everyone the name of the owner of the object
    [PunRPC]
    void UpdateHubName(string owner)
    {        
        nameText.text = owner;
    }

    // tell others about the chracter you selected
    [PunRPC]
    void UpdateChracterInfo()
    {
        characterImage.GetComponent<Image>().sprite = currentChar.GetComponent<SpriteRenderer>().sprite;
        charName.text = currentChar.GetComponent<CharacterInfo>().characterName;
        charInfo.text = currentChar.GetComponent<CharacterInfo>().characterInfo;
        Debug.Log("Update character info on " + photonView.name);
    }

    //[PunRPC]
    void CharacterAvailable()
    {
        foreach(HubHolder hub in hubs)
        {
            if (!hub.playerIsReady && hub.playerInSlot)
            {
                Debug.Log("Is " + hub.currentChar + " still selectable? " + hub.currentChar.GetComponent<CharacterInfo>().isSelectable);
                if (!hub.currentChar.GetComponent<CharacterInfo>().isSelectable)
                {
                    Debug.Log(hub.photonView.name + " will switch to the next available chracter.");
                    hub.OnRightButtonPressed();
                }
            }
        }
    }

    // let others know when you have done a ready check
    [PunRPC]
    void OnReadyClick()
    {
        Debug.Log("OnReadyClick");
        // turn the button into a bool switch
        playerIsReady = !playerIsReady;
        // if ready, then change the button and set player as ready
        if (playerIsReady)
        {
            readyButton.colors = readyColors;
            currentChar.GetComponent<CharacterInfo>().SelectChamp();
            lobbyManager.PlayersReady += 1;
            photonView.RPC("UpdateChracterInfo", RpcTarget.AllBuffered, null);
            //photonView.RPC("CharacterAvailable", RpcTarget.AllBuffered, null);
            CharacterAvailable();
        }
        // if not ready, then change the button and set player as not ready
        else
        {
            readyButton.colors = notReadyColors;
            currentChar.GetComponent<CharacterInfo>().SelectChamp();
            lobbyManager.PlayersReady -= 1;
            photonView.RPC("UpdateChracterInfo", RpcTarget.AllBuffered, null);
        }
    }

    // let others know that you were granted ownership over an object
    [PunRPC]
    void TransferObjectOwnership(Photon.Realtime.Player requestingPlayer)
    {
        // tell everyone that you are taking ownership of this object
        photonView.TransferOwnership(requestingPlayer);
    }

    // called when there is a request for ownership transfer
    public void OnOwnershipRequest(PhotonView targetView, Photon.Realtime.Player requestingPlayer)
    {
        // when a transfer of ownership is requested on this specific hub, the RPC TransferObjectOwnership
        if (requesting)
        {
            Debug.Log(targetView.name + ": Request Ownership Transfer from " + targetView.name + " to " + requestingPlayer);
            photonView.RPC("TransferObjectOwnership", RpcTarget.AllBuffered, requestingPlayer);
        }
    }

    // called when ownership is transfered
    public void OnOwnershipTransfered(PhotonView targetView, Photon.Realtime.Player newOwner)
    { 
        // if this hub is the object being transfered 
        if (photonView == targetView)
        {
            // and this hub isn't transfering back to the scene
            if(newOwner == null)
            {
                Debug.Log("OnOwnershipTransfered: " + targetView.name + " was transfered back to the Scene.");
            }
            // then tranfer ownership to the player
            else
            {
                Debug.Log("OnOwnershipTransfered: " + targetView.name + " was transfered to " + newOwner + " .");
                photonView.RPC("JoinHub", RpcTarget.AllBuffered, null);
            }
        }
    }
}
