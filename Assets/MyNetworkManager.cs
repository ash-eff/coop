using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MyNetworkManager : NetworkManager {

    public void MyStartHost()
    {
        StartHost();
        Debug.Log(Time.timeSinceLevelLoad + "Starting Host.");
    }

    public override void OnStartHost()
    {
        Debug.Log(Time.timeSinceLevelLoad + "Host Started.");
    }

    public override void OnStartClient(NetworkClient myClient)
    {
        Debug.Log(Time.timeSinceLevelLoad + "Client start requested.");
        //base.OnStartClient(myClient);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        Debug.Log(Time.timeSinceLevelLoad + "Client Connected to IP: " + conn.address);
        //base.OnClientConnect(conn);
    }
}
