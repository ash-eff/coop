using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Protons : MonoBehaviour
{
    ParticleSystem pSystem;
    public Rigidbody2D sparks;
    float projectileForce = 250;

    List<ParticleCollisionEvent> collisionEvents;

    private void Start()
    {
        pSystem = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    private void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(pSystem, other, collisionEvents);

        for (int i = 0; i < collisionEvents.Count; i++)
        {
            EmitAtLocation(collisionEvents[i]);
        }
    }

    void EmitAtLocation(ParticleCollisionEvent particleCollisionEvent)
    {
        Rigidbody2D cloneGO = Instantiate(sparks, particleCollisionEvent.intersection, Quaternion.identity) as Rigidbody2D;
        cloneGO.AddForce(particleCollisionEvent.normal * Random.Range(25, 75));
    }
}
