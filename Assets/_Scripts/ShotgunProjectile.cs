using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShotgunProjectile : MonoBehaviourPunCallbacks
{

    // TODO add particle system to projectile. Shot gun will knock enemies back if it doesn't kill them
    private float baseDamage = 2f;
    private float projectileLifeTime = .25f;
    public Vector3 startPos;
    public Transform firedBy;

    private void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        projectileLifeTime -= Time.deltaTime;

        if (projectileLifeTime < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, baseDamage);

            Destroy(gameObject);
        }

        if(collision.gameObject.tag == "Obstacle")
        {
            Destroy(gameObject);
        }
    }
}
