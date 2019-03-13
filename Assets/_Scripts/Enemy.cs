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
    public GameObject targetHolder;
    float speed;
    int targetIndex;
    Rigidbody2D rb2d;
    public float health = 25f;
    private float damage;
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
    public Collider2D bodyCol;
    public bool targeted;
    private bool active;
    private bool gameOver;
    private bool wandering;
    public float targetTimer;
    private GameManager gm;

    public enum State { CHASING, STUNNED, SPAWNING, TAUNTED };
    public State state;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        es = FindObjectOfType<EnemySpawner>();
        spr = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
        state = State.SPAWNING;
        speed = Random.Range(1f, 7f);
        StartCoroutine(Spawning());
        StartCoroutine(ColorLerp());
    }

    private void Update()
    {
        healthBar.fillAmount = health / 25;

        if (!photonView.IsMine)
        {
            return;
        }

        if (gm.gameOver)
        {
            gameOver = true;
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

            if (gameOver && !wandering)
            {
                wandering = true;
                StopAllCoroutines();
                speed = 1f;
                state = State.CHASING;
                StartCoroutine(Wandering());
            }
        }
    }

    public float Speed
    {
        set { speed = value; }
    }

    public float Damage
    {
        set { damage = value; }
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

    IEnumerator Wandering()
    {
        while (state == State.CHASING)
        {
            yield return new WaitForSeconds(5f);

            float randX = Random.Range(-18, 18);
            float randY = Random.Range(-9, 9);
            targetDestination = new Vector2(randX, randY);
            StopCoroutine(FollowPath());
            StartCoroutine(FollowPath());
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
        if(!gm.gameOver)
        {
            int randomIndex = Random.Range(0, players.Length);
            target = players[randomIndex].transform.gameObject;
        }
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

        if (collision.gameObject.tag == "Laser" && !targeted)
        {
            targeted = true;
            photonView.RPC("Activate", RpcTarget.All, null);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Laser" && targeted)
        {
            targeted = false;
            photonView.RPC("Deactivate", RpcTarget.All, null);
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
