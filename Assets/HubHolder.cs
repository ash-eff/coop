using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HubHolder : MonoBehaviourPunCallbacks {

    public bool occupied;
    public int viewId;

    public bool Occupied
    {
        get { return occupied; }
        set { occupied = value; }
    }
}
