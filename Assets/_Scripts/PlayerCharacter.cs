using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class PlayerCharacter : MonoBehaviourPunCallbacks, IPunObservable {

    private float maxHealth;
    private float health;

    public TextMeshProUGUI healthText;
    public GameObject redScreen;
    public Image healthBar;
    public GameObject youDiedScreen;
    float redTimer;

    PlayerInput input;
    WeaponShotgun shotgun;
    CameraControl cc;

    bool dead;

    // game stats
    public string playerName;
    public float deathTime;
    public int numOfZombiesKilled = 0;
    public float accuracy = 0;
    public int numOfReloads;
    public int shotsFired;
    public float dmgTaken;
    public float dmgHealed;

    private void Start()
    {
        playerName = photonView.Owner.NickName;
        health = maxHealth;
        cc = GetComponent<CameraControl>();
    }

    private void Update()
    {
        if (health > 0 && !dead)
        {
            healthBar.fillAmount = health / maxHealth;
            healthText.text = Mathf.RoundToInt(health).ToString() + "/" + maxHealth.ToString();
        }

        if (health <= 0f && !dead)
        {
            dead = true;
            deathTime = Time.time;
            GameManager.playersDead++;
            //GameManager.Instance.gameObject.GetPhotonView().RPC("UpdateStats", RpcTarget.All, playerName, DeathTime, NumOfZombiesKilled, Accuracy, NumOfReloads, ShotsFired, DmgTaken, DmgHealed);
            GameManager.Instance.UpdateStats(playerName, DeathTime, NumOfZombiesKilled, Accuracy, NumOfReloads, ShotsFired, DmgTaken, DmgHealed);
            photonView.RPC("RPCDead", RpcTarget.AllViaServer, null);
        }


        if (!photonView.IsMine)
        {
            return;
        }

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

    public float DeathTime
    {
        get { return deathTime; }
        set { deathTime = value; }
    }

    public int NumOfZombiesKilled
    {
        get { return numOfZombiesKilled; }
        set { numOfZombiesKilled = value; }
    }

    public float Accuracy
    {
        get { return accuracy; }
        set { accuracy = value; }
    }

    public int ShotsFired
    {
        get { return shotsFired; }
        set { shotsFired = value; }
    }

    public int NumOfReloads
    {
        get { return numOfReloads; }
        set { numOfReloads = value; }
    }

    public float DmgTaken
    {
        get { return dmgTaken; }
        set { dmgTaken = value; }
    }

    public float DmgHealed
    {
        get { return dmgHealed; }
        set { dmgHealed = value; }
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

    [PunRPC]
    public void RPCAddHealth(int amount)
    {
        float currentHealth = health;
        float actual;
        if (currentHealth + amount > maxHealth)
        {
            float over = currentHealth + amount - maxHealth;
            actual = maxHealth - over;
            health = actual;
        }
        actual = amount;
        health += actual;

        if (photonView.IsMine)
        {
            DmgHealed += actual;
        }
    }

    [PunRPC]
    public void TakeZombieDamage(float dmg)
    {
        if(health > 0)
        {
            health -= dmg;
        }
        if(health - dmg <= 0)
        {
            health = 0;
        }

        redTimer = .2f;
        if (photonView.IsMine)
        {
            DmgTaken += dmg;
        }
    }

    [PunRPC]
    void RPCDead()
    {
        cc.targetDead = true;
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

    #region IPunObservable implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(this.health);
            stream.SendNext(this.NumOfReloads);
            stream.SendNext(this.ShotsFired);
            stream.SendNext(this.DmgTaken);
            stream.SendNext(this.DmgHealed);
        } 
        else
        {
            // Network player, receive data
            this.health = (float)stream.ReceiveNext();
            this.NumOfReloads = (int)stream.ReceiveNext();
            this.ShotsFired = (int)stream.ReceiveNext();
            this.DmgTaken = (float)stream.ReceiveNext();
            this.DmgHealed = (float)stream.ReceiveNext();
        }
    }
    
    #endregion
}
