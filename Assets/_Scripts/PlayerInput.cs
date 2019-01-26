using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerInput : MonoBehaviourPunCallbacks {

    public Vector3 camOffset;
    public float radius;
    private Vector3 velocity;
    private float speed = 15f;
    private Vector3 cursorPos;
    private Vector2 directionToMouse;
    public GameObject cursor;
    private float camHeight;
    private float camWidth;
    //TODO for testing
    public RadiusTest curNum;
    public bool testing;

    void Start ()
    {
        camHeight = Camera.main.orthographicSize;
        camWidth = camHeight * Camera.main.aspect;
        curNum = FindObjectOfType<RadiusTest>();
    }

    void Update ()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        //TODO for testing
        if (testing)
        {
            radius = curNum.cursorRadius;
        }

        //clampOffset = new Vector3((camWidth / clampOffsetOffset) - .5f, (camHeight / clampOffsetOffset) - .5f, 0f);

        velocity.x = Input.GetAxisRaw("Horizontal");
        velocity.y = Input.GetAxisRaw("Vertical");

        transform.Translate(velocity.normalized * speed * Time.deltaTime);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // pointing from the character to the cursor
        Vector3 heading = ray.origin - transform.position;
        Vector3 newPos = transform.position + heading;
        Vector3 offset = newPos - transform.position;

        Vector3 constraintSize = new Vector3(camWidth, camHeight, 0f);

        //cursor.transform.position = newPos;
        //cursor.transform.position = transform.position + Vector3.ClampMagnitude(offset, radius);
        //cursor.transform.position = transform.position + new Vector3(Mathf.Clamp(offset.x, -constraintSize.x, +constraintSize.x),
                                                                     //Mathf.Clamp(offset.y, -constraintSize.y, +constraintSize.y), 
                                                                     //0f);

        //Vector3 clampedPosition = cursor.transform.position - transform.position;
        cursor.transform.position = new Vector2(transform.position.x, transform.position.y) + Vector2.ClampMagnitude(offset, radius);
    }

    // TODO build custom collsion detection
}
