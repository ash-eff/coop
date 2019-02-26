using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProtonPack : MonoBehaviour
{

    // TODO use a raycast to kill enemies, just use the particle system for an indicator
    // proton pack will slow enemies and do damage over time

    public ParticleSystem pSystem;
    public GameObject sparks;
    public Transform weapon;

    public float nextFireTime = 3;
    private bool recharge;

    void Update()
    {
        Vector2 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - weapon.position;
        difference.Normalize();
        float zRot = (Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg);
        weapon.rotation = Quaternion.Euler(0f, 0f, zRot);
        //pSystem.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, zRot);

        if (Input.GetButton("Fire1") && nextFireTime > 0)
        {
            recharge = false;
            nextFireTime -= Time.deltaTime;
            pSystem.Play();
            if(nextFireTime < 0)
            {
                pSystem.Stop();
            }
        }
        else if(Input.GetButtonUp("Fire1"))
        {
            recharge = true;
            pSystem.Stop();
        }

        if (recharge)
        {
            if(nextFireTime < 3)
            {
                nextFireTime += (Time.deltaTime * 2);
            }
            else
            {
                nextFireTime = 3;
            }
        }
    }
}
