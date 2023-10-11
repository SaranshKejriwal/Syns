using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ExitKey is also collectible by Player, but PlayerTwo only
public class ExitKeyController : GenericCollectibleItem
{
    //create Singleton object of this class.
    private static ExitKeyController instance;
    public static ExitKeyController Instance
    {
        get { return instance; }//not very different from getters and setters
        private set { instance = value; }
    }


    // Start is called before the first frame update
    private float hoverGapAbovePlayerTwoVisual = 1.5f;
    private bool keyAlreadyCollected = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;//start by setting the Singleton instance 
        }
        else
        {
            Debug.LogError("Fatal Error: Cannot have a predefined instance of Exit Key");
            Destroy(this);
        }
        instance.destroyObjectOnCollect = false;//key should not be destroyed on collection
        instance.isObjectMovable = true;
        instance.correctCollectingPlayer = isCollectableBy.playerTwoOnly;//Key cannot be collected by PlayerOne

    }

    void Start()
    {
        //instance.itemDetectionDistance = 1000;//for testing only
        //instance.itemCollectionDistance = 1000;//for testing only
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameMaster.Instance.IsLevelPlaying())
        {
            return;//do nothing if game is paused or level has ended.
        }

        if (!keyAlreadyCollected)
        //As soon as setKeyParentAsPlayerTwo() is called once, this will be true and Update() will do nothing.
        {            
            base.IsActivePlayerInVicinityForCollection(PlayerTwoController.Instance);
            //ExitKey is not interested in PlayerOne

            setKeyParentAsPlayerTwo();
        }
    }

    
    public void setKeyParentAsPlayerTwo()
    {
        if (!isObjectCollected)
        {
            return;//Key has to be in vicinity of Player Two
        }

        Vector3 keySizeOnCollect = new Vector3(0.7f, 0.7f, 0.7f);
        //this will directly make the Key move relative to PlayerTwo, without calling in Update each time.
        transform.parent = PlayerTwoController.Instance.transform;
        transform.position = GetHoverLocationAbovePlayerTwoVisual();
        transform.localScale = keySizeOnCollect;

        PlayerTwoController.Instance.SetHasCollectedExitKey(true);
        keyAlreadyCollected= true;
        //we're maintaining this separate bool so that
        //the above setter doesn't keep getting called infinitely.
    }

    private Vector3 GetHoverLocationAbovePlayerTwoVisual()
    {
        //this inefficient method is acceptable because it will be called only once, whenever playerTwo is in ExitKey vicinity
        return new Vector3(PlayerTwoController.Instance.GetPlayerPosition().x, PlayerTwoController.Instance.GetPlayerPosition().y+hoverGapAbovePlayerTwoVisual, PlayerTwoController.Instance.GetPlayerPosition().z);
    }
}
