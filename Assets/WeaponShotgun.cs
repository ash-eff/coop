using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShotgun : MonoBehaviour
{
    public Rigidbody2D projectile;
    public Transform weapon;
    public float projectileForce = 500f;
    public float fireRate;
    public float bullets;
    public float angle;
    
    private Rigidbody2D rb2d;
    private float nextFireTime;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - weapon.position;
        difference.Normalize(); 
       
        if(Input.GetButton("Fire1"))
        {
            if(Time.time > nextFireTime)
            {
                for (int i = 0; i < bullets; i++)
                {

                    float forceOffset = Random.Range(1f, 1.2f);
                    float dividedAngle = angle / 2;
                    float offset = angle / bullets * i;
                    float zRot = (Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg) - dividedAngle;
                    weapon.rotation = Quaternion.Euler(0f, 0f, zRot + offset);
                    Rigidbody2D cloneGO = Instantiate(projectile, weapon.position, Quaternion.identity) as Rigidbody2D;
                    cloneGO.AddForce(weapon.transform.right * (projectileForce * forceOffset));
                    RaycastHit2D hit = Physics2D.Raycast(weapon.transform.position, weapon.transform.right);
                    Debug.DrawRay(weapon.transform.position, weapon.transform.right * 8, Color.red, fireRate / 2);
                }

                nextFireTime = Time.time + fireRate;
            }
        }
    }
}
