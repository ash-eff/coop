using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks {

    #region Private Variables;

    private string gameVersion = "1";
    //private ExitGames.Client.Photon.Hashtable password;
    private bool isConnecting = false;
    private bool isCreating = false;
    private bool lobbyVisibility;

    [Tooltip("The UI Panel to let the user enter name, connect and play")]
    [SerializeField]
    private GameObject controlPanel;

    [Tooltip("The Ui Text to inform the user about the connection progress")]
    [SerializeField]
    private Text feedbackText;

    [Tooltip("The UI Label to inform the user that the connection is in progress")]
    [SerializeField]
    private GameObject progressLabel;

    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    [SerializeField]
    private byte maxPlayersPerRoom = 4;

    [Tooltip("The UI Input for entering the name of the Room to Create or Join")]
    [SerializeField]
    private InputField roomInput;

    [Tooltip("The UI Toggle for Room Visibility in the Lobby")]
    [SerializeField]
    private Toggle visibilityToggle;

    #endregion


    #region MonoBehaviour CallBacks

    private void Awake()
    {
        // #Critical
        // This makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same
        // room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;

        // set server to US ?
        //PhotonNetwork.ConnectToRegion("us");
    }

    private void Start()
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }

    #endregion


    #region Public Methods

    /// <summary>
    ///  Start the connection process.
    ///  If already connection, we attempt join a random room
    ///  If not yet connected, Connect this application instance to Photon Cloud Network
    /// </summary>
    public void Connect()
    {
        //PasswordManager.Instance.passwordToCheck = passwordInput.text;
        // we want to make sure the log is clear everytime we connect, we might have several failed attempted if connection failed.
        feedbackText.text = "";

        isConnecting = true;
        //passwordToCheck[0] = passwordInput.text.ToString();
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);

        // we check if we are connected or not, we join if we are, else we initiate the connection to the server.
        if (PhotonNetwork.IsConnected)
        {
            LogFeedback("Joining Room Named " + roomInput.text);
            Debug.Log("Joining Room Named " + roomInput.text);
            // #Critical we need at this point to attempt joining a Random Room. Failure calls OnJoinRoomFailed()
            //PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            LogFeedback("Connecting to Master...");
            // #Critical we must first and foremost connect to the Photon Online Server
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();

        }
    }

    public void Create()
    {
        //PasswordManager.Instance.password = passwordInput.text;

        lobbyVisibility = visibilityToggle.isOn;
        // we want to make sure the log is clear everytime we connect, we might have several failed attempted if connection failed.
        feedbackText.text = "";

        isCreating = true;
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);

        LogFeedback("Connecting to Master...");
        // #Critical we must first and foremost connect to the Photon Online Server
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();
    }

    #endregion


    #region Private Methods

    /// <summary>
    /// Logs the feedback in the UI view for the player, as opposed to inside the Unity Editor for the developer.
    /// </summary>
    /// <param name="message">Message.</param>
    void LogFeedback(string message)
    {
        // we do not assume there is a feedbackText defined.
        if (feedbackText == null)
        {
            return;
        }

        // add new messages as a new line and at the bottom of the log.
        feedbackText.text += System.Environment.NewLine + message;
    }

    void SettingsToDefault()
    {
        isConnecting = false;
        isCreating = false;
    }

    #endregion


    #region MonoBehaviourPunCallbacks CallBacks

    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            LogFeedback("Connected To Master: Trying to Join Room Named: " + roomInput.text);
            Debug.Log("LAUNCHER: OnConnectedToMaster() Connecting was called by PUN");
            // #Critical: The first thing we try to do is join a potential existing room. If there is, good
            // else we will be called back with OnJoinRoomFailed()
            PhotonNetwork.JoinRoom(roomInput.text);
        }

        if (isCreating)
        {
            LogFeedback("Connected To Master: Trying to Create Room Named: " + roomInput.text);
            Debug.Log("LAUNCHER: OnConnectedToMaster() Creating was called by PUN");
            Debug.Log("OnConnectedToMaster: Trying to Create " + roomInput.text);
            PhotonNetwork.CreateRoom(roomInput.text, new RoomOptions { IsVisible = lobbyVisibility, MaxPlayers = this.maxPlayersPerRoom });
        }
    }

    public override void OnCreatedRoom()
    {
        LogFeedback("<Color=Green>Created Room Named:</Color> " + roomInput.text);
        Debug.Log("LAUNCHER: OnCreatedRoom() was called by PUN. Now this client has created a room.");
        PhotonNetwork.LoadLevel("Lobby");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("OnCreateRoomFailed " + returnCode);
        Debug.Log("OnJCreateRoomFailed " + message);
        LogFeedback("<Color=red>Failed to Creat Room:</Color> " + message);
        PhotonNetwork.Disconnect();
    }

    public override void OnJoinedRoom()
    {
        LogFeedback("<Color=Green>Joined Room:</Color> " + roomInput.text);
        Debug.Log("LAUNCHER: OnJoinedRoom() was called by PUN. Now this client is in a room.");

        PhotonNetwork.LoadLevel("Lobby");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRoomFailed " + returnCode);
        Debug.Log("OnJoinRoomFailed " + message);
        LogFeedback("<Color=Red>Failed to Join Room:</Color> " + message + " <Color=Red>Contact room administrator for help.</Color>");
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        LogFeedback("<Color=Red>Disconnected:</Color> " + cause);
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
        Debug.Log("LAUNCHER: OnDisconnected was called by PUN with the reason {0} " + cause);
        SettingsToDefault();
    }

    #endregion
}
