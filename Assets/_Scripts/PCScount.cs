using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class PCScount : MonoBehaviourPunCallbacks
{
    public Image abilityBar;
    public TextMeshProUGUI abilityText;
    bool canSprint = true;
    float cooldown;
    float abilityCountdown = 5f;
    bool countingDown;
    bool coolingDown;
    PlayerInput input;
    CharacterInfo ci;

    private void Start()
    {
        abilityText.text = "Ready.";
        input = GetComponent<PlayerInput>();
        ci = GetComponent<CharacterInfo>();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.E) && canSprint)
            {
                abilityText.text = "";
                canSprint = false;
                cooldown = 60;
                photonView.RPC("ExceleratedSpeed", RpcTarget.All, null);
                abilityBar.fillAmount = 1;
                countingDown = true;
            }
        }

        if (countingDown)
        {
            abilityText.text = "";
            abilityCountdown -= Time.deltaTime;
            abilityBar.fillAmount -= 1f / 5f * Time.deltaTime;
            if (abilityCountdown <= 0)
            {
                coolingDown = true;
                countingDown = false;
                abilityBar.fillAmount = 0;
                photonView.RPC("ResetSpeed", RpcTarget.All, null);
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
                canSprint = true;
                abilityBar.fillAmount = 1;
            }
        }
    }

    [PunRPC]
    void ExceleratedSpeed()
    {
        input.Speed = ci.speed * ci.bonusSpeed;
    }


    [PunRPC]
    void ResetSpeed()
    {
        input.Speed = ci.speed;
    }
}
