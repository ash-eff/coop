using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class PlayerCharacter : MonoBehaviourPunCallbacks, IPunObservable {

    [Tooltip("The current Health of our player")]
    [Range(0,1)]
    public float health = 100f;

    [Tooltip("The UI of our character")]
    public GameObject playerUI;

    public Image healthBar;

    private Vector3 offset = new Vector3(0f, .65f, 0f);

    private void Update()
    {
        healthBar.fillAmount = health / 100;

        if (health <= 0f)
        {
            photonView.RPC("Dead", RpcTarget.All, null);
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

    [PunRPC]
    public void TakeZombieDamage(float dmg)
    {
        health -= dmg;
    }

    [PunRPC]
    void Dead()
    {
        gameObject.SetActive(false);
    }

    #region IPunObservable implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(this.health);
        }
        else
        {
            // Network player, receive data
            this.health = (float)stream.ReceiveNext();
        }
    }
    
    #endregion
}
