using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerConnectionManager : MonoBehaviourPunCallbacks //, IPunObservable
{

    #region Public Fields

    public enum GameState { Lobby, Playing, }
    public GameState gamestate = GameState.Lobby;

    [Tooltip("The current Password of our player")]
    public string password = "";

    [Tooltip("The current Health of our player")]
    public float Health = 1f;

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject localPlayerInstance;

    #endregion

    #region Private Fields

    [Tooltip("The Player's GAME UI GameObject Prefab")]
    [SerializeField]
    private GameObject playerGameUIPrefab;

    [Tooltip("The Player's LOBBY UI GameObject Prefab")]
    [SerializeField]
    private GameObject playerLobbyUIPrefab;

    private bool isPlayerGameUILoaded;
    private bool isPlayerLobbyUILoaded;
    public LobbyManager lm;
    Vector2 openPos;
    public int viewToSend;

    #endregion


    #region MonoBehaviour CallBacks

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
    /// </summary>
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

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during initialization phase.
    /// </summary>
    public void Start()
    {
        lm = FindObjectOfType<LobbyManager>();
        //CameraWork cameraWork = gameObject.GetComponent<CameraWork>();
        
        //if (cameraWork != null)
        //{
        //    if (photonView.IsMine)
        //    {
        //        cameraWork.OnStartFollowing();
        //    }
        //}
        //else
        //{
        //    Debug.LogError("<Color=Red><b>Missing</b></Color> CameraWork Component on player Prefab.", this);
        //}
    }

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity on every frame.
    /// Process Inputs if local player.
    /// Watch for end of game, when local player health is 0.
    /// </summary>
    public void Update()
    {
        // we only process Inputs and check health if we are the local player
        if (photonView.IsMine)
        {

            if(gamestate == GameState.Playing)
            {
                if (!isPlayerGameUILoaded)
                {
                    GameObject GUiGo = PhotonNetwork.Instantiate(playerGameUIPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
                    Debug.Log("Instantiate Game UI with viewID" + GUiGo.GetPhotonView().ViewID);
            
                    isPlayerGameUILoaded = true;
                }
            
                ProcessInputs();
            
                if (Health <= 0f)
                {
                    GameConnectionManager.Instance.LeaveRoom();
                }
            }
            
            if (gamestate == GameState.Lobby)
            {
                if (!isPlayerLobbyUILoaded)
                {
                    foreach(GameObject lobbyPosition in lm.lobbySlots)
                    {
                        if (!lobbyPosition.GetComponent<HubHolder>().Occupied)
                        {
                            openPos = lobbyPosition.GetComponent<RectTransform>().localPosition;
                            break;
                        }
                    }
            
                    print("Load Lobby UI");
            
                    PhotonView pv = PhotonView.Get(lm);
                    GameObject LUiGo = PhotonNetwork.Instantiate(playerLobbyUIPrefab.name, openPos, Quaternion.identity, 0);
                    viewToSend = LUiGo.GetPhotonView().ViewID;
                    pv.RPC("CheckForAvailablePosition", RpcTarget.AllBuffered, viewToSend);
                    Debug.Log("Instantiate Lobby UI with viewID" + LUiGo.GetPhotonView().ViewID);
            
                    isPlayerLobbyUILoaded = true;        
                }
            }
        }
    }

    #endregion


    #region Private Methods

    /// <summary>
    /// Processes the inputs. This MUST ONLY BE USED when the player has authority over this Networked GameObject (photonView.isMine == true)
    /// </summary>
    void ProcessInputs()
    {
        // inputs
    }

    #endregion

    #region IPunObservable implementation

    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    {
    //        // We own this player: send the others our data
    //        stream.SendNext(this.Health);
    //    }
    //    else
    //    {
    //        // Network player, receive data
    //        this.Health = (float)stream.ReceiveNext();
    //    }
    //}

    #endregion
}
