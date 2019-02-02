using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraConstraint : MonoBehaviour
{
    // cached transform of the camera
    [SerializeField]
    private Transform cameraTransform;

    // cached transform of the target character
    [SerializeField]
    private Transform targetTransform;

    // cached transform of the target character's cursor
    [SerializeField]
    private Transform cursorTransform;

    public Transform[] transformBounds;
    public Vector3 offset;

    private Bounds bounds;

    private void Awake()
    {
        //cameraTransform = transform;
        //targetTransform = gameObject.transform;
        //transformBounds[0] = targetTransform;
        //transformBounds[1] = cursorTransform;
    }

    private void LateUpdate()
    {
        Vector3 centerPoint = GetCenterPoint();

        Vector3 newPosition = centerPoint + offset;

        transform.position = newPosition;
    }

    Vector3 GetCenterPoint()
    {
        var bounds = new Bounds(transformBounds[0].position, Vector3.zero);
        for (int i = 0; i < transformBounds.Length; i++)
        {
            bounds.Encapsulate(transformBounds[i].position);
        }

        return bounds.center;
    }
}
