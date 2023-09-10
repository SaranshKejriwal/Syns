using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This enum can be used to assign a type to any player

public enum PlayerType
{
    PlayerOne, PlayerTwo, 
    GameShop, CommonSack,//these are identified as inactive players only because they can hold buffs.
    Generic //Parent class
}
//This Generic Controller PlayerClass will be the Parent of P1 and P2
//It can be used to apply generic buff items, health setters etc.
public class GenericPlayerController : MonoBehaviour
{
    protected int playerHealth = 10;
    protected int goldCollectedByPlayer = 0;
    protected bool isActive = false; //true for playerOne and PlayerTwo only. False for Shop and Sack.
    protected PlayerType playerType = PlayerType.Generic;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual Vector3 GetPlayerPosition()
    {
        return transform.position;//this is set as virtual because Generic has no location of its own.
    }

    public virtual void CollectGold()
    {
        goldCollectedByPlayer++;//separate counters for PlayerOne and PlayerTwo Instances.
    }

    public int GetGoldCollected()
    {
        return goldCollectedByPlayer;
    }

    //common function to manage Max Player health
    public void SetPlayerMaxHealth(int playerHealth)
    {
        this.playerHealth = playerHealth;
    }

    public bool isActivePlayer()
    {
        return isActive;
    }

    public PlayerType GetPlayerType()
    {
        return playerType;
    }

}
