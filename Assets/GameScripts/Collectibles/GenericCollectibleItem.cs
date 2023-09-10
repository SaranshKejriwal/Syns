using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class will be the parent of all collectibles - Gold, Health and ExitKey
//It will define the generic behaviour on player vicinity, and each class will implement an onCollection method
public class GenericCollectibleItem : MonoBehaviour
{
    //this can be PlayerOne, PlayerTwo, Sack, Shop or null
    private GenericPlayerControl itemOwner;

    protected bool destroyObjectOnCollect = true;//this is true for coins and Health. False for Door and Key
    protected bool isObjectMovable = true;//false for the exit door only.
    protected enum isCollectableBy
    {
        playerOneOnly, //weapons
        playerTwoOnly, //exit key
        BothActivePlayers //coins and exit.
    }
    protected isCollectableBy itemCollectionStatus = isCollectableBy.BothActivePlayers;
    protected float itemCollectionDistance = 5;//if item is movable, it will come to activePlayer if player is 5 units away
    protected bool isCollected = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CheckPlayerVicinity()
    {

    }

    public bool ShouldObjectDestroyOnCollect() 
    {
        return destroyObjectOnCollect; 
    }

    public bool IsObjectMovable()
    {
        return isObjectMovable;
    }

}
