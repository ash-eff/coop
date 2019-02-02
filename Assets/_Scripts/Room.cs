using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Room : MonoBehaviourPunCallbacks
{
    public List<Vector2Int> exitTo = new List<Vector2Int>();
    public List<Vector2Int> enteredFrom = new List<Vector2Int>();
    public Dictionary<string, Sprite> spritesDict = new Dictionary<string, Sprite>();

    public Sprite[] sprites;
    public bool startingRoom;
    public bool endingRoom;
    public bool explored;
    public bool up;
    public bool right;
    public bool down;
    public bool left;

    public string doorDirection;
    public Sprite currentSprite;

    public SpriteRenderer spr;

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
        foreach (Sprite sprite in sprites)
        {
            spritesDict.Add(sprite.name.ToString(), sprite);
        }
    }

    public Vector2Int GetRoomPos()
    {
        return new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
    }

    [PunRPC]
    void AddToExitList(int x, int y)
    {
        Vector2Int newV2Int = new Vector2Int(x, y);
        Debug.Log(newV2Int + " added to Exit List");
        exitTo.Add(newV2Int);
    }

    [PunRPC]
    void AddToEntertList(int x, int y)
    {
        Vector2Int newV2Int = new Vector2Int(x, y);
        Debug.Log(newV2Int + " added to Enter List");
        enteredFrom.Add(newV2Int);
    }

    [PunRPC]
    void SetStartingRoom(bool b)
    {
        startingRoom = b;
        spr.color = Color.green;
    }

    [PunRPC]
    void SetEndingRoom(bool b)
    {
        endingRoom = b;
        spr.color = Color.red;
    }

    [PunRPC]
    void SetExplored(bool b)
    {
        explored = b;
    }

    [PunRPC]
    public void CheckDoorDirections()
    {
        foreach (Vector2Int exit in exitTo)
        {
            if (!endingRoom)
            {
                Vector2 direction = new Vector2(exit.x - transform.position.x, exit.y - transform.position.y);
                if (direction.y > 0 && !up)
                {
                    up = true;
                }
                if (direction.x > 0 && !right)
                {
                    right = true;
                }
                if (direction.y < 0 && !down)
                {
                    down = true;
                }
                if (direction.x < 0 && !left)
                {
                    left = true;
                }
            }
        }

        foreach (Vector2Int entrance in enteredFrom)
        {
            // TODO dirty way to not add an entrance to the start room. Come up with something more clever
            if (entrance.x >= 0)
            {
                Vector2 direction = new Vector2(entrance.x - transform.position.x, entrance.y - transform.position.y);
                if (direction.y > 0 && !up)
                {
                    up = true;
                }
                if (direction.x > 0 && !right)
                {
                    right = true;
                }
                if (direction.y < 0 && !down)
                {
                    down = true;
                }
                if (direction.x < 0 && !left)
                {
                    left = true;
                }
            }
        }

        SetDoor();
    }

    public void SetDoor()
    {

        if (up)
        {
            doorDirection += "Up";
        }
        if (right)
        {
            doorDirection += "Right";
        }
        if (down)
        {
            doorDirection += "Down";
        }
        if (left)
        {
            doorDirection += "Left";
        }
        if (up && right && down && left)
        {
            doorDirection = "All";
        }

        //photonView.RPC("SetSprite", RpcTarget.All, null);
        SetSprite();
    }

    void SetSprite()
    {
        foreach (KeyValuePair<string, Sprite> sprite in spritesDict)
        {
            if (sprite.Key == doorDirection)
            {
                currentSprite = sprite.Value;
                spr.sprite = currentSprite;
            }
        }
    }
}
