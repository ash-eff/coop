using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerCharacter : MonoBehaviourPunCallbacks, IPunObservable {

    [Tooltip("The current Health of our player")]
    [Range(0,1)]
    public float health = 100f;

    [Tooltip("The UI of our character")]
    public GameObject playerUI;

    public GameObject skullPrefab;
    private float skullTimer;

    public GameObject whoShotMe;

    private Vector3 offset = new Vector3(0f, .65f, 0f);

    private void Update()
    {
        if (health <= 0f)
        {
            photonView.RPC("Dead", RpcTarget.All, null);
        }

        if(skullTimer > 0)
        {
            skullTimer -= Time.deltaTime;
        }

        if(skullTimer < 0)
        {
            skullTimer = 0;
            photonView.RPC("SetSkullDeActive", RpcTarget.All, null);
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

    // TODO this needs to translate over the network better. Maybe dont knockback, but make the player blink
    public void TakeFriendlyDamage()
    {
        health -= 1;
        skullTimer = .5f;
        photonView.RPC("SetSkullActive", RpcTarget.All, null);
        //GameObject skull = PhotonNetwork.Instantiate(skullPrefab.name, transform.position + offset, Quaternion.identity);
        //skull.SendMessage("StartMoveSkull");
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;
    }

    [PunRPC]
    void Dead()
    {
        gameObject.SetActive(false);
    }

    [PunRPC]
    void SetSkullActive()
    {
        skullPrefab.SetActive(true);
    }

    [PunRPC]
    void SetSkullDeActive()
    {
        skullPrefab.SetActive(false);
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
