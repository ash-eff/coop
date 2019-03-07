using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class Enemy : MonoBehaviourPunCallbacks, IPunObservable
{
    int playerIndex;
    public GameObject target;
    public Image healthBar;
    public GameObject healthBarHolder;
    float speed;
    int targetIndex;
    Rigidbody2D rb2d;
    public float health = 25f;
    public float damage;
    private float deathTimer = 60f;
    private SpriteRenderer spr;
    private Color currentColor = Color.white;
    public Color startColor;
    public Color endColor;
    private float smoothness = .2f;
    private float duration = 2f;
    private Vector3 targetDestination;
    private float stunTimer;
    private bool isStunned;
    public PlayerCharacter[] players;
    private EnemySpawner es;
    public Collider2D circleCol;

    public enum State { CHASING, STUNNED, SPAWNING, TAUNTED };
    public State state;

    private void Awake()
    {
        es = FindObjectOfType<EnemySpawner>();
        spr = GetComponent<SpriteRenderer>();
        speed = Random.Range(1f, 7f);
        state = State.SPAWNING;
        rb2d = GetComponent<Rigidbody2D>();
        StartCoroutine(Spawning());
        StartCoroutine(ColorLerp());
    }

    private void Update()
    {
        healthBar.fillAmount = health / 25;
        //deathTimer -= Time.deltaTime;
        //if(deathTimer <= 0)
        //{
        //    EnemySpawner.deathCount += 1;
        //    photonView.RPC("Die", RpcTarget.All, null);
        //}
        if (!photonView.IsMine)
        {
            return;
        }

        if (state != State.SPAWNING)
        {
            
            if (health <= 0)
            {
                EnemySpawner.deathCount += 1;
                photonView.RPC("Die", RpcTarget.All, null);              
            }

            if (state == State.STUNNED)
            {
                isStunned = true;
            }
            else
            {
                isStunned = false;
            }

            if (state == State.TAUNTED)
            {
                StopAllCoroutines();
                StartCoroutine(Taunted());
            }

            //if (knockBackTimer > 0)
            //{
            //    knockBackTimer -= Time.deltaTime;
            //}
            //
            //if (knockBackTimer <= 0 && state == State.STUNNED)
            //{
            //    StartCoroutine(UpdatePath());
            //}
        }
    }

    public float Speed
    {
        set { speed = value; }
    }

    IEnumerator Taunted()
    {
        state = State.CHASING;
        speed = 9f;
        StartCoroutine(UpdatePath());
        float frenzyTimer = 4f;
        while (frenzyTimer > 0)
        {
            frenzyTimer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        StopAllCoroutines();
        target = null;
        speed = 9f;
        photonView.RPC("RPCSelectTarget", RpcTarget.All, null);
        StartCoroutine(UpdatePath());
    }

    IEnumerator ColorLerp()
    {
        float progress = 0; //This float will serve as the 3rd parameter of the lerp function.
        float increment = smoothness / duration; //The amount of change to apply.
        while (progress < 1)
        {
            currentColor = Color.Lerp(startColor, endColor, progress);
            spr.color = currentColor;
            progress += increment;
            yield return new WaitForSeconds(smoothness);
        }
    }

    IEnumerator Spawning()
    {
        float spawnTimer = 2f;
        while(true)
        {
            spawnTimer -= Time.deltaTime;
            
            // play spawning animation
            yield return new WaitForEndOfFrame();
            if(spawnTimer <= 0)
            {
                break;
            }
        }

        healthBarHolder.SetActive(true);
        photonView.RPC("RPCSelectTarget", RpcTarget.All, null);
        state = State.CHASING;
        circleCol.enabled = true;
        StartCoroutine(UpdatePath());
    }

    IEnumerator UpdatePath()
    {
        while (state == State.CHASING)
        {
            yield return new WaitForSeconds(.2f);

            targetDestination = target.transform.position;
            StopCoroutine(FollowPath());
            StartCoroutine(FollowPath());

            if (!target.activeInHierarchy)
            {
                photonView.RPC("RPCSelectTarget", RpcTarget.All, null);
            }
        }
    }

    IEnumerator FollowPath()
    {
        while (state == State.CHASING)
        { 
            if (transform.position != targetDestination)
            { 
                Vector2 newPosition = Vector2.MoveTowards(transform.position, targetDestination, speed * Time.deltaTime);
                rb2d.MovePosition(newPosition);
            }
            yield return null;
        }      
    }

    IEnumerator Stunned()
    {
        while (state == State.STUNNED)
        {
            stunTimer -= Time.deltaTime;
            if(stunTimer <= 0)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        state = State.CHASING;
        StartCoroutine(UpdatePath());
    }

    [PunRPC]
    void TakeDamage(float dmg)
    {
        if(state != State.SPAWNING)
        {
            health -= dmg;
            if (!isStunned)
            {
                StopAllCoroutines();
                StartCoroutine(Stunned());
            }
        }
    }

    [PunRPC]
    void RPCSelectTarget()
    {
        players = FindObjectsOfType<PlayerCharacter>();
        int randomIndex = Random.Range(0, players.Length);
        target = players[randomIndex].transform.gameObject;
    }

    [PunRPC]
    void Die()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetPhotonView().RPC("TakeZombieDamage", RpcTarget.All, damage * Time.deltaTime);
        }
    }

    //private void OnCollisionStay2D(Collision2D collision)
    //{
    //    // play attack animation
    //    if (collision.gameObject.tag == "Player")
    //    {
    //        collision.gameObject.GetPhotonView().RPC("TakeZombieDamage", RpcTarget.All, damage * Time.deltaTime);
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
