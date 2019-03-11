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
    public bool startWithGrenade;
    private float shotgunFireRate;
    private float grenadeFireRate;
    private float reloadSpeed;
    private float pellets;
    private float angle;
    private float range;
    private int ammo;
    private int maxAmmo;
    private int grenade;
    private int maxGrenades;
    private float damage;
    private float bonusDamage;
    public GameObject[] shells;
    public GameObject[] grenades;
    public ParticleSystem blast;
    public ParticleSystem smoke;
    public ParticleSystem flare;
    public ParticleSystem grenadeSmoke;
    public ParticleSystem grenadeFlare;
    public GameObject grenadeExplosion;
    float zRot;
    private bool reloading;

    private float nextFireTime;
    private float nextGrenadeTime;

    PlayerCharacter pc;
    PlayerInput input;

    private void Start()
    {
        maxAmmo = ammo;
        input = GetComponent<PlayerInput>();
        pc = GetComponent<PlayerCharacter>();
        if (startWithGrenade)
        {
            photonView.RPC("RPCAddGrenade", RpcTarget.All, null);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine && !pc.Dead)
        {
            Vector2 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - weapon.position;
            difference.Normalize();

            if (Input.GetButton("Fire1") && ammo > 0 && !reloading)
            {
                if (Time.time > nextFireTime)
                {
                    photonView.RPC("SpendShell", RpcTarget.All, null);
                    pc.ShotsFired += 1;
                    //photonView.RPC("ShotGunCount", RpcTarget.All, null);
                    for (int i = 0; i < pellets; i++)
                    {
                        float dividedAngle = angle / 2;
                        float offset = angle / pellets * i;
                        zRot = (Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg) - dividedAngle;
                        weapon.rotation = Quaternion.Euler(0f, 0f, zRot + offset);
                        RaycastHit2D hit = Physics2D.Raycast(weapon.transform.position, weapon.transform.right, range, enemyLayer);
                        Debug.DrawRay(weapon.transform.position, weapon.transform.right * range, Color.red, .1f);

                        if (hit)
                        {
                            hit.transform.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, (damage + bonusDamage));
                        }
                    }

                    nextFireTime = Time.time + shotgunFireRate;
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
                pc.NumOfReloads += 1;
                photonView.RPC("RPCReload", RpcTarget.All, null);        
            }
        }     
    }

    public float ShotgunFireRate
    {
        set { shotgunFireRate = value; }
    }

    public float GrenadeFireRate
    {
        set { grenadeFireRate = value; }
    }

    public float ReloadSpeed
    {
        set { reloadSpeed = value; }
    }

    public float Pellets
    {
        set { pellets = value; }
    }

    public float Angle
    {
        set { angle = value; }
    }

    public float Range
    {
        set { range = value; }
    }

    public int Ammo
    {
        set { ammo = value; }
    }

    public int Grenade
    {
        set { grenade = value; }
    }

    public int MaxGrenades
    {
        set { maxGrenades = value; }
    }

    public float Damage
    {
        set { damage = value; }
    }

    public float BonusDamage
    {
        set { bonusDamage = value; }
    }

    IEnumerator Reload()
    {
        input.Speed = input.BaseSpeed / input.SpeedDebuff;
        while (ammo != maxAmmo)
        {
            foreach(GameObject shell in shells)
            {
                if (!shell.activeInHierarchy)
                {
                    shell.SetActive(true);
                    yield return new WaitForSeconds(reloadSpeed);
                    ammo++;
                }
            }
        }

        reloading = false;
        input.Speed = input.BaseSpeed;
    }

    [PunRPC]
    public void RPCAddGrenade()
    {
        grenade = maxGrenades;
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
