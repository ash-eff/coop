using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    private Vector3 input;

	// Update is called once per frame
	void Update () {
		if(!isLocalPlayer)
        {
            return;
        }

        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        input.z = 0f;

        transform.Translate(input);
	}
}
