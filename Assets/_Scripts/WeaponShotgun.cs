using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WeaponShotgun : MonoBehaviourPunCallbacks
{
    public Transform weapon;
    public LayerMask enemyLayer;
    public float fireRate;
    public float rays;
    public float angle;
    public float ammo = 6f;
    public float damage = 2f;
   
    private float nextFireTime;

    // Update is called once per frame
    void Update()
    {
        Vector2 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - weapon.position;
        difference.Normalize();

        if (Input.GetButton("Fire1"))
        {
            if(Time.time > nextFireTime)
            {
                //playerHits = new List<Transform>();
                //ammo--;
                for (int i = 0; i < rays; i++)
                {
                    float dividedAngle = angle / 2;
                    float offset = angle / rays * i;
                    float zRot = (Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg) - dividedAngle;
                    weapon.rotation = Quaternion.Euler(0f, 0f, zRot + offset);
                    RaycastHit2D hit = Physics2D.Raycast(weapon.transform.position, weapon.transform.right * 8, enemyLayer);
                    Debug.DrawRay(weapon.transform.position, weapon.transform.right * 8, Color.red, .1f);

                    if (hit)
                    {
                        hit.transform.SendMessage("TakeDamage", damage);
                    }
                }

                nextFireTime = Time.time + fireRate;
            }
        }
    }
}
