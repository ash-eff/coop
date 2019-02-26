using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[ExecuteInEditMode]
public class MapManagerNew : MonoBehaviour
{
    public GameObject generateButton;

    public _Node node;
    public GameObject marker;
    public _Node currentNode;
    private Vector2Int currentPosition;

    public Room startingRoom;
    public Room endingRoom;
    public Room room;
    public Room currentRoom;

    public int nodeSpacing = 5;
    public int gridWidth;
    public int gridHeight;
    public int numOfRooms;
    private int numberOfRooms;
    public int numOfNodesToSearch;
    public Vector2 startingPos = Vector2.zero;
    public Vector2Int nextNode;

    public int nameNum;

    private Dictionary<Vector2Int, _Node> grid = new Dictionary<Vector2Int, _Node>();
    private Dictionary<Vector2Int, Room> roomDict = new Dictionary<Vector2Int, Room>();

    private Vector2Int[] directions = { Vector2Int.right, Vector2Int.down, Vector2Int.up };

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            generateButton.SetActive(true);
            GenerateGrid();
        }
    }

    public void GenerateGrid()
    {
        numberOfRooms = numOfRooms;
        //if (grid != null)
        //{
        //    photonView.RPC("ClearGrid", RpcTarget.All, null);
        //}

        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                nextNode = new Vector2Int(Mathf.RoundToInt(startingPos.x + i * nodeSpacing), Mathf.RoundToInt(startingPos.y + j * nodeSpacing));
                
                grid.Add(nextNode, node);
            }
        }

        GetStartPosition();
    }

    //[PunRPC]
    public void ClearGrid()
    {
        _Node[] nodes = FindObjectsOfType<_Node>();
        Room[] rooms = FindObjectsOfType<Room>();

        foreach (_Node node in nodes)
        {
            Destroy(node.gameObject);
            //DestroyImmediate(node.gameObject);
        }

        foreach (Room room in rooms)
        {
            PhotonNetwork.Destroy(room.gameObject);
            //DestroyImmediate(room.gameObject);
        }

        nodes = null;
        rooms = null;
        nextNode = Vector2Int.zero;
        grid.Clear();
        roomDict.Clear();
        nameNum = 1;
    }

    void GetStartPosition()
    {
        Vector2Int startCoords;

        // pick a random starting X position on the 0 Y axis between 0 X Axis and gridwidth-1 X Axis
        int startXCoord = Random.Range(0, gridWidth) * nodeSpacing;
        //Debug.Log("Start X Coord: " + startXCoord);
        int startYCoord = 0;

        startCoords = new Vector2Int(startXCoord, startYCoord);
        // set to minus one, so that the rooms will skip adding an entrance on the first room generated
        CheckNode(new Vector2Int(startXCoord, startYCoord), new Vector2Int(-1, 0));
    }

    void CheckNode(Vector2Int _currentPosition, Vector2Int _enteredFrom)
    {
        if (numberOfRooms > 0)
        {
            foreach (KeyValuePair<Vector2Int, _Node> node in grid)
            {
                if (node.Key == _currentPosition)
                {
                    currentNode = node.Value;
                    currentPosition = node.Key;

                    if (currentNode.available == true)
                    {
                        Instantiate(marker, new Vector2(currentPosition.x, currentPosition.y), Quaternion.identity);
                    }

                    currentNode.available = false;
                    numberOfRooms--;
                    ExploreArea();
                }
            }
        }
    }

    //void AssignRoom(Node nodeToAssignTo, Vector2Int enteredFrom)
    //{
    //    GameObject roomHolder = GameObject.Find("RoomHolder");
    //    // instantiate rooms if there isn't a room, then count that as part of the room count
    //    if (!nodeToAssignTo.available)
    //    {
    //        Debug.Log("Assigning Room at: " + nodeToAssignTo.GetGridPos());
    //        GameObject theRoom = PhotonNetwork.Instantiate(room.name, new Vector2(nodeToAssignTo.GetGridPos().x, nodeToAssignTo.GetGridPos().y), Quaternion.identity);
    //        currentRoom = theRoom.GetComponent<Room>();
    //        roomDict.Add(nodeToAssignTo.GetGridPos(), currentRoom);
    //        currentRoom.name = "Room" + nameNum.ToString();
    //        //currentRoom.enteredFrom.Add(enteredFrom);
    //        currentRoom.photonView.RPC("AddToEntertList", RpcTarget.All, enteredFrom.x, enteredFrom.y);
    //        nameNum++;
    //        currentRoom.transform.parent = roomHolder.transform;
    //        nodeToAssignTo.available = true;
    //        numberOfRooms--;
    //        if (numberOfRooms == numOfRooms - 1)
    //        {
    //            startingRoom = currentRoom;
    //            //currentRoom.spr.color = Color.green;
    //            //currentRoom.startingRoom = true;
    //            currentRoom.photonView.RPC("SetStartingRoom", RpcTarget.All, true);
    //            ExploreArea(nodeToAssignTo);
    //        }
    //        else if (numberOfRooms == 0)
    //        {
    //            endingRoom = currentRoom;
    //            //currentRoom.spr.color = Color.red;
    //            //currentRoom.endingRoom = true;
    //            currentRoom.photonView.RPC("SetEndingRoom", RpcTarget.All, true);
    //            foreach (KeyValuePair<Vector2Int, Room> room in roomDict)
    //            {
    //                Room theRoomToCheck = room.Value;
    //                //Debug.Log("Checking Door Direction for: " + theRoomToCheck.name);
    //                theRoomToCheck.photonView.RPC("CheckDoorDirections", RpcTarget.All, null);
    //                //theRoomToCheck.CheckDoorDirections();
    //            }
    //
    //            if (PhotonNetwork.IsMasterClient)
    //            {
    //                //photonView.RPC("GetSeededStartPosition", RpcTarget.All, (object)directionSeeds);
    //            }
    //        }
    //        else
    //        {
    //            ExploreArea(nodeToAssignTo);
    //        }
    //
    //    }
    //    else
    //    {
    //        //Debug.Log("Room already assigned at: " + nodeToAssignTo.GetGridPos());
    //        foreach (KeyValuePair<Vector2Int, Room> room in roomDict)
    //        {
    //            if (room.Key == nodeToAssignTo.GetGridPos())
    //            {
    //                //Debug.Log("Key/Value Match!");
    //                currentRoom = room.Value;
    //                //currentRoom.enteredFrom.Add(enteredFrom);
    //                currentRoom.photonView.RPC("AddToEntertList", RpcTarget.All, enteredFrom.x, enteredFrom.y);
    //            }
    //        }
    //        // if there is  a room in the spot, don't instantiate a room, move on exploring
    //        //Debug.Log("There is a room here, moving on...");
    //        ExploreArea(nodeToAssignTo);
    //    }
    //}

    void ExploreArea()
    {
        _Node searchedNode;
        List<Vector2Int> validDirection = new List<Vector2Int>();

        // check all available nodes for an available slot first.
        foreach (Vector2Int direction in directions)
        {
            Vector2Int availableNode = currentPosition + (direction * nodeSpacing);
            if (grid.TryGetValue(availableNode, out searchedNode))
            {
                if (searchedNode.available)
                {
                    validDirection.Add(availableNode);
                }
            }
        }

        // if all the nodes were checked, but none had available slots, then allow checking unavailable slots
        if (validDirection.Count == 0)
        {
            foreach (Vector2Int direction in directions)
            {
                Vector2Int nextNode = currentPosition + (direction * nodeSpacing);
                if (grid.TryGetValue(nextNode, out searchedNode))
                {
                    validDirection.Add(nextNode);
                }
            }
        }

        //currentRoom.photonView.RPC("SetExplored", RpcTarget.All, true);

        PickDirection(validDirection);
    }

    void PickDirection(List<Vector2Int> validExits)
    {
        // randomly select a valid direction
        int selection = Random.Range(0, validExits.Count);
        // and assign it to direction
        Vector2Int direction = validExits[selection];

        // send the direction to AssignRoom
        if (direction != null)
        {
            CheckNode(direction, currentPosition);
        }
    }
}
