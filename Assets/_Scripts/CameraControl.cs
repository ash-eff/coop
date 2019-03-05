using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraControl : MonoBehaviourPunCallbacks
{

    // cached transform of the camera
    private Transform cameraTransform;

    // The Character and Cursor in an Array for bounds to use
    public Transform[] transformBounds;

    // Any offset for the Camera to use
    public Vector3 offset;

    public float smoothTime;

    public float radius;

    private Bounds bounds;
    private Vector3 velocity;
    public bool isFollowing;
    public Vector3 centerPoint;
    public Vector3 newPosition;
    public Vector3 clampedPosition;
    float lerpTime = 3f;
    float currentLerpTime;
    public bool targetDead;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if(photonView.IsMine)
        {
            // The transform target may not destroy on level load,
            // so we need to cover corner cases where the Main Camera is different everytime we load a new scene, and reconnect when that happens
            if (cameraTransform == null && isFollowing)
            {
                OnStartFollowing();
            }
            // only follow is explicitly declared
            if (isFollowing && !targetDead)
            {
                SmoothFollow();
            }

            if (isFollowing && targetDead)
            {
                StartCoroutine(SetToObserve());
            }
        }
    }

    public void OnStartFollowing()
    {
        cameraTransform = Camera.main.transform;
        isFollowing = true;
        // we don't smooth anything, we go straight to the right camera shot
        SmoothFollow();
    }

    IEnumerator SetToObserve()
    {
        isFollowing = false;
        Vector3 endPos = new Vector3(0f, 0f, -10f);
        Vector3 startPos = cameraTransform.position;
        bool inPos = false;

        while (!inPos)
        {
            if(cameraTransform.position.x == 0 && cameraTransform.position.y == 0)
            {
                inPos = true;
            }

            currentLerpTime += Time.deltaTime;
            float perc = currentLerpTime / lerpTime;
            cameraTransform.position = Vector3.Lerp(startPos, endPos, perc);
            Camera.main.orthographicSize = Mathf.Lerp(6, 12, perc);

            yield return new WaitForEndOfFrame();
        }
    }

    void SmoothFollow()
    {
        centerPoint = GetCenterPoint();
        
        newPosition = centerPoint + offset;

        cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, newPosition, ref velocity, smoothTime);
        clampedPosition = cameraTransform.position - transform.position;
        cameraTransform.position = transform.position + Vector3.ClampMagnitude(clampedPosition, radius);
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
