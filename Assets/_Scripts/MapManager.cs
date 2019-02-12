using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MapManager : MonoBehaviourPunCallbacks
{
    public GameObject generateButton;

    public Node node;
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
    public Vector2 nextNode;

    public int nameNum;

    private Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();
    private Dictionary<Vector2Int, Room> roomDict = new Dictionary<Vector2Int, Room>();

    private Vector2Int[] directions = { Vector2Int.right, Vector2Int.down, Vector2Int.left, Vector2Int.up };

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
        GameObject gridHolder = GameObject.Find("GridHolder");
        if(grid != null)
        {
            photonView.RPC("ClearGrid", RpcTarget.All, null);
        }

        Node obj = null;
        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                nextNode = new Vector2(startingPos.x + i * nodeSpacing, startingPos.y + j * nodeSpacing);
                obj = Instantiate(node, nextNode, Quaternion.identity);
                obj.transform.parent = gridHolder.transform;
                grid.Add(obj.GetGridPos(), obj);
            }
        }

        GetStartPosition();
    }

    [PunRPC]
    public void ClearGrid()
    {
        Node[] nodes = FindObjectsOfType<Node>();
        Room[] rooms = FindObjectsOfType<Room>();

        foreach(Node node in nodes)
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
        nextNode = Vector2.zero;
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
        CheckNode(new Vector2Int(startXCoord, startYCoord), new Vector2Int(-1,0));
    }

    void CheckNode(Vector2Int pos, Vector2Int enteredFrom)
    {
        if(numberOfRooms > 0)
        {
            Node checkingNode;
            //Debug.Log("Checking..." + pos);
            checkingNode = null;
            foreach (KeyValuePair<Vector2Int, Node> node in grid)
            {
                if (node.Key == pos)
                {
                    //Debug.Log("Key/Value Match!");
                    checkingNode = node.Value;
                    AssignRoom(checkingNode, enteredFrom);
                }
            }
        }
    }

    void AssignRoom(Node nodeToAssignTo, Vector2Int enteredFrom)
    {
        GameObject roomHolder = GameObject.Find("RoomHolder");
        // instantiate rooms if there isn't a room, then count that as part of the room count
        if (!nodeToAssignTo.available)
        {
            Debug.Log("Assigning Room at: " + nodeToAssignTo.GetGridPos());
            GameObject theRoom = PhotonNetwork.Instantiate(room.name, new Vector2(nodeToAssignTo.GetGridPos().x, nodeToAssignTo.GetGridPos().y), Quaternion.identity);
            currentRoom = theRoom.GetComponent<Room>();
            roomDict.Add(nodeToAssignTo.GetGridPos(), currentRoom);
            currentRoom.name = "Room" + nameNum.ToString();
            //currentRoom.enteredFrom.Add(enteredFrom);
            currentRoom.photonView.RPC("AddToEntertList", RpcTarget.All, enteredFrom.x, enteredFrom.y);
            nameNum++;
            currentRoom.transform.parent = roomHolder.transform;
            nodeToAssignTo.available = true;
            numberOfRooms--;
            if(numberOfRooms == numOfRooms - 1)
            {
                startingRoom = currentRoom;
                //currentRoom.spr.color = Color.green;
                //currentRoom.startingRoom = true;
                currentRoom.photonView.RPC("SetStartingRoom", RpcTarget.All, true);
                ExploreArea(nodeToAssignTo);
            }
            else if(numberOfRooms == 0)
            {
                endingRoom = currentRoom;
                //currentRoom.spr.color = Color.red;
                //currentRoom.endingRoom = true;
                currentRoom.photonView.RPC("SetEndingRoom", RpcTarget.All, true);
                foreach (KeyValuePair<Vector2Int, Room> room in roomDict)
                {
                    Room theRoomToCheck = room.Value;
                    //Debug.Log("Checking Door Direction for: " + theRoomToCheck.name);
                    theRoomToCheck.photonView.RPC("CheckDoorDirections", RpcTarget.All, null);
                    //theRoomToCheck.CheckDoorDirections();
                }

                if (PhotonNetwork.IsMasterClient)
                {
                    //photonView.RPC("GetSeededStartPosition", RpcTarget.All, (object)directionSeeds);
                }
            }
            else
            {
                ExploreArea(nodeToAssignTo);
            }
            
        }
        else
        {
            //Debug.Log("Room already assigned at: " + nodeToAssignTo.GetGridPos());
            foreach (KeyValuePair<Vector2Int, Room> room in roomDict)
            {
                if (room.Key == nodeToAssignTo.GetGridPos())
                {
                    //Debug.Log("Key/Value Match!");
                    currentRoom = room.Value;
                    //currentRoom.enteredFrom.Add(enteredFrom);
                    currentRoom.photonView.RPC("AddToEntertList", RpcTarget.All, enteredFrom.x, enteredFrom.y);
                }
            }
            // if there is  a room in the spot, don't instantiate a room, move on exploring
            //Debug.Log("There is a room here, moving on...");
            ExploreArea(nodeToAssignTo);
        }
    }

    void ExploreArea(Node node)
    {
        //Debug.Log("Exploring...");
        Node neighborNode = null;
        List<Vector2Int> validExits= new List<Vector2Int>();

        // this will make it so that the path chosen is one where rooms haven't already been added
        foreach(Vector2Int direction in directions)
        {
            Vector2Int explored = node.GetGridPos() + (direction * nodeSpacing);
            if(grid.TryGetValue(explored, out neighborNode))
            {
                if (!neighborNode.available)
                {
                    validExits.Add(explored);
                }
            }
        }

        if(validExits.Count == 0)
        {
            foreach (Vector2Int direction in directions)
            {
                Vector2Int explored = node.GetGridPos() + (direction * nodeSpacing);
                if (grid.TryGetValue(explored, out neighborNode))
                {
                    validExits.Add(explored);
                }
            }
        }

        //currentRoom.explored = true;
        currentRoom.photonView.RPC("SetExplored", RpcTarget.All, true);

        PickExit(validExits);
    }

    void PickExit(List<Vector2Int> validExits)
    {
        // randomly select a valid exit
        int selection = Random.Range(0, validExits.Count);
        // and assign it to direction
        Vector2Int direction = validExits[selection];
        //currentRoom.exitTo.Add(direction);
        currentRoom.photonView.RPC("AddToExitList", RpcTarget.All, direction.x, direction.y);
        //currentRoom.doorDir = new Vector2(direction.x - currentRoom.transform.position.x, direction.y - currentRoom.transform.position.y);
        //Debug.Log("Picking..." + direction);

        // send the direction to AssignRoom
        if (direction != null)
        {
            CheckNode(direction, currentRoom.GetRoomPos());
        }
    }
}
