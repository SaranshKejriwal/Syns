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
        public Runes currentPlayerRunes = Runes.None;//this will track which runes are tagged to the player.

        //PlayerOne XP increases based on damage done to enemies, Bonus XP for Boss damage 
        //PlayerTwo XP increases based on time spent away from PlayerOne
        //totalPlayerXP indicates the total xp gained by player. playerLevel is the cube root of the totalPlayerXP

        public float playerOnePunchAttackDamage = 30f;//increased to instakill for test only.

    }
    //this object will be saved into Progress Manager and into the final json.
    protected GenericPlayerControllerProperties playerControllerProperties = new GenericPlayerController.GenericPlayerControllerProperties();
    
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
        bool playerHasGreenRune = this.playerControllerProperties.currentPlayerRunes.HasFlag(Runes.GreedRune_Glimmer);
        //if player has the greed rune, increase gold coin value.
        float goldCoinValueWithRune = goldCoinValue * RuneEffectManager.Instance.GetGreedRuneEffect(playerHasGreenRune);
        goldCollectedByPlayerInLevel+= goldCoinValueWithRune;//separate counters for PlayerOne and PlayerTwo Instances.
        
        //Note - this means that if level is failed, coins collected are still added.
        GameProgressManager.Instance.AddTotalCollectedGold(goldCoinValueWithRune);

        LevelHUDStatsManager.Instance.AddToGoldCounterOnHUD(goldCoinValueWithRune);
        //Note - A separate Gold Counter makes sense for HUD because we don't want to run Update() method to read the count from GameMaster
    }

    public float GetGoldCollected()
    {
        return goldCollectedByPlayerInLevel;
    }

    //This function will increase PlayerXP on different occasions
    public void IncreasePlayerXP(float additionalXP)
    {
        float xpSurpassedAtPresentLevel = (float)Math.Pow((playerControllerProperties.currentPlayerLevel), 3);//Get cube of current Player Level to see XP threshold passed

        playerControllerProperties.totalPlayerXP += additionalXP;

        double cubeRootExponent = 1f / 3f;

        //update level to the round value of cube root of the Total Xp
        playerControllerProperties.currentPlayerLevel = (uint)Math.Pow(playerControllerProperties.totalPlayerXP, cubeRootExponent);//this tactic is used in case Player jumps several levels at once

        float xpNeededToLevelUp = (float)Math.Pow((playerControllerProperties.currentPlayerLevel + 1), 3);//Get cube of next Player Level

        //For HUD, show all values after calibrating xpSurpassedAtPresentLevel at the 0-line
        LevelHUDStatsManager.Instance.UpdateHUDPlayerCurrentXPBar(this, playerControllerProperties.totalPlayerXP - xpSurpassedAtPresentLevel, (xpNeededToLevelUp - xpSurpassedAtPresentLevel));
        LevelHUDStatsManager.Instance.SetPlayerLevelOnHUD(this, playerControllerProperties.currentPlayerLevel);
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
        return playerControllerProperties;
    }

    public void SetPlayerPropertiesFromSave(GenericPlayerControllerProperties newProperties)
    {

        this.playerControllerProperties = newProperties;
        //Set each field individually rather than just as an object, to be absolutely sure
        this.playerControllerProperties.maxPlayerHealth = newProperties.maxPlayerHealth;
        this.playerControllerProperties.maxPlayerMovementSpeed = newProperties.maxPlayerMovementSpeed;
        this.playerControllerProperties.currentPlayerArmour = newProperties.currentPlayerArmour;
        this.playerControllerProperties.currentPlayerLevel = newProperties.currentPlayerLevel;
        this.playerControllerProperties.totalPlayerXP = newProperties.totalPlayerXP;

    }

    public void DamagePlayer(float attackDamage)
    {
        //check if player's sloth rune has returned true.
        bool playerHasSlothRune = this.playerControllerProperties.currentPlayerRunes.HasFlag(Runes.SlothRune_Haste);
        if (playerHasSlothRune && RuneEffectManager.Instance.GetSlothRuneEffect(playerHasSlothRune))
        {
            return;//ignore enemy damage altogether
        }

        //reduce damage based on Armour of player - Max Damage reduction capped at 50%, hence 2*
        this.currentPlayerHealth -= attackDamage * (1 - (playerControllerProperties.currentPlayerArmour/(2*maxPossiblePlayerArmour)));

        if (this.currentPlayerHealth <= 0)
        {
            this.currentPlayerHealth = 0; //Health cannot be negative.
            KillPlayer();
        }

        Debug.Log(this + " has remaining health: " + this.currentPlayerHealth);
        LevelHUDStatsManager.Instance.UpdateHUDPlayerHealthBar(this, this.currentPlayerHealth, this.playerControllerProperties.maxPlayerHealth);
    }

    protected void ApplyGluttonyRuneEffect()
    {
        bool playerHasGluttonyRune = this.playerControllerProperties.currentPlayerRunes.HasFlag(Runes.GluttonyRune_Resolve);
        if (!playerHasGluttonyRune)
        {
            return;
        }
        else
        {
            this.currentPlayerHealth += RuneEffectManager.Instance.GetGluttonyRuneEffect(playerHasGluttonyRune, this.playerControllerProperties.maxPlayerHealth);
        }
    }

    protected virtual void KillPlayer()
    {
        Debug.Log(this + " is dead.");
        this.playerState = PlayerState.isDead;
    }

    public bool IsAtMaxHealth()
    {
        return this.currentPlayerHealth == this.playerControllerProperties.maxPlayerHealth;
    }

    public void HealPlayer(float healPercent)
    {
        if(this.currentPlayerHealth >= this.playerControllerProperties.maxPlayerHealth)
        {
            return; //can't allow a condition where playerHealth exceeds MaxHealth.
        }

        if(this.currentPlayerHealth < 0)
        {
            this.currentPlayerHealth = 0;//health cannot be negative.
        }
        //heal is not constant. The more damaged the player, the more effective the heal.
        this.currentPlayerHealth += healPercent*(this.playerControllerProperties.maxPlayerHealth - this.currentPlayerHealth);
        Debug.Log("Player Health Healed to - " + this.currentPlayerHealth);

        LevelHUDStatsManager.Instance.UpdateHUDPlayerHealthBar(this, this.currentPlayerHealth, this.playerControllerProperties.maxPlayerHealth);    
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
