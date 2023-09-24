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

public enum PlayerState
{
    isActiveNormal,
    isRunning,
    isInjured,
    isDead,
    isInactive
}


//This Generic Controller PlayerClass will be the Parent of P1 and P2
//It can be used to apply generic buff items, health setters etc.
public abstract class GenericPlayerController : MonoBehaviour
{
    protected float playerHealth = 10;
    protected float playerMaxHealth = 10;
    protected int goldCollectedByPlayer = 0;
    protected bool isActive = false; //true for playerOne and PlayerTwo only. False for Shop and Sack.
    protected bool canBeAttacked = true;//True for Active players, except PlayerTwo while reaching exit.

    protected PlayerType playerType = PlayerType.Generic;
    // Start is called before the first frame update

    protected PlayerState playerState = PlayerState.isInactive;

    protected Vector3 nextIntendedDestination = Vector3.zero;
    //this will be used to dictate the target for PlayerTwo only

    public abstract void RespondToEnemyHunt(Vector3 enemyPosition);
    public abstract void RespondToEnemyAttack(Vector3 enemyPosition);

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


    public bool isActivePlayer()
    {
        return isActive;
    }
    public bool CanBeAttacked()
    {
        return canBeAttacked;
    }

    public PlayerType GetPlayerType()
    {
        return playerType;
    }

    public void SetNextIntendedDestination(Vector3 destination)
    {
        nextIntendedDestination = destination;
    }

    public void DamagePlayer(float attackDamage)
    {
        this.playerHealth -= attackDamage;
        Debug.Log(this + " has remaining health: "+this.playerHealth);
        if (this.playerHealth <= 0)
        {
            KillPlayer();
        }

    }

    protected virtual void KillPlayer()
    {
        Debug.Log(this + " is dead.");
        this.playerState = PlayerState.isDead;
    }

    public void SetPlayerMaxHealth(int playerHealth)
    {
        this.playerMaxHealth = playerHealth;
    }

    public bool IsAtMaxHealth()
    {
        return this.playerHealth == this.playerMaxHealth;
    }

    public void HealPlayer(float healPercent)
    {
        if(this.playerHealth >= this.playerMaxHealth)
        {
            return; //can't allow a condition where playerHealth exceeds MaxHealth.
        }
        //heal is not constant. The more damaged the player, the more effective the heal.
        this.playerHealth += healPercent*(this.playerMaxHealth - this.playerHealth);
    }

}
