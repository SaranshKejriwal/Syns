using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//This class will be the parent of all collectibles - Gold, Health and ExitKey
//It will define the generic behaviour on player vicinity, and each class will implement an onCollection method
public class GenericCollectibleItem : MonoBehaviour
{
    //this can be PlayerOne, PlayerTwo, Sack, Shop or null
    protected GenericPlayerController itemOwner;

    protected bool destroyObjectOnCollect = true;//this is true for coins and Health. False for Door and Key
    protected bool isObjectMovable = true;//false for the exit door only.
    protected enum isCollectableBy
    {
        playerOneOnly, //weapons
        playerTwoOnly, //exit key
        BothActivePlayers, //coins and exit.
        NoPlayer //Generic parent
    }
    protected isCollectableBy correctCollectingPlayer = isCollectableBy.BothActivePlayers;
    protected float itemDetectionDistance = 5;//if item is movable, it will come to activePlayer if player is 5 units away
    protected float itemCollectionDistance = 1;//if item is movable, it will come to activePlayer if player is 5 units away


    protected bool isObjectCollected = false;
    protected int itemMovementSpeed = 4;//speed at which item moves to player

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //CheckPlayerVicinity();
    }

    protected bool IsActivePlayerInVicinityForCollection(GenericPlayerController player)
    {
        //corner case check.
        if(correctCollectingPlayer == isCollectableBy.NoPlayer || isObjectCollected)
        {
            return false;//don't try collecting Generic object, or an object that is already collected. 
        }
        if (player == null || !player.isActivePlayer())
        {
            return false; //Null or inactive players cannot collect anything.
        }
        if (correctCollectingPlayer == isCollectableBy.playerOneOnly && player == PlayerTwoController.Instance)
        {
            return false; //PlayerTwo cannot collect PlayerOne stuff
        }else if (correctCollectingPlayer == isCollectableBy.playerTwoOnly && player == PlayerOneController.Instance)
        {
            return false;//PlayerOne cannot collect PlayerTwo stuff
        }

        float distanceFromPlayer = Vector3.Distance(transform.position, player.GetPlayerPosition());
        
        if(distanceFromPlayer > itemDetectionDistance)
        {
            return false; //player is not in range yet.
        }

        if (isObjectMovable)//immovable exit door should not come flying :D
        {
            Vector3 collectionDirection = (player.GetPlayerPosition() - transform.position).normalized;
            //move collectible towards player
            transform.position += collectionDirection * Time.deltaTime * itemMovementSpeed;
        }
        else
        {
            //Debug.Log("Player should go to immovable collectible");
            //player should move to position of object. Applies to ExitDoor and PlayerTwo only
            //player.SetNextIntendedDestination(transform.position);
            //Custom handling added for ExitDoor to prevent PlayerTwo from coming from behind Exit door.

        }

        if(distanceFromPlayer <= itemCollectionDistance)
        {
            isObjectCollected = true; //item is now collected. Check if it needs to be destroyed.
            return true;
        }
        return false;
    }

    protected void CheckDestroyOnCollection()
    {
        if(!destroyObjectOnCollect)
        {
            return;//should not destroy object if it is not meant to be destroyed.
        }

        Destroy(this);//fingers crossed.
    }

    public virtual void OnCollectionByPlayer()
    {
        CheckDestroyOnCollection();
    }

    public bool ShouldObjectDestroyOnCollect() 
    {
        return destroyObjectOnCollect; 
    }

    public bool IsObjectMovable()
    {
        return isObjectMovable;
    }

    public bool IsCollected()
    {
        return isObjectCollected;
    }
}
