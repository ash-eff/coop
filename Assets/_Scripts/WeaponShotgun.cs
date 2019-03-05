using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class WeaponShotgun : MonoBehaviourPunCallbacks
{
    public Transform weapon;
    public LayerMask enemyLayer;
    public GameObject reloadIndicator;
    public PlayerInput input;
    public float fireRate;
    public float rays;
    public float angle;
    public int ammo = 6;
    public float damage = 2f;
    public GameObject[] shells;
    public ParticleSystem blast;
    public ParticleSystem smoke;
    public ParticleSystem flare;
    float zRot;
    public bool reloading;

    private float nextFireTime;

    private void Start()
    {
        input = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            Vector2 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - weapon.position;
            difference.Normalize();

            if (Input.GetButton("Fire1") && ammo > 0 && !reloading)
            {
                if (Time.time > nextFireTime)
                {
                    photonView.RPC("SpendShell", RpcTarget.All, null);
                    photonView.RPC("ShotGunCount", RpcTarget.All, null);
                    for (int i = 0; i < rays; i++)
                    {
                        float dividedAngle = angle / 2;
                        float offset = angle / rays * i;
                        zRot = (Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg) - dividedAngle;
                        weapon.rotation = Quaternion.Euler(0f, 0f, zRot + offset);
                        RaycastHit2D hit = Physics2D.Raycast(weapon.transform.position, weapon.transform.right, 6f, enemyLayer);
                        Debug.DrawRay(weapon.transform.position, weapon.transform.right * 6, Color.red, .1f);

                        if (hit)
                        {
                            hit.transform.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, damage);
                        }
                    }

                    nextFireTime = Time.time + fireRate;
                    photonView.RPC("ShotGunBlast", RpcTarget.Others, null);
                    blast.Play();
                    smoke.Play();
                    flare.Play();
                }
            }

            if(ammo == 0)
            {
                reloadIndicator.SetActive(true);
            }
            else
            {
                reloadIndicator.SetActive(false);
            }

            if (Input.GetButtonDown("R") && !reloading && ammo < 6)
            {
                photonView.RPC("RPCReload", RpcTarget.All, null);        
            }
        }     
    }

    IEnumerator Reload()
    {
        input.Speed = 4f;
        while (ammo != 6)
        {
            foreach(GameObject shell in shells)
            {
                if (!shell.activeInHierarchy)
                {
                    shell.SetActive(true);
                    yield return new WaitForSeconds(.3f);
                    ammo++;
                }
            }
        }

        reloading = false;
        input.Speed = 8f;
    }

    [PunRPC]
    void SpendShell()
    {
        ammo--;
        shells[ammo].SetActive(false);
    }

    [PunRPC]
    void RPCReload()
    {
        reloading = true;
        StartCoroutine(Reload());
    }

    [PunRPC]
    void ShotGunBlast()
    {
        blast.Play();
        smoke.Play();
        flare.Play();
    }
}
