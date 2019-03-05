using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HealthPickup : MonoBehaviourPunCallbacks
{

    float deathTimer = 12f;

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
            collision.gameObject.GetPhotonView().RPC("RPCAddHealth", RpcTarget.All, 10f);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
