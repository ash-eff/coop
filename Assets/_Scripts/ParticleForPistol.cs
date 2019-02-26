using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleForPistol : MonoBehaviour
{
    public ParticleSystem pistol;
    public ParticleSystem sparks;
    public ParticleSystem bulletRicochet;
    ParticleSystem.CollisionModule coll;

    List<ParticleCollisionEvent> collisionEvents;

    private void Start()
    {
        collisionEvents = new List<ParticleCollisionEvent>();
        coll = pistol.collision;
    }

    private void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(pistol, other, collisionEvents);
        
        if (other.gameObject.tag == "Obstacle")
        {
            for (int i = 0; i < collisionEvents.Count; i++)
            {
                EmitAtLocation(collisionEvents[i]);
            }
        }
    }

    void EmitAtLocation(ParticleCollisionEvent particleCollisionEvent)
    {
        //Debug.Log("Pew");
        //float dotProduct = Vector3.Dot(transform.position, particleCollisionEvent.intersection);
        //Vector3 direction = particleCollisionEvent.intersection + transform.position;
        //Vector3 reflection = -2 * dotProduct * direction;
        sparks.transform.position = particleCollisionEvent.intersection;
        sparks.transform.rotation = Quaternion.LookRotation(particleCollisionEvent.normal);
        //bulletRicochet.transform.position = particleCollisionEvent.intersection;
        //bulletRicochet.transform.rotation = Quaternion.LookRotation(reflection);
        sparks.Emit(3);
        //bulletRicochet.Emit(1);
    }
}
