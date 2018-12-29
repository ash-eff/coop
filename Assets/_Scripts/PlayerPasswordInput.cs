using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerPasswordInput : MonoBehaviour {


    void Start()
    {
        InputField inputField = GetComponent<InputField>();

        PhotonNetwork.NickName = inputField.text;
    }


    public void SetPlayerPassword(string value)
    {
        PhotonNetwork.NickName = value;
    }
}
