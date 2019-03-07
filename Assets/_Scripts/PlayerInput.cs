using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerInput : MonoBehaviourPunCallbacks, IPunObservable {

    [Tooltip("The gameobject for the cursor")]
    [SerializeField]
    private GameObject cursor;
    private Vector3 velocity;
    private PlayerCharacter pc;
    public float speed;
    private float baseSpeed;
    private float bonusSpeed;
    private float speedDebuff;
    private float camHeight;
    private float camWidth;
    private Camera cam;
    Vector3 oldPosition;
    Vector3 movement;
    Vector3 remoteMovement;
    Vector3 remotePosition;
    Vector3 remoteVelocity;
    Vector3 remoteOldPosition;
    float distCovered;
    float remoteSpeed;
    float lag;
    float journeyLength;
    float fracJourney;
    float lerpTime;
    float currentLerpTime;
    Rigidbody2D rb2d;


    void Start ()
    {
        pc = GetComponent<PlayerCharacter>();
        baseSpeed = speed;
        rb2d = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        camHeight = cam.orthographicSize;
        camWidth = camHeight * cam.aspect;

        if (!photonView.IsMine)
        {
            cursor.gameObject.SetActive(false);
        }
    }

    void Update ()
    {
        if (!photonView.IsMine)
        {
            transform.position = Vector3.MoveTowards(transform.position, remotePosition, Time.deltaTime * remoteSpeed);

            return;
        }

        if (!pc.Dead)
        {
            oldPosition = transform.position;

            velocity.x = Input.GetAxisRaw("Horizontal");
            velocity.y = Input.GetAxisRaw("Vertical");

            movement = transform.position - oldPosition;
            transform.Translate(velocity.normalized * speed * Time.deltaTime);
            Vector2 clampedPos = transform.position;
            clampedPos.x = Mathf.Clamp(transform.position.x, -19, 19);
            clampedPos.y = Mathf.Clamp(transform.position.y, -10, 10);
            transform.position = clampedPos;
        }

    }

    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    public float SpeedDebuff
    {
        get { return speedDebuff; }
        set { speedDebuff = value; }
    }

    public float BaseSpeed
    {
        get { return baseSpeed; }
    }

    public float BonusSpeed
    {
        get { return bonusSpeed; }
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(oldPosition);
            stream.SendNext(movement);
            stream.SendNext(transform.position);
            stream.SendNext(speed);
        }
        else
        {
            remoteOldPosition = (Vector3)stream.ReceiveNext();
            remoteMovement = (Vector3)stream.ReceiveNext(); ;
            remotePosition = (Vector3)stream.ReceiveNext();
            remoteSpeed = (float)stream.ReceiveNext();

            lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
            remotePosition += (remoteMovement * lag);
        }
    }

    // TODO build custom collsion detection
}
