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
    public GameObject youDied;
    private Vector3 offset = new Vector3(0f, .65f, 0f);

    CameraControl cc;

    public bool dead;

    private void Awake()
    {
        cc = GetComponent<CameraControl>();
    }

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
    public void RPCAddHealth(float amount)
    {
        health += amount;
        if (health > 100)
        {
            health = 100;
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
        cc.targetDead = true;
        dead = true;
        StartCoroutine(DeadPhase());
        // change sprite to dead
    }

    IEnumerator DeadPhase()
    {
        if (photonView.IsMine)
        {
            youDied.SetActive(true);
        }

        float timer = 4f;
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);

        if (photonView.IsMine)
        {
            youDied.SetActive(false);
        }
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
