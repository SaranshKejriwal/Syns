using System;
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
    isMoving,
    isRunning,
    isInjured,
    isDead,
    isInactive,
    isStanding
}


//This Generic Controller PlayerClass will be the Parent of P1 and P2
//It can be used to apply generic buff items, health setters etc.
public abstract class GenericPlayerController : MonoBehaviour
{
    protected float playerHealth = 10;
    protected float playerMaxHealth = 10;
    protected float goldCollectedByPlayer = 0;

    protected uint currentPlayerLevel = 1; //starting level of Player
    protected float totalPlayerXP = 1f;//initialize at 1f because both players will start at Level 1
    //PlayerOne XP increases based on damage done to enemies, Bonus XP for Boss damage
    //PlayerTwo XP increases based on time spent away from PlayerOne
    //totalPlayerXP indicates the total xp gained by player. playerLevel is the cube root of the totalPlayerXP


    protected bool isActive = false; //true for playerOne and PlayerTwo only. False for Shop and Sack.
    protected bool canBeAttacked = true;//True for Active players, except PlayerTwo while reaching exit.

    protected PlayerType playerType = PlayerType.Generic;
    // Start is called before the first frame update

    protected PlayerState playerState = PlayerState.isInactive;

    protected Vector3 nextIntendedDestination = Vector3.zero;
    //this will be used to dictate the target for PlayerTwo only


    public event EventHandler OnPlayerHealthChange;//this event will be fired when a Player is damaged/healed, for HUD update.

    //this function sets the enemy object for P2 to evade and P1 to attack
    public abstract void SetEnemyInFocus(GenericEnemyController enemy);
    


    void Start()
    {
        //add subscribers to  event
        OnPlayerHealthChange += GameHUDStatsManager.Instance.UpdateHUDOnPlayerHealthChangeEvent;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual Vector3 GetPlayerPosition()
    {
        return transform.position;//this is set as virtual because Generic has no location of its own.
    }

    public virtual void CollectGold(float goldCoinValue)
    {
        goldCollectedByPlayer+= goldCoinValue;//separate counters for PlayerOne and PlayerTwo Instances.
        GameMaster.Instance.AddTotalCollectedGold(goldCoinValue);
        GameHUDStatsManager.Instance.AddToGoldCounterOnHUD(goldCoinValue);
        //Note - A separate Gold Counter makes sense for HUD because we don't want to run Update() method to read the count from GameMaster
    }

    public float GetGoldCollected()
    {
        return goldCollectedByPlayer;
    }

    //This function will increase PlayerXP on different occasions
    public void IncreasePlayerXP(float additionalXP)
    {
        float xpSurpassedAtPresentLevel = (float)Math.Pow((currentPlayerLevel), 3);//Get cube of current Player Level to see XP threshold passed

        totalPlayerXP += additionalXP;

        double cubeRootExponent = 1f / 3f;

        //update level to the round value of cube root of the Total Xp
        currentPlayerLevel = (uint)Math.Pow(totalPlayerXP, cubeRootExponent);//this tactic is used in case Player jumps several levels at once

        float xpNeededToLevelUp = (float)Math.Pow((currentPlayerLevel + 1), 3);//Get cube of next Player Level

        //For HUD, show all values after calibrating xpSurpassedAtPresentLevel at the 0-line
        GameHUDStatsManager.Instance.UpdateHUDPlayerCurrentXPBar(this, totalPlayerXP - xpSurpassedAtPresentLevel, (xpNeededToLevelUp - xpSurpassedAtPresentLevel));
        GameHUDStatsManager.Instance.SetPlayerLevelOnHUD(this, currentPlayerLevel);
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
        if (this.playerHealth <= 0)
        {
            this.playerHealth = 0; //Health cannot be negative.
            KillPlayer();
        }

        Debug.Log(this + " has remaining health: " + this.playerHealth);
        GameHUDStatsManager.Instance.UpdateHUDPlayerHealthBar(this, this.playerHealth, this.playerMaxHealth);

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

        if(this.playerHealth < 0)
        {
            this.playerHealth = 0;//health cannot be negative.
        }
        //heal is not constant. The more damaged the player, the more effective the heal.
        this.playerHealth += healPercent*(this.playerMaxHealth - this.playerHealth);
        Debug.Log("Player Health Healed to - " + this.playerHealth);

        GameHUDStatsManager.Instance.UpdateHUDPlayerHealthBar(this, this.playerHealth, this.playerMaxHealth);    
    }

    //fire health change event to update HUD
    private void FireOnHealthChangeEvent()
    {
        EventArgs healthChangeEventArgs;
        //healthChangeEventArgs.
        if(OnPlayerHealthChange != null)
        {
            OnPlayerHealthChange(this, EventArgs.Empty);
        }
    }



}
