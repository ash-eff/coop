using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerCharacter : MonoBehaviourPunCallbacks {

    [Tooltip("The current Health of our player")]
    [Range(0,1)]
    public float Health = 1f;

    [Tooltip("The UI of our character")]
    public GameObject playerUI;

    private void Update()
    {
        if (Health <= 0f)
        {
            GameConnectionManager.Instance.LeaveRoom();
        }
    }

    public void InstantiateUI()
    {
        if (photonView.IsMine)
        {
            GameObject ui = Instantiate(playerUI, Vector2.zero, Quaternion.identity);
            Debug.Log("INSTANTIATE " + ui + " FOR " + photonView.Owner.NickName);
            ui.SendMessage("SetTarget", gameObject);
        }
    }

    //#region IPunObservable implementation
    //
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
    //
    //#endregion
}
