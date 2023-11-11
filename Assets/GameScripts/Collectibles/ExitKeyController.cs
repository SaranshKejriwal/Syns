using System;
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
    
    public event EventHandler OnExitKeyCollect;//this event will be fired when PlayerTwo collects ExitKey



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
        //Set subscribers to OnExitKeyCollect event
        OnExitKeyCollect += SetKeyParentAsPlayerTwoOnExitKeyCollectEvent;
        OnExitKeyCollect += ExitDoorController.Instance.EnableExitDoorForPlayerTwoOnExitKeyCollectEvent;

    }

    // Update is called once per frame
    void Update()
    {
        if (!GameMaster.Instance.IsLevelPlaying())
        {
            return;//do nothing if game is paused or level has ended.
        }

        if (this.isObjectCollected)
        {
            return;//do nothing if key is already collected.
        }

        if (base.IsActivePlayerInVicinityForCollection(PlayerTwoController.Instance))
        {
            if(OnExitKeyCollect != null)
            {
                OnExitKeyCollect(this, EventArgs.Empty);//fire the event for exit key collection
            }

        }
    }

    public void SetKeyParentAsPlayerTwoOnExitKeyCollectEvent(object key, EventArgs e)
    {
        Vector3 keySizeOnCollect = new Vector3(0.7f, 0.7f, 0.7f);
        //this will directly make the Key move relative to PlayerTwo, without calling in Update each time.
        transform.parent = PlayerTwoController.Instance.transform;
        transform.position = GetHoverYOffsetAbovePlayerTwoVisual();
        transform.localScale = keySizeOnCollect;

        PlayerTwoController.Instance.SetHasCollectedExitKey(true);
    }

    private Vector3 GetHoverYOffsetAbovePlayerTwoVisual()
    {
        return PlayerTwoController.Instance.GetPlayerPosition()+ new Vector3(0,hoverGapAbovePlayerTwoVisual,0);
    }
}
