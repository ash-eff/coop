using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Enemy : MonoBehaviourPunCallbacks
{
    int playerIndex;
    public GameObject target;

    float speed;
    int targetIndex;
    Rigidbody2D rb2d;
    public float health = 25f;
    public float baseDamage = 2f;
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

    public enum State { CHASING, STUNNED, SPAWNING};
    public State state;

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
        speed = Random.Range(4f, 5f);
        state = State.SPAWNING;
        rb2d = GetComponent<Rigidbody2D>();
        StartCoroutine(Spawning());
        StartCoroutine(ColorLerp());
    }

    public void SetTheTarget(GameObject _target)
    {
        target = _target;
    }

    private void Update()
    {
        deathTimer -= Time.deltaTime;
        if(deathTimer <= 0)
        {
            PhotonNetwork.Destroy(gameObject);
        }

        if (state != State.SPAWNING)
        {
            if (health <= 0)
            {
                PhotonNetwork.Destroy(gameObject);
            }

            if(state == State.STUNNED)
            {
                isStunned = true;
            }
            else
            {
                isStunned = false;
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

        state = State.CHASING;
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        // play attack animation
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.SendMessage("TakeZombieDamage", baseDamage * Time.deltaTime);
        }
    }

    #region IPunObservable implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(this.health);
            stream.SendNext(this.target);
        }
        else
        {
            // Network player, receive data
            this.health = (float)stream.ReceiveNext();
            this.target = (GameObject)stream.ReceiveNext();
        }
    }

    #endregion
}
