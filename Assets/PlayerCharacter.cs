using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerCharacter : MonoBehaviourPunCallbacks, IPunObservable {

    [Tooltip("The current Health of our player")]
    public float Health = 1f;

    public GameObject playerUI;

    private void Awake()
    {
        GameObject ui = Instantiate(playerUI, Vector2.zero, Quaternion.identity);
        ui.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
    }

    private void Update()
    {
        if (Health <= 0f)
        {
            GameConnectionManager.Instance.LeaveRoom();
        }
    }

    #region IPunObservable implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(this.Health);
        }
        else
        {
            // Network player, receive data
            this.Health = (float)stream.ReceiveNext();
        }
    }

    #endregion
}
