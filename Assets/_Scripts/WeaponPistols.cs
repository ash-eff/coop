using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeaponPistols : MonoBehaviourPunCallbacks
{
    //public ParticleSystem pSystem1;
    //public ParticleSystem pSystem2;
    public Transform weapon;
    public GameObject projectile;
    public Vector3 offset;
    public float projectileForce = 500f;

    public float fireRate;
    private float nextFireTime;
    public bool fireFirst;

    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }


        if (Input.GetButton("Fire1"))
        {
            Shoot();
            //photonView.RPC("Shoot", RpcTarget.All, null);
        }
    }

    //[PunRPC]
    void Shoot()
    {
        Vector2 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - weapon.position;
        difference.Normalize();
        float zRot = (Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg);
        weapon.rotation = Quaternion.Euler(0f, 0f, zRot);

        if (Time.time > nextFireTime)
        {
            fireFirst = !fireFirst;
            if (fireFirst)
            {
                //pSystem1.Emit(1);
                GameObject cloneGO = PhotonNetwork.Instantiate(projectile.name, weapon.position + offset, Quaternion.identity);
                cloneGO.GetComponent<Rigidbody2D>().AddForce(weapon.transform.right * projectileForce);
            }
            else
            {
                //pSystem2.Emit(1);
                GameObject cloneGO = PhotonNetwork.Instantiate(projectile.name, weapon.position + -offset, Quaternion.identity);
                cloneGO.GetComponent<Rigidbody2D>().AddForce(weapon.transform.right * projectileForce);
            }

            nextFireTime = Time.time + fireRate;
        }
    }
}
