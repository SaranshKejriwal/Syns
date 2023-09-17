using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoorController : GenericCollectibleItem
{
    //create Singleton object of this class.
    private static ExitDoorController instance;
    public static ExitDoorController Instance
    {
        get { return instance; }//not very different from getters and setters
        private set { instance = value; }
    }

    //ExitDoor specific objects
    private bool isExitDoorOpen = false;

    private MazeCell containerCell;//this stored the cell in which Exit Door is spawned

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

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckExitDoorCollectedStatus()
    {
        if (!PlayerTwoController.Instance.HasCollectedExitKey() || !PlayerTwoController.Instance.CanEnterExitDoorInVicinity())
        {
            return;//collected in this case means that PlayerTwo reached the door with the key.
            //Exit Door will not be marked Collected without key, since its status updates from NoPlayer only after Key Collection.
        }
        //if PlayerTwo has reached ExitDoor with Key, then it should be open and accessible to PlayerOne also.
        instance.isExitDoorOpen = true;
        instance.correctCollectingPlayer = isCollectableBy.BothActivePlayers;//Allow PlayerOne to Enter
        Debug.Log("Exit Door can now be entered");
        //PlayerTwoController.Instance.EnterOpenExit();

        //fire an event here for victory.
    }

    public bool IsExitDoorOpen() 
        {  
            return isExitDoorOpen; 
        }

    public void EnableExitDoorForPlayerTwo()
    {
        Debug.Log("PlayerTwo has collected Exit Key. Can open Exit Door");
        instance.correctCollectingPlayer = isCollectableBy.playerTwoOnly;
        //this will be called when playerTwo collects the Key.
        //Only PlayerTwo can access it until he reaches the door.
    }

    public Vector3 GetExitDoorPosition()
    {
        return instance.transform.position;
    }

    public void RevealExitDoor()
    {
        //this method can be used to show the visual for Exit Door once PlayerTwo has collected the Key.
        //hiding exit door until exit key is retrieved, warrants PlayerTwo to retraverse the Maze.
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
