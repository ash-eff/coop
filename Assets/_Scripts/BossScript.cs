using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BossScript : MonoBehaviourPunCallbacks
{
    public Rigidbody2D projectile;
    public GameObject pivot;
    public int numberOfProjectiles;
    float remainingProjectiles;
    public float timeToWait;
    float rotVariable;

    private void Start()
    {
        rotVariable = 360 / (numberOfProjectiles / 4);
    }

    public void ActivateBoss()
    {
        photonView.RPC("ShootBullets", RpcTarget.AllViaServer, null);
    }

    [PunRPC]
    void ShootBullets()
    {
        StartCoroutine("IShootBullets");
    }

    IEnumerator IShootBullets()
    {
        remainingProjectiles = numberOfProjectiles;
        float rotAmount = rotVariable;
        while (remainingProjectiles > 0)
        {
            Rigidbody2D go = Instantiate(projectile, pivot.transform.position, pivot.transform.rotation) as Rigidbody2D;
            pivot.transform.rotation = Quaternion.Euler(0f, 0f, rotAmount);
            rotAmount += rotVariable;
            remainingProjectiles--;
            yield return new WaitForSeconds(timeToWait);
        }
    }
}
