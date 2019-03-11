using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CharacterInfo : MonoBehaviourPunCallbacks
{
    public string characterName;
    public string characterInfo;

    public float health;
    public float speed;
    public float bonusSpeed;
    public float speedDebuff;
    public float damage;
    public float bonusDamage;
    public float shotgunFireRate;
    public float grenadeFireRate;
    public float reloadSpeed;
    public float pellets;
    public float angle;
    public float range;
    public int ammo;
    public int grenade;
    public int maxGrenades;

    PlayerCharacter pc;
    PlayerInput input;
    WeaponShotgun shotgun;

    private bool playable;
    
    public bool isSelectable;

    private void Awake()
    {
        CameraControl cameraControl = gameObject.GetComponent<CameraControl>();
        pc = GetComponent<PlayerCharacter>();
        input = GetComponent<PlayerInput>();
        shotgun = GetComponent<WeaponShotgun>();

        pc.MaxHealth = health;
        input.Speed = speed;
        input.SpeedDebuff = speedDebuff;
        shotgun.Damage = damage;
        shotgun.ShotgunFireRate = shotgunFireRate;
        shotgun.GrenadeFireRate = grenadeFireRate;
        shotgun.ReloadSpeed = reloadSpeed;
        shotgun.Pellets = pellets;
        shotgun.Angle = angle;
        shotgun.Range = range;
        shotgun.Ammo = ammo;
        shotgun.Grenade = grenade;
        shotgun.MaxGrenades = maxGrenades;
        
        if (cameraControl != null)
        {
            if (photonView.IsMine)
            {
                cameraControl.OnStartFollowing();
            }
        }
        else
        {
            Debug.LogError("<Color=Red><b>Missing</b></Color> CameraControl Component on character Prefab.", this);
        }
    }

    public void WakeUp()
    {
        isSelectable = true;
    }

    public void SelectChamp()
    {
        if (isSelectable)
        {
            Debug.Log(name + " has been selected.");
            isSelectable = false;
            Debug.Log("Selectable? " + isSelectable);
        }
        else if (!isSelectable)
        {
            Debug.Log(name + " has been de-selected.");
            isSelectable = true;
            Debug.Log("Selectable? " + isSelectable);
        }
    }
}