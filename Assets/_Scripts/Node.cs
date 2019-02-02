using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public bool hasRoom;
    public bool startingNode;
    public bool endingNode;
    public bool miniBossNode;
    public bool roomNode;
    public int roomWidth;
    public int roomHeight;
    public SpriteRenderer spr;

    public Vector2Int GetGridPos()
    {
        return new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
    }

    public void SetNodeType()
    {
        if (startingNode)
        {
            spr.color = Color.green;
        }

        if (endingNode)
        {
            spr.color = Color.red;
        }

        if (!hasRoom)
        {
            spr.color = Color.blue;
        }

        if (roomNode)
        {
            spr.color = Color.magenta;
        }
    }
}
