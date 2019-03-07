using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Explosion : MonoBehaviourPunCallbacks
{
    float timer = 1f;
    float deathTimer = 2f;
    public ParticleSystem ps;

    private void Update()
    {
        deathTimer -= Time.deltaTime;

        if(deathTimer < 0)
        {
            PhotonNetwork.Destroy(gameObject);
        }

    }

    void Activate()
    {
        StartCoroutine(CountDown());
    }

    IEnumerator CountDown()
    {
        Debug.Log("Count");
        while(timer > 0)
        {
            timer -= Time.deltaTime;
        }

        ps.Play();
        yield return null;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Enemy")
        {
            collision.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, 25f);           
        }
    }
}
