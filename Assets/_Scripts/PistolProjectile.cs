using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PistolProjectile : MonoBehaviourPunCallbacks, IPunObservable
{

    // add particle system to projectile. a bullet does double dmg if it ricochets

    public float timer = 3f;
    public float baseDamage = 10f;
    public ParticleSystem spark;
    int bounce = 0;
    Rigidbody2D rb2d;
    Vector3 networkPosition;
    float networkRotation;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0 || bounce >= 3)
        {
            Destroy(gameObject);
        }
    }

    void fixedUpdate()
    {
        if (!photonView.IsMine)
        {
            rb2d.position = Vector3.MoveTowards(rb2d.position, networkPosition, Time.fixedDeltaTime);
            //rb2d.rotation = Quaternion.RotateTowards(rb2d.rotation, networkRotation, Time.fixedDeltaTime * 100.0f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            PhotonNetwork.Instantiate(spark.name, transform.position, Quaternion.identity);
            spark.Emit(3);
            bounce++;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            float totalDmg = baseDamage * (bounce + 1);
            collision.SendMessage("TakeDamage", totalDmg);
            Destroy(gameObject);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(rb2d.position);
            //stream.SendNext(rb2d.rotation);
            stream.SendNext(rb2d.velocity);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            //networkRotation = (float)stream.ReceiveNext();
            rb2d.velocity = (Vector3)stream.ReceiveNext();

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
            rb2d.position += rb2d.velocity * lag;
        }

    }
}
