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

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject localPlayerInstance;

    #endregion

    #region Private Fields

    // TODO make this so that it can be set from the lobby from a drop down menu
    [Tooltip("The Player's GAME Character Prefab")]
    public GameObject playerCharacterPrefab;

    private bool playerLoaded;

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

    public void Update()
    {  
        if (photonView.IsMine)
        {
            if (gamestate == GameState.Playing)
            {
                if (!playerLoaded)
                {
                    GameObject playerChar = PhotonNetwork.Instantiate(playerCharacterPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
                    Debug.Log("INSTANTIATE " + playerChar + " FOR " + photonView.Owner.NickName);
                    //playerChar.SendMessage("InstantiateUI");
                    playerLoaded = true;
                }        
            }
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
                gamestate = GameState.Playing;
            }
        }    
    }

    #endregion
}
