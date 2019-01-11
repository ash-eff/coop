﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerConnectionManager : MonoBehaviourPunCallbacks //, IPunObservable
{

    #region Public Fields

    public Color characterColor;

    public enum GameState { Lobby, Playing, }
    public GameState gamestate = GameState.Lobby;

    [Tooltip("The current Password of our player")]
    public string password = "";

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject localPlayerInstance;

    #endregion

    #region Private Fields

    [Tooltip("The Player's GAME UI GameObject Prefab")]
    [SerializeField]
    private GameObject playerGameUIPrefab;

    // TODO make this so that it can be set from the lobby from a drop down menu
    [Tooltip("The Player's GAME Character Prefab")]
    [SerializeField]
    private GameObject playerCharacterPrefab;

    [Tooltip("The Player's LOBBY UI GameObject Prefab")]
    [SerializeField]
    private GameObject playerLobbyUIPrefab;

    private bool isPlayerGameUILoaded;
    private bool isPlayerLobbyUILoaded;
    private LobbyManager lm;
    private Vector2 openPos;
    public int viewToSend;

    #endregion


    #region MonoBehaviour CallBacks

    public override void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void Awake()
    {
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine)
        {
            // the local player instance os equal to the GameObject that this gameobject is attached to
            localPlayerInstance = gameObject;
        }

        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(gameObject);
        
    }

    public void Start()
    {
        lm = FindObjectOfType<LobbyManager>();

    }

    public void Update()
    {  
        // we only process Inputs and check health if we are the local player
        if (photonView.IsMine)
        {
            if (gamestate == GameState.Playing)
            {
                if (!isPlayerGameUILoaded)
                {
                    Debug.Log("INSTANTIATE OBJECT FOR " + photonView.Owner.NickName);
                    GameObject playerChar = PhotonNetwork.Instantiate(playerCharacterPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
                    playerChar.gameObject.transform.SetParent(transform, false);
                    playerChar.GetPhotonView().RPC("CharacterColor", RpcTarget.AllBuffered, new Vector3(characterColor.r, characterColor.g, characterColor.b));


                    //GameObject GUiGo = PhotonNetwork.Instantiate(playerGameUIPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
                    //GUiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);

                    //Debug.Log("Instantiate Game UI with viewID" + GUiGo.GetPhotonView().ViewID);
                    //Debug.Log("Instantiate Player Character with viewID" + playerChar.GetPhotonView().ViewID);

                    isPlayerGameUILoaded = true;
                }
            
        
            }
        
            //if (gamestate == GameState.Lobby)
            //{
            //    if (!isPlayerLobbyUILoaded)
            //    {
            //        foreach(GameObject lobbyPosition in lm.lobbySlots)
            //        {
            //            if (!lobbyPosition.GetComponent<HubHolder>().Occupied)
            //            {
            //                openPos = lobbyPosition.GetComponent<RectTransform>().localPosition;
            //                break;
            //            }
            //        }
            //
            //        print("Load Lobby UI");
            //
            //        PhotonView pv = PhotonView.Get(lm);
            //        GameObject LUiGo = PhotonNetwork.Instantiate(playerLobbyUIPrefab.name, openPos, Quaternion.identity, 0);
            //        viewToSend = LUiGo.GetPhotonView().ViewID;
            //        pv.RPC("CheckForAvailablePosition", RpcTarget.AllBuffered, viewToSend);
            //        Debug.Log("Instantiate Lobby UI with viewID" + LUiGo.GetPhotonView().ViewID);
            //
            //        isPlayerLobbyUILoaded = true;   
            //    
            //    }
            //}
        } 
    }

    #endregion


    #region Private Methods

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
    {
        if (photonView.IsMine)
        {
            if (scene.name == "Main")
            {
                GetComponent<PlayerInput>().enabled = true;
                GetComponent<CameraWork>().enabled = true;
                CameraWork cameraWork = gameObject.GetComponent<CameraWork>();

                if (cameraWork != null)
                {
                    if (photonView.IsMine)
                    {
                        cameraWork.OnStartFollowing();
                    }
                }
                else
                {
                    Debug.LogError("<Color=Red><b>Missing</b></Color> CameraWork Component on player Prefab.", this);
                }

                gamestate = GameState.Playing;
            }
        }    
    }

    //[PunRPC]
    void UpdateCharacterColor(Vector3 col)
    {
        if (photonView.IsMine)
        {
            Vector3 _color = col;
            characterColor.r = col.x;
            characterColor.g = col.y;
            characterColor.b = col.z;
            characterColor.a = 1;
        }
    }

    #endregion
}
