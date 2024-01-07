using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System; //for EventHandler

public class ExitDoorController : GenericCollectibleItem
{
    //create Singleton object of this class.
    private static ExitDoorController instance;
    public static ExitDoorController Instance
    {
        get { return instance; }//not very different from getters and setters
        private set { instance = value; }
    }

    private MazeCell containerCell;//this stored the cell in which Exit Door is spawned

    public event EventHandler OnExitDoorOpen;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;//start by setting the Singleton instance 
        }
        else
        {
            Debug.LogError("Fatal Error: Cannot have a predefined instance of Exit Door");
            Destroy(this);
        }
        instance.destroyObjectOnCollect = false;//Door should not be destroyed on collection
        instance.isObjectMovable = false;//Door cannot move
        instance.correctCollectingPlayer = isCollectableBy.NoPlayer;
        //Door needs to be activated first.

        Debug.Log("Exit Door instance instantiated");

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenExitDoorForBothPlayers()
    {
        if (!PlayerTwoController.Instance.HasCollectedExitKey())
        {
            return;//collected in this case means that PlayerTwo reached the door with the key.
            //Exit Door will not be marked Collected without key, since its status updates from NoPlayer only after Key Collection.
        }
        //if PlayerTwo has reached ExitDoor with Key, then it should be open and accessible to PlayerOne also.
        instance.correctCollectingPlayer = isCollectableBy.BothActivePlayers;//Allow PlayerOne to Enter
        instance.isObjectCollected = true;
    }

    public bool IsExitDoorOpen() 
        {  
            return instance.IsCollected(); 
        }


    public void EnableExitDoorForPlayerTwoOnExitKeyCollectEvent(object key, EventArgs e)
    {
        //Debug.Log("ExitDoor is Listening to Exit Key Collection Event. PlayerTwo can Open Exit door");
        instance.correctCollectingPlayer = isCollectableBy.playerTwoOnly;
        //this will be called when OnExitKeyCollect event is fired.
        //Only PlayerTwo can access it until he reaches the door.
    }

    public Vector3 GetExitDoorPosition()
    {
        return instance.transform.position;
    }

    public void SetExitDoorPosition(Vector3 newPosition)
    {
        ResetExitDoorForNextLevel();//if position is being set explicitly, then it is not collected.
        instance.transform.position = newPosition;
        
    }

    public void ResetExitDoorForNextLevel()
    {
        instance.isObjectCollected = false;//if position is being set explicitly, then it is not collected.
        instance.correctCollectingPlayer = isCollectableBy.NoPlayer;//needs the key to be collected.
    }

    public MazeCell GetExitDoorContainerCell()
    {
        return instance.containerCell;
    }

    public void SetExitDoorContainerCell(MazeCell cell)
    {
        instance.containerCell = cell;
    }

}
