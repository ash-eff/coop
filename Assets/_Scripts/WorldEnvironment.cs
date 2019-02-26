using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEnvironment : MonoBehaviour
{
    public GameObject marker;

    private void OnParticleCollision(GameObject other)
    {
        print("hit by " + other.name + " at " + other.gameObject.transform.position);
        Instantiate(marker, other.gameObject.transform.position, Quaternion.identity);
    }
}
