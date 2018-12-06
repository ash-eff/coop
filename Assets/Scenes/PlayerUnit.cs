using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// A PlayerUnit is a unit controller by a player

public class PlayerUnit : NetworkBehaviour {

    public float speed;

    private Vector3 movement;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        // if hasAuthority is true, I am allowed to mess around with this object
        if(!hasAuthority)
        {
            return;
        }
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        transform.Translate(movement * speed * Time.deltaTime);
	}
}
