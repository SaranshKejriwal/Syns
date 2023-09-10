using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ExitKey is also collectible by Player, but PlayerTwo only
public class ExitKeyController : GenericCollectibleItem
{
    //create Singleton object of this class.
    private static ExitKeyController instance;
    public static ExitKeyController Instance
    //this instance "Property" will be tracked by PlayerTwo Only
    {
        get { return instance; }//not very different from getters and setters
        private set { instance = value; }//we do not want any other object to modify PlayerTwo entirely.
    }


    // Start is called before the first frame update
    private int hoverGapAbovePlayerTwoVisual = 2;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;//start by setting the Singleton instance 
        }
        else
        {
            Debug.Log("Fatal Error: Cannot have a predefined instance of PlayerTwo");
        }
        instance.destroyObjectOnCollect = false;//key should not be destroyed on collection
        instance.isObjectMovable = true;
        instance.itemCollectionStatus = isCollectableBy.playerTwoOnly;//Key cannot be collected by PlayerOne

    }

    void Start()
    {
        
        setParentAsPlayerTwo();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setParentAsPlayerTwo()
    {
        //this will directly make the Key move relative to PlayerTwo, without calling in Update each time.
        transform.parent = PlayerTwoController.Instance.transform;
        transform.position = GetHoverLocationAbovePlayerTwoVisual();
        
    }

    private Vector3 GetHoverLocationAbovePlayerTwoVisual()
    {
        //this inefficient method is acceptable because it will be called only once, whenever playerTwo is in ExitKey vicinity
        return new Vector3(PlayerTwoController.Instance.GetPlayerTwoLocation().x, PlayerTwoController.Instance.GetPlayerTwoLocation().y+hoverGapAbovePlayerTwoVisual, PlayerTwoController.Instance.GetPlayerTwoLocation().z);
    }
}
