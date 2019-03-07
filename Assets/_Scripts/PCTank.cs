using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class PCTank : MonoBehaviourPunCallbacks
{
    public Image abilityBar;
    public TextMeshProUGUI abilityText;
    bool canTaunt = true;
    float cooldown;
    float abilityCountdown = 5f;
    bool countingDown;
    bool coolingDown;

    private void Start()
    {
        abilityText.text = "Ready.";
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.E) && canTaunt)
            {
                abilityText.text = "";
                canTaunt = false;
                cooldown = 60f;
                photonView.RPC("Taunt", RpcTarget.All, null);
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
                canTaunt = true;
                abilityBar.fillAmount = 1;
            }
        }
    }

    [PunRPC]
    void Taunt()
    {
        Enemy[] enemyArray = FindObjectsOfType<Enemy>();
        foreach(Enemy enemy in enemyArray)
        {
            enemy.target = null;
            enemy.target = transform.gameObject;
            enemy.state = Enemy.State.TAUNTED;
        }
    }
}
