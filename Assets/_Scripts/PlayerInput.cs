using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerInput : MonoBehaviourPunCallbacks {

    //private Rigidbody2D rb2d;
    private PhotonView pView;
    private Vector3 velocity;
    private float speed = 15f;

	void Start ()
    {
        //rb2d = GetComponent<Rigidbody2D>();
        pView = GetComponent<PhotonView>();
	}

	void Update ()
    {
        if (!pView.IsMine)
        {
            return;
        }

        velocity.x = Input.GetAxisRaw("Horizontal");
        velocity.y = Input.GetAxisRaw("Vertical");

        transform.Translate(velocity.normalized * speed * Time.deltaTime);
    }

    // TODO build custom collsion detection
}
