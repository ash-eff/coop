using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Node : MonoBehaviour
{
    public bool available = true;
    public List<Vector2Int> exitTo = new List<Vector2Int>();
    public List<Vector2Int> enteredFrom = new List<Vector2Int>();

    public Vector2Int GetGridPos()
    {
        return new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
    }
}
