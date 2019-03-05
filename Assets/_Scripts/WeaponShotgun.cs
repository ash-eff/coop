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
    public float grenadeFireRate;
    public float rays;
    public float angle;
    public int ammo = 6;
    public int grenade = 2;
    private float damage = 4f;
    public GameObject[] shells;
    public GameObject[] grenades;
    public ParticleSystem blast;
    public ParticleSystem smoke;
    public ParticleSystem flare;
    public ParticleSystem grenadeSmoke;
    public ParticleSystem grenadeFlare;
    public GameObject grenadeExplosion;
    float zRot;
    public bool reloading;

    private float nextFireTime;
    private float nextGrenadeTime;

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
                    //photonView.RPC("ShotGunCount", RpcTarget.All, null);
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

            if (Input.GetButton("Fire2") && grenade > 0)
            {
                if (Time.time > nextGrenadeTime)
                {
                    Vector2 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    photonView.RPC("SpendGrenade", RpcTarget.All, null);
                    zRot = (Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg);
                    weapon.rotation = Quaternion.Euler(0f, 0f, zRot);
                    RaycastHit2D hit = Physics2D.Raycast(weapon.transform.position, weapon.transform.right, 6f, enemyLayer);
                    Debug.DrawRay(weapon.transform.position, weapon.transform.right * 6, Color.red, .1f);



                    nextGrenadeTime = Time.time + grenadeFireRate;
                    photonView.RPC("FireGrenade", RpcTarget.Others, null);
                    grenadeFlare.Play();
                    grenadeSmoke.Play();

                    GameObject go = PhotonNetwork.Instantiate(grenadeExplosion.name, clickPos, Quaternion.identity);
                    go.SendMessage("Activate");
                }
            }

            if (ammo == 0)
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
    public void RPCAddGrenade()
    {
        grenade = 2;
        foreach(GameObject nade in grenades)
        {
            nade.SetActive(true);
        }
    }

    [PunRPC]
    public void SpendGrenade()
    {
        grenade--;
        grenades[grenade].SetActive(false);
    }

    [PunRPC]
    void FireGrenade()
    {
        grenadeFlare.Play();
        grenadeSmoke.Play();
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
