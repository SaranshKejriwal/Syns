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
        base.IsActivePlayerInVicinityForCollection(PlayerTwoController.Instance);
        CheckExitDoorOpenStatus();
    }

    private void CheckExitDoorOpenStatus()
    {
        if (!isObjectCollected)
        {
            return;//collected in this case means that PlayerTwo reached the door with the key.
        }
        //if PlayerTwo has reached ExitDoor with Key, then it should be open and accessible to PlayerOne also.
        instance.isExitDoorOpen = true;
        instance.correctCollectingPlayer = isCollectableBy.BothActivePlayers;

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
}
