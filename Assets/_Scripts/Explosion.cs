using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Explosion : MonoBehaviourPunCallbacks
{
    float timer = 1f;
    float deathTimer = 2f;
    public ParticleSystem ps;
    public PlayerCharacter pc;
    bool zombieHit;

    private void Update()
    {
        deathTimer -= Time.deltaTime;

        if(deathTimer < 0)
        {
            Destroy(gameObject);
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
            collision.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, 50f);
            if (!zombieHit)
            {
                zombieHit = true;
                pc.GetComponent<WeaponShotgun>().hit++;
            }                
        }
        if (collision.tag == "Player")
        {
            pc.FriendlyDmgGiven += 25f * Time.deltaTime;
            collision.gameObject.GetPhotonView().RPC("TakeFriendlyDamage", RpcTarget.All, 25f * Time.deltaTime);
        }
    }
}
