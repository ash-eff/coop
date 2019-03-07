using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class PCShotgunner : MonoBehaviourPunCallbacks
{
    public Image abilityBar;
    public TextMeshProUGUI abilityText;
    bool canDamage = true;
    float cooldown;
    float abilityCountdown = 5f;
    bool countingDown;
    bool coolingDown;
    WeaponShotgun shotgun;
    CharacterInfo ci;

    private void Start()
    {
        abilityText.text = "Ready.";
        shotgun = GetComponent<WeaponShotgun>();
        ci = GetComponent<CharacterInfo>();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.E) && canDamage)
            {
                abilityText.text = "";
                canDamage = false;
                cooldown = 60f;
                photonView.RPC("IncreaseDamage", RpcTarget.All, null);
                abilityBar.fillAmount = 1;
                countingDown = true;
            }

        }

        if (countingDown)
        {
            abilityText.text = "";
            abilityCountdown -= Time.deltaTime;
            abilityBar.fillAmount -= 1f / 5f * Time.deltaTime;
            if (abilityCountdown < 0)
            {

                coolingDown = true;
                countingDown = false;
                abilityBar.fillAmount = 0;
                photonView.RPC("ResetDamage", RpcTarget.All, null);
            }
        }

        if (coolingDown)
        {
            cooldown -= Time.deltaTime;
            abilityBar.fillAmount += 1f / 60f * Time.deltaTime;
            if (cooldown < 0)
            {
                abilityText.text = "Ready.";
                coolingDown = false;
                canDamage = true;
                abilityBar.fillAmount = 1;
            }
        }
    }

    [PunRPC]
    void IncreaseDamage()
    {
        shotgun.ReloadSpeed = .1f;
        shotgun.Damage = ci.damage * ci.bonusDamage;
    }


    [PunRPC]
    void ResetDamage()
    {
        shotgun.ReloadSpeed = ci.reloadSpeed;
        shotgun.Damage = ci.damage;
    }
}
