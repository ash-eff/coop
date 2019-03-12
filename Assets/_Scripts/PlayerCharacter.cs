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
    float lifeTimer;

    PlayerInput input;
    WeaponShotgun shotgun;
    CameraControl cc;

    bool dead;

    // game stats
    public string playerName;
    public float deathTime;
    public int numOfZombiesKilled;
    public float accuracy;
    public int numOfReloads;
    public int shotsFired;
    public float dmgTaken;
    public float friendlyDmgTaken;
    public float friendlyDmgGiven;
    public float dmgHealed;

    private void Start()
    {
        shotgun = GetComponent<WeaponShotgun>();
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

        lifeTimer += Time.deltaTime;

        if (!photonView.IsMine)
        {
            return;
        }
      
        if (health <= 0f && !dead)
        {
            dead = true;
            DeathTime = lifeTimer % 60;
            NumOfZombiesKilled = EnemySpawner.instance.deadCount;
            photonView.RPC("SendStats", RpcTarget.Others, DeathTime, NumOfZombiesKilled, Accuracy, NumOfReloads, ShotsFired, DmgTaken, FriendlyDmgTaken, FriendlyDmgGiven, DmgHealed);
            photonView.RPC("RPCDead", RpcTarget.All, null);
            //GameManager.Instance.UpdateStats(playerName, DeathTime, NumOfZombiesKilled, Accuracy, NumOfReloads, ShotsFired, DmgTaken, DmgHealed);
            GameManager.Instance.gameObject.GetPhotonView().RPC("UpdateStats", RpcTarget.All, playerName, DeathTime, NumOfZombiesKilled, Accuracy, NumOfReloads, ShotsFired, DmgTaken, FriendlyDmgTaken, FriendlyDmgGiven, DmgHealed);
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

    public float FriendlyDmgTaken
    {
        get { return friendlyDmgTaken; }
        set { friendlyDmgTaken = value; }
    }

    public float FriendlyDmgGiven
    {
        get { return friendlyDmgGiven; }
        set { friendlyDmgGiven = value; }
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
    public void TakeFriendlyDamage(float dmg)
    {
        if (health > 0)
        {
            health -= dmg;
        }
        if (health - dmg <= 0)
        {
            health = 0;
        }

        redTimer = .2f;
        if (photonView.IsMine)
        {
            DmgTaken += dmg;
            FriendlyDmgTaken += dmg;
        }
    }

    [PunRPC]
    void RPCDead()
    {
        GameManager.playersDead++;
        cc.targetDead = true;
        StartCoroutine(DeadPhase());
        // change sprite to dead
    }

    [PunRPC]
    void SendStats(float _deathTime, int _numOfZombiesKilled, float _accuracy, int _numOfReloads, int _shotsFired, float _dmgTaken, float _friendlyTaken, float _friendlyGiven, float _dmgHealed)
    {
        DeathTime = _deathTime;
        NumOfZombiesKilled = _numOfZombiesKilled;
        Accuracy = _accuracy;
        NumOfReloads = _numOfReloads;
        ShotsFired = _shotsFired;
        DmgTaken = _dmgTaken;
        FriendlyDmgTaken = _friendlyTaken;
        FriendlyDmgGiven = _friendlyGiven;
        DmgHealed = _dmgHealed;
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
            //stream.SendNext(this.NumOfReloads);
            //stream.SendNext(this.ShotsFired);
            //stream.SendNext(this.DmgTaken);
            //stream.SendNext(this.DmgHealed);
        } 
        else
        {
            // Network player, receive data
            this.health = (float)stream.ReceiveNext();
            //this.NumOfReloads = (int)stream.ReceiveNext();
            //this.ShotsFired = (int)stream.ReceiveNext();
            //this.DmgTaken = (float)stream.ReceiveNext();
            //this.DmgHealed = (float)stream.ReceiveNext();
        }
    }
    
    #endregion
}
