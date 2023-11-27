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
    //defensive properties
    protected float currentPlayerHealth = 10; //this will be reset at level start anyway. 
    protected const float maxPossiblePlayerArmour = 10;//this is to reduce the enemy damage by a %. MaxPossible armour acts as a demominator.


    protected float goldCollectedByPlayerInLevel = 0;//this need not be saved into Save-File


    protected bool isActive = false; //true for playerOne and PlayerTwo only. False for Shop and Sack.
    protected bool canBeAttacked = true;//True for Active players, except PlayerTwo while reaching exit.

    protected PlayerType playerType = PlayerType.Generic;
    // Start is called before the first frame update

    protected PlayerState playerState = PlayerState.isInactive;

    public event EventHandler OnPlayerHealthChange;//this event will be fired when a Player is damaged/healed, for HUD update.

    //this function sets the enemy object for P2 to evade and P1 to attack
    public abstract void SetEnemyInFocus(GenericEnemyController enemy);

    //this class will be used to define the properties that will be saved by the GameProgressManager in the JSON

    [Serializable]//Serializable Keyword is necessary for JsonUtility to include this object.
    public class GenericPlayerControllerProperties
    {
        public float maxPlayerHealth = 10;
        public int maxPlayerMovementSpeed = 8;//it is assumed that PlayerOne will always move at Max speed.
        public float currentPlayerArmour = 0f;
        public uint currentPlayerLevel = 1; //starting level of Player
        public float totalPlayerXP = 1f;//initialize at 1f because both players will start at Level 1

        //PlayerOne XP increases based on damage done to enemies, Bonus XP for Boss damage 
        //PlayerTwo XP increases based on time spent away from PlayerOne
        //totalPlayerXP indicates the total xp gained by player. playerLevel is the cube root of the totalPlayerXP

        public float playerOnePunchAttackDamage = 30f;//increased to instakill for test only.

        public void setAllPlayerProperties()//as good as a constructor.
        {

        }
    }
    //this object will be saved into Progress Manager and into the final json.
    protected GenericPlayerControllerProperties PlayerControllerProperties = new GenericPlayerController.GenericPlayerControllerProperties();
    
    void Start()
    {
        //add subscribers to  event
        OnPlayerHealthChange += LevelHUDStatsManager.Instance.UpdateHUDOnPlayerHealthChangeEvent;
    }

    public virtual Vector3 GetPlayerPosition()
    {
        return transform.position;//this is set as virtual because Generic has no location of its own.
    }

    public virtual void CollectGold(float goldCoinValue)
    {
        goldCollectedByPlayerInLevel+= goldCoinValue;//separate counters for PlayerOne and PlayerTwo Instances.
        GameMaster.Instance.AddTotalCollectedGold(goldCoinValue);
        LevelHUDStatsManager.Instance.AddToGoldCounterOnHUD(goldCoinValue);
        //Note - A separate Gold Counter makes sense for HUD because we don't want to run Update() method to read the count from GameMaster
    }

    public float GetGoldCollected()
    {
        return goldCollectedByPlayerInLevel;
    }

    //This function will increase PlayerXP on different occasions
    public void IncreasePlayerXP(float additionalXP)
    {
        float xpSurpassedAtPresentLevel = (float)Math.Pow((PlayerControllerProperties.currentPlayerLevel), 3);//Get cube of current Player Level to see XP threshold passed

        PlayerControllerProperties.totalPlayerXP += additionalXP;

        double cubeRootExponent = 1f / 3f;

        //update level to the round value of cube root of the Total Xp
        PlayerControllerProperties.currentPlayerLevel = (uint)Math.Pow(PlayerControllerProperties.totalPlayerXP, cubeRootExponent);//this tactic is used in case Player jumps several levels at once

        float xpNeededToLevelUp = (float)Math.Pow((PlayerControllerProperties.currentPlayerLevel + 1), 3);//Get cube of next Player Level

        //For HUD, show all values after calibrating xpSurpassedAtPresentLevel at the 0-line
        LevelHUDStatsManager.Instance.UpdateHUDPlayerCurrentXPBar(this, PlayerControllerProperties.totalPlayerXP - xpSurpassedAtPresentLevel, (xpNeededToLevelUp - xpSurpassedAtPresentLevel));
        LevelHUDStatsManager.Instance.SetPlayerLevelOnHUD(this, PlayerControllerProperties.currentPlayerLevel);
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

    public GenericPlayerControllerProperties GetPlayerControllerProperties()
    {
        return PlayerControllerProperties;
    }

    public void SetPlayerPropertiesFromSave(GenericPlayerControllerProperties newProperties)
    {

        this.PlayerControllerProperties = newProperties;
        //Set each field individually rather than just as an object, to be absolutely sure
        this.PlayerControllerProperties.maxPlayerHealth = newProperties.maxPlayerHealth;
        this.PlayerControllerProperties.maxPlayerMovementSpeed = newProperties.maxPlayerMovementSpeed;
        this.PlayerControllerProperties.currentPlayerArmour = newProperties.currentPlayerArmour;
        this.PlayerControllerProperties.currentPlayerLevel = newProperties.currentPlayerLevel;
        this.PlayerControllerProperties.totalPlayerXP = newProperties.totalPlayerXP;

    }

    public void DamagePlayer(float attackDamage)
    {
        //reduce damage based on Armour of player - Max Damage reduction capped at 50%, hence 2*
        this.currentPlayerHealth -= attackDamage * (1 - (PlayerControllerProperties.currentPlayerArmour/(2*maxPossiblePlayerArmour)));

        if (this.currentPlayerHealth <= 0)
        {
            this.currentPlayerHealth = 0; //Health cannot be negative.
            KillPlayer();
        }

        Debug.Log(this + " has remaining health: " + this.currentPlayerHealth);
        LevelHUDStatsManager.Instance.UpdateHUDPlayerHealthBar(this, this.currentPlayerHealth, this.PlayerControllerProperties.maxPlayerHealth);

    }

    protected virtual void KillPlayer()
    {
        Debug.Log(this + " is dead.");
        this.playerState = PlayerState.isDead;
    }

    public bool IsAtMaxHealth()
    {
        return this.currentPlayerHealth == this.PlayerControllerProperties.maxPlayerHealth;
    }

    public void HealPlayer(float healPercent)
    {
        if(this.currentPlayerHealth >= this.PlayerControllerProperties.maxPlayerHealth)
        {
            return; //can't allow a condition where playerHealth exceeds MaxHealth.
        }

        if(this.currentPlayerHealth < 0)
        {
            this.currentPlayerHealth = 0;//health cannot be negative.
        }
        //heal is not constant. The more damaged the player, the more effective the heal.
        this.currentPlayerHealth += healPercent*(this.PlayerControllerProperties.maxPlayerHealth - this.currentPlayerHealth);
        Debug.Log("Player Health Healed to - " + this.currentPlayerHealth);

        LevelHUDStatsManager.Instance.UpdateHUDPlayerHealthBar(this, this.currentPlayerHealth, this.PlayerControllerProperties.maxPlayerHealth);    
    }

    //fire health change event to update HUD
    /*private void FireOnHealthChangeEvent()
    {
        EventArgs healthChangeEventArgs;
        //healthChangeEventArgs.
        if(OnPlayerHealthChange != null)
        {
            OnPlayerHealthChange(this, EventArgs.Empty);
        }
    }*/



}
