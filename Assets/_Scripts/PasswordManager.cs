using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PasswordManager : MonoBehaviourPunCallbacks {

    static public PasswordManager Instance;
    private GameObject instance;
    public string password;

    private InputField passwordInput;

    public void Awake()
    {
        passwordInput = GetComponent<InputField>();
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        //DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start ()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdatePassword()
    {
        password = passwordInput.text;
    }
}
