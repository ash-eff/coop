using UnityEngine;

/// <summary>
/// Camera work. Follow a target but allow the camera to move away from the target within a certain range based on the mouse
/// position and a bounding box. The goal is create space for the player and allow him to see further in any given direction.
/// </summary>
public class CameraWork : MonoBehaviour {

    // cached transform of the camera
    private Transform cameraTransform;

    // The Character and Cursor in an Array for bounds to use
    public Transform[] transformBounds;

    // Any offset for the Camera to use
    public Vector3 offset;

    private Bounds bounds;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        Vector3 centerPoint = GetCenterPoint();
        //var heading = centerPoint - transformBounds[0].position;
        //var distance = heading.magnitude;
        //var direction = heading / distance;

        Vector3 newPosition = centerPoint + offset;
        //Vector3 newPosition = centerPoint + offset;
        //Debug.Log("Center Position plus offset: " + (centerPoint + offset));
        //Debug.Log("Center Position /w direciton plus offset: " + ((centerPoint - direction / 1.1f) + offset));

        cameraTransform.position = newPosition;
       // Debug.Log("Normal: " + newPosition);
       // Debug.Log("Half: " + (newPosition / 2)) ;
    }

    Vector3 GetCenterPoint()
    {
        var bounds = new Bounds(transformBounds[0].position, Vector3.zero);
        for(int i = 0; i < transformBounds.Length; i++)
        {
            bounds.Encapsulate(transformBounds[i].position);
        }

        return bounds.center;
    }
}