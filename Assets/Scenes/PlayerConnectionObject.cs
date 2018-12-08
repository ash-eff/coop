using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerConnectionObject : NetworkBehaviour {

    public GameObject playerUnitPrefab;
    public string playerName = "Anonymous";

    void Start ()
    {	
        // Is this actually my own local playerObject?
        if(!isLocalPlayer)
        {   
            // this object belongs to another player.
            return;
        }

        // Since the PlayerObject  is invisible and not part of the world,
        // give me something physical to move around!

        Debug.Log("PlayerObject::Start -- Spawning my own personal unit.");

        // Instantiate only creates an object on the LOCAL COMPUTER.
        // Even is it has a NetworkIdentity it still will NOT exist on
        // the network (and therefor not on any otyher client) UNLESS
        // NetworkServer.Spawn() is called on this object.

        //Instantiate(PlayerUnitPrefab);

        // Ask the server to spawn our unit

        CmdSpawnMyUnit();
	}

	void Update ()
    {
        // REMEMBER: Update runs on EVERYONE'S computer, whether or not they own this
        // particular player object.

        if (!isLocalPlayer)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            string n = "Prisoner " + Random.Range(647648, 937862);

            Debug.Log("Sending the server a request to change player name to " + n);
            CmdChangePlayerName(n);
        }
	}

    ///////////////////COMMANDS
    // Commands are special function that ONLY get executed on the server.

    [Command]
    void CmdSpawnMyUnit()
    {
        // We are guaranteed to be on the server right now.
        GameObject go = Instantiate(playerUnitPrefab);

        //go.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
        // Now that the object exists on the server, propagate it to all
        // the clients and also wire up the NetworkIdentity.

        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
    }

    [Command]
    void CmdChangePlayerName(string name)
    {
        Debug.Log("CmdChangePlayerName: " + name);
        playerName = name;
    }
}
