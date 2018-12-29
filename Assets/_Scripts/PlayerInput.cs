using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerInput : MonoBehaviourPunCallbacks {

    //private Rigidbody2D rb2d;
    private PhotonView pView;
    private Vector3 velocity;
    private float speed = 15f;

	// Use this for initialization
	void Start ()
    {
        //rb2d = GetComponent<Rigidbody2D>();
        pView = GetComponent<PhotonView>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!pView.IsMine)
        {
            Debug.Log("Not My PhotonView");
            return;
        }

        velocity.x = Input.GetAxisRaw("Horizontal");
        velocity.y = Input.GetAxisRaw("Vertical");

        transform.Translate(velocity.normalized * speed * Time.deltaTime);
    }

    // TODO build custom collsion detection
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Obstacle")
        {
            print("HIT!");
        }
    }
}
