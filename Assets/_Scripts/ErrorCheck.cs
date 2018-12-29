using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ErrorCheck : MonoBehaviour {

    [Tooltip("The UI Text to update for error logs")]
    [SerializeField]
    private Text errorText;

    [Tooltip("The UI Panel for error logs")]
    [SerializeField]
    private GameObject errorPanel;


    void Start ()
    {
        OnErrorMessage("Invalid password. Please contact room's creator for access.");
    }

    void OnErrorMessage(string s)
    {
        errorText.text = s;
    }

    public void ReturnToLauncher()
    {    
        PhotonNetwork.LoadLevel("Launcher");
    }
}
