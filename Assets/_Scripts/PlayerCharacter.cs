using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class PlayerCharacter : MonoBehaviourPunCallbacks, IPunObservable {

    private float maxHealth;
    private float health;

    //[Tooltip("The UI of our character")]
    //public GameObject playerUI;

    public TextMeshProUGUI healthText;
    public GameObject redScreen;
    public Image healthBar;
    public GameObject youDiedScreen;
    float redTimer;

    PlayerInput input;
    WeaponShotgun shotgun;
    CameraControl cc;

    bool dead;

    private void Start()
    {
        health = maxHealth;
        cc = GetComponent<CameraControl>();
    }

    private void Update()
    {
        healthBar.fillAmount = health / maxHealth;
        healthText.text = Mathf.RoundToInt(health).ToString() + "/" + maxHealth.ToString();

        if (photonView.IsMine)
        {
            redTimer -= Time.deltaTime;

            if (redTimer > 0)
            {
                redScreen.SetActive(true);
            }
            else
            {
                redScreen.SetActive(false);
            }
        }

        if (health <= 0f)
        {
            dead = true;
            photonView.RPC("RPCDead", RpcTarget.All, null);
        }
    }

    public float MaxHealth
    {
        set { maxHealth = value; }
    }

    public bool Dead
    {
        get { return dead; }
        set { dead = value; }
    }

    //public void InstantiateUI()
    //{
    //    if (photonView.IsMine)
    //    {
    //        GameObject ui = Instantiate(playerUI, Vector2.zero, Quaternion.identity);
    //        Debug.Log("INSTANTIATE " + ui + " FOR " + photonView.Owner.NickName);
    //        ui.SendMessage("SetTarget", gameObject);
    //    }
    //}

    [PunRPC]
    public void RPCAddHealth(int amount)
    {
        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    [PunRPC]
    public void TakeZombieDamage(float dmg)
    {
        health -= dmg;
        redTimer = .2f;
    }

    [PunRPC]
    void RPCDead()
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
            youDiedScreen.SetActive(true);
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
            youDiedScreen.SetActive(false);
        }
    }

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if(collision.tag == "Enemy" && photonView.IsMine)
    //    {
    //        red.SetActive(true);
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.tag == "Enemy" && photonView.IsMine)
    //    {
    //        red.SetActive(false);
    //    }
    //}

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
