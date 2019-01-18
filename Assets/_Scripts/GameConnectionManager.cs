using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class GameConnectionManager : MonoBehaviourPunCallbacks
{

    static public GameConnectionManager Instance;
    private GameObject instance;

    ExitGames.Client.Photon.Hashtable password = new ExitGames.Client.Photon.Hashtable();
    ExitGames.Client.Photon.Hashtable clientPassword = new ExitGames.Client.Photon.Hashtable();
    public List<string> storedClientPasswords = new List<string>();
    public List<string> storedMasterPasswords = new List<string>();
    private bool isInError;
    private string playerName;

    [Tooltip("The prefab to use for representing the player")]
    [SerializeField]
    private GameObject playerConnectionPrefab;

    [Tooltip("The UI Text to use for displaying ping")]
    [SerializeField]
    private Text pingText;

    private LobbyManager lobbyManager;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // in case we started this demo with the wrong scene being active, simply load the menu scene
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("Launcher");

            return;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            password.Add("pw", PasswordManager.Instance.password.ToString());
            PhotonNetwork.MasterClient.SetCustomProperties(propertiesToSet: password);
            foreach (string value in PhotonNetwork.MasterClient.CustomProperties.Values)
            {
                storedMasterPasswords.Add(value);
            }

            InstantiatePlayer();
        }
        else
        {
            clientPassword.Clear();
            clientPassword.Add("pw", PasswordManager.Instance.password.ToString());
            PhotonNetwork.LocalPlayer.SetCustomProperties(propertiesToSet: clientPassword);

            foreach (string value in PhotonNetwork.MasterClient.CustomProperties.Values)
            {
                storedMasterPasswords.Add(value);
            }

            foreach (string value in PhotonNetwork.LocalPlayer.CustomProperties.Values)
            {
                storedClientPasswords.Add(value);
            }

            if (storedMasterPasswords[0] != storedClientPasswords[0])
            {
                Debug.Log("PASSWORD IN ERROR!!");
                isInError = true;
                LeaveRoom();
            }
            else
            {
                Debug.Log("PASSWORD MATCHES!!");
                InstantiatePlayer();
            }
        }

        StartCoroutine(UpdatePing());
    }

    IEnumerator UpdatePing()
    {
        while (true)
        {
            pingText.text = PhotonNetwork.GetPing().ToString();

            yield return new WaitForSeconds(1f);
        }

    }

    #region Photon CallBacks

    public override void OnLeftRoom()
    {
        if (isInError)
        {
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene("ErrorCheck");
        }
        else
        {
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene(0);
        }
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you are the player connecting.

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", otherPlayer.NickName); // seen when otherPlayer disconnects


        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
            //LoadArena();
        }
    }

    #endregion


    #region Public Methods

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void InstantiatePlayer()
    {
        if (playerConnectionPrefab == null)
    
        { // #Tip Never assume public properties of Components are filled up properly, always check and inform the developer of it.
    
            Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
        }
        else
        {
            if (PlayerConnectionManager.localPlayerInstance == null)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
    
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                PhotonNetwork.Instantiate(playerConnectionPrefab.name, Vector2.zero, Quaternion.identity, 0);
            }
            else
            {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
        }
    }
    
    #endregion
}
