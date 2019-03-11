using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HealthPickup : MonoBehaviourPunCallbacks
{
    public int healthAmount;
    float deathTimer = 12f;

    private void Start()
    {
        healthAmount = Mathf.RoundToInt(5 + (Time.time / 60));
    }

    private void Update()
    {
        deathTimer -= Time.deltaTime;

        if(deathTimer < 0)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetPhotonView().RPC("RPCAddHealth", RpcTarget.All, healthAmount);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Pickup")
        {
            collision.transform.position = new Vector2(collision.transform.position.x + Random.Range(.1f,.5f), collision.transform.position.y + Random.Range(.1f, .5f));
        }
    }
}
