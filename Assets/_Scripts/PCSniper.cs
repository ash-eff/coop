using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(LineRenderer))]
public class PCSniper : MonoBehaviourPunCallbacks
{
    public LayerMask enemyMask;
    LineRenderer lr;
    Vector3 startPos;
    Vector3 endPos;
    WeaponShotgun shotgun;
    CharacterInfo info;
    PlayerInput input;
    public Collider2D laserCol;

    private void Start()
    {
        shotgun = GetComponent<WeaponShotgun>();
        info = GetComponent<CharacterInfo>();
        lr = GetComponent<LineRenderer>();
        input = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        startPos = new Vector3(transform.position.x, transform.position.y, 0);
        lr.SetPosition(0, startPos);
        Vector2 direction = input.cursor.transform.position - transform.position;
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, direction, Mathf.Infinity, enemyMask);
        if (hit)
        {
            lr.SetPosition(1, hit.point);
            laserCol.transform.position = hit.point;
        }
        else
        {
            lr.SetPosition(1, input.cursor.transform.position);
            laserCol.transform.position = input.cursor.transform.position;
        }
    }
}
