using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerInput : MonoBehaviourPunCallbacks {

    [Tooltip("The gameobject for the cursor")]
    [SerializeField]
    private GameObject cursor;

    private Vector3 velocity;
    private float speed = 15f;
    private float camHeight;
    private float camWidth;
    private Camera cam;

    void Start ()
    {
        cam = Camera.main;
        camHeight = cam.orthographicSize;
        camWidth = camHeight * cam.aspect;
    }

    void Update ()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        velocity.x = Input.GetAxisRaw("Horizontal");
        velocity.y = Input.GetAxisRaw("Vertical");

        transform.Translate(velocity.normalized * speed * Time.deltaTime);
    }

    private void LateUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // direction from the character to the mouse position
        Vector3 dirToMouse = ray.origin - transform.position;
        // sets a position for the cursor based on where the character has moved
        Vector3 cursorPos = transform.position + dirToMouse;
        Vector3 cursorOffset = cursorPos - transform.position;

        // these offsets use the distance of the camera to the character to set the "center"
        // and calculated the offset using this distance + the cam view area
        float camWidthOffset = Mathf.Abs(cam.transform.position.x - transform.position.x) + camWidth;
        float camHeightOffset = Mathf.Abs(cam.transform.position.y - transform.position.y) + camHeight;

        // using the characters position as the center
        // allow cursor to reach the edge of the screen no matter what the limitations on the camera placement are

        cursor.transform.position = transform.position + new Vector3(Mathf.Clamp(cursorOffset.x, -camWidthOffset, camWidthOffset),
                                                                     Mathf.Clamp(cursorOffset.y, -camHeightOffset, camHeightOffset),
                                                                     0f);
    }

    // TODO build custom collsion detection
}
