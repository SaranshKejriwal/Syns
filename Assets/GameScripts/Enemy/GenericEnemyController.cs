using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//This enum controls all possible states of an enemy. Needed for Animator and Behaviours
//instead of having multiple booleans to represent mutually exclusive states, we can use a single Enum
//this way, we won't have to toggle 3 booleans to set one state.
public enum enemyStates
{
    isStanding, //standing in one place. Applies to Boss only.
    isMoving,//moving randomly. Applies to Grunt only
    isHunting,//chasing PlayerTwo. Applies to Grunt only
    isAttacking,//attacking PlayerOne or PlayerTwo. Applies to Grunt and Boss
    isHit, //attacked by PlayerOne. Applies to Grunt and Boss
    isDead //killed by PlayerOne. Applies to Grunt and Boss
}

public enum EnemyType
{
    Grunt,//spawned little enemies that move on screen
    Boss,//single boss of all enemies on the level
    Generic//parent class only.
}

public class GenericEnemyController : MonoBehaviour
{
    protected int enemyWalkingMovementSpeed = 3; //when enemy is walking normally
    protected int enemyHuntingMovementSpeed = 7; //when Player 2 is detected by the Enemy and Enemy is chasing Player 2
    protected int enemyRotationSpeed = 10;
    //Note - Hunting speed should be same as the max movement speed of playerTwo, else enemy will never catch up.
    
    protected Vector3 currentEnemyMovementDirection = Vector3.zero;
    //This will be useful for Grunts. Bosses don't move.

    protected EnemyType enemyType = EnemyType.Generic;

    protected enemyStates currentEnemyState = enemyStates.isStanding;
    protected enemyStates defaultEnemyState = enemyStates.isStanding;//this is used to restore enemy/boss to normal state

    protected float attackRadius = 2.5f;//radius at which enemy can attack playerTwo
    protected float enemyHealth = 25;

    //We have 4 attack types in the free animator. Each can be assigned its own damage value.
    protected float leftClawAttackDamage = 3f;
    protected float rightClawAttackDamage = 3f;
    protected float biteAttackDamage = 5f;
    protected float stompAttackDamage = 8f;

    protected GenericPlayerController targetPlayer = null;//this will be used to track which player is being tracked by the enemy.

    //these dictionaries will be useful for configuring behaviour of each of the enemy types against each of the player types in one base function.
    protected Dictionary<GenericPlayerController, float> enemyDetectionRadiusReference;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public virtual void UpdateEnemyRadii()
    {

    }//abstract forces the child classes to add a child implem
    // Update is called once per frame
    void Update()
    {
        
    }

   
    //this method needs revision. Reaction to PlayerOne is cancelled by subsequent Normal Reaction to far away PlayerTwo.
    protected void ReactToPlayer(GenericPlayerController player)
    {
        if (currentEnemyState == enemyStates.isDead)
        {
            return;//dead enemies can't react to anything.
        }

        if (player == null || !player.CanBeAttacked() || !player.isActivePlayer())
        {            
            return;
            //player should not be null and should be attack-able. Ignore Shop and Bag.
        }

        bool hasPlayerDetectionRadius = enemyDetectionRadiusReference.TryGetValue(player, out float playerDetectionRadius);
        if (!hasPlayerDetectionRadius )
        {
            Debug.LogError(this + "Error - Cannot Get Detection of this enemy");
            return;
        }

        float distanceFromPlayer = Vector3.Distance(player.GetPlayerPosition(), transform.position);
        

        if(distanceFromPlayer <= playerDetectionRadius && distanceFromPlayer > attackRadius)
        {
            //check for obstruction only when player is in radius, not all the time.
            bool isPlayerNotObstructed = IsPlayerRayCastNotObstructed(player, playerDetectionRadius);
            //Grunts will chase. Boss will Roar. This is configured in the animation
            if(isPlayerNotObstructed)
            {               
                HuntPlayer(player);
                return;//return here so that one Player doesn't reset the state of enemy against another player
            }
            else
            {
                //Debug.Log("Obstruction in between. Enemy is ignoring player");
            }

        }else if(distanceFromPlayer <= attackRadius)
        {            
            AttackPlayer(player);
            return;//return here so that one Player doesn't reset the state of enemy against another player
        }
        else
        {
            ContinueDefaultState();//Continue Default state ONLY if none of the 2 players is in vicinity.
        }

    }

    protected void HuntPlayer(GenericPlayerController player)
    {
        //this is used by grunts to chase PlayerTwo
        targetPlayer = player;
        currentEnemyState = enemyStates.isHunting;     

        currentEnemyMovementDirection = AutoMovementHandler.GetDirectionTowardsUnobstructedDestination(player.GetPlayerPosition(), transform.position);
        transform.LookAt(player.GetPlayerPosition());//look at Player

        //set itself as the Enemy in focus for either player;
        player.SetEnemyInFocus(this);


    }

    //this will only play the Enemy animation.
    //There is an Enemy Animation Event which will fire an event for Enemy to cause damage to targetPlayer
    protected void AttackPlayer(GenericPlayerController player)
    {
        //this is used by grunts and bosses to attack both Players
        targetPlayer = player;
        currentEnemyState = enemyStates.isAttacking;
        transform.LookAt(player.GetPlayerPosition());//look at Player
    }

    //if Enemy is far away from Player2 
    protected void ContinueDefaultState()
    {
        targetPlayer = null;
        currentEnemyState = defaultEnemyState;
    }

    public bool IsEnemyMoving()
    {
        return currentEnemyState == enemyStates.isMoving;
    }
    public bool IsEnemyHunting()
    {
        return currentEnemyState == enemyStates.isHunting;
    }
    public bool IsEnemyAttacking()
    {
        return currentEnemyState == enemyStates.isAttacking;
    }
    public bool IsEnemyHit()
    {
        return currentEnemyState == enemyStates.isHit;
    }

    public bool IsEnemyDead()
    {
        return currentEnemyState == enemyStates.isDead;
    }

    public bool isEnemyStanding()
    {
        return currentEnemyState == enemyStates.isStanding;
    }

    public Vector3 GetEnemyPosition()
    {
        return transform.position;
    }

    protected bool IsPlayerRayCastNotObstructed(GenericPlayerController playerInFocus, float detectionRadius)
    {
        //get the direction that enemy needs to take to face the player, regardless of its current movement direction.
        Vector3 directionToPlayer = AutoMovementHandler.GetDirectionTowardsUnobstructedDestination(playerInFocus.GetPlayerPosition(), transform.position);

        //get All the objects that Enemy can see in the direction to Player
        RaycastHit[] visibleObjects = Physics.RaycastAll(transform.position, directionToPlayer, detectionRadius);
        //IMPORTANT - Box Collider is needed on Players for this to work
        //tweak the collider size to be able to correctly ignore players behind walls.

        if (visibleObjects.Length > 0)
        {
            //Component test;
           // bool gotComponent = visibleObjects[0].transform.TryGetComponent(out test);
           // Debug.Log("First Object Visible to Enemy: " + test);//returns the name of the object that was hit.

            //tries to confirm if the first object is Player
            if (visibleObjects[0].transform.TryGetComponent(out MazeWallLengthHandler wall))
            {               
                //Debug.Log("Player is in Radius, but obstructed.");
                return false;
            }

        }
        return true;
    }

    public float GetEnemyDetectionRadiusOfPlayerTwo()
    {
        //Needed to configure the radius of the visible circle.
        float playerTwoDetectionRadius = 0f;
        enemyDetectionRadiusReference.TryGetValue(PlayerTwoController.Instance, out playerTwoDetectionRadius);
        return playerTwoDetectionRadius;
    }

    //This method returns the player between PlayerOne and PlayerTwo that is nearest to the base object
    protected GenericPlayerController GetNearestPlayer()
    {
        float playerOneDistance = Vector3.Distance(transform.position, PlayerOneController.Instance.GetPlayerPosition());
        float playerTwoDistance = Vector3.Distance(transform.position, PlayerTwoController.Instance.GetPlayerPosition());

        return playerTwoDistance <= playerOneDistance ? PlayerTwoController.Instance : PlayerOneController.Instance;
    }


    protected void IncreaseAttackDamageByMultiplier(float multiplier)
    {
        leftClawAttackDamage = multiplier * leftClawAttackDamage;
        rightClawAttackDamage = multiplier * rightClawAttackDamage;
        biteAttackDamage = multiplier * biteAttackDamage;
        stompAttackDamage = multiplier * stompAttackDamage;
    }


    protected int GetEnemyMovementSpeed()
    {
        if (IsEnemyMoving())
        {
            return enemyWalkingMovementSpeed;
        }
        else if (IsEnemyHunting())
        {
            return enemyHuntingMovementSpeed;
        }
        else
        {
            return 0;
        }
    }

    public void HandleStompAnimationCompletionEvent()
    {
        CheckPlayerDistanceAndCauseDamage(stompAttackDamage);
    }

    public void HandleRightClawAnimationCompletionEvent()
    {
        CheckPlayerDistanceAndCauseDamage(rightClawAttackDamage);
    }

    public void HandleLeftClawAnimationCompletionEvent()
    {
        CheckPlayerDistanceAndCauseDamage(leftClawAttackDamage);
    }

    public void HandleBiteAnimationCompletionEvent()
    {
        CheckPlayerDistanceAndCauseDamage(biteAttackDamage);
    }

    private void CheckPlayerDistanceAndCauseDamage(float attackDamage)
    {
        if(currentEnemyState == enemyStates.isDead)
        {
            return;//dead enemy do nothing.
        }

        if(targetPlayer == null)
        {
            return;//This will happen if enemy spawns right on top of player
        }

        if (!targetPlayer.CanBeAttacked())
        {
            return; //don't damage a player that can't be attacked.
        }
         

        //check if Player Dodged the attack.
        float currentPlayerDistance = Vector3.Distance(transform.position, targetPlayer.GetPlayerPosition());
        if (currentPlayerDistance <= attackRadius)
        {
            targetPlayer.DamagePlayer(attackDamage);
        }
        else
        {
            //player moved out of attack range.
            Debug.Log(targetPlayer + " evaded attack.");
        }

        //attack animation is completed.  Enemy is no longer attacking
        //currentEnemyState = enemyStates.isHunting;

    }

    public void RespondToPlayerOnePunch(float punchDamage)
    {
        if(currentEnemyState == enemyStates.isDead)
        {
            return;//dead enemy cannot respond.
        }
        //function is public because it will be called for the Player class interaction handler
        Debug.Log("Player One Punched Enemy. " + this);
        currentEnemyState = enemyStates.isHit;
        DamageEnemy(punchDamage);
    }

    public bool IsTargetingPlayerTwo()
    {
        return targetPlayer == PlayerTwoController.Instance;
    }

    public void DamageEnemy(float attackDamage)
    {
        if(currentEnemyState == enemyStates.isDead) 
        { 
            return; //can't damage a dead enemy.
        }
        this.enemyHealth -= attackDamage;
        Debug.Log(this + " has remaining health: " + this.enemyHealth);
        if (this.enemyHealth <= 0)
        {
            KillEnemy();
        }

    }

    protected void KillEnemy()
    {
        currentEnemyState = enemyStates.isDead;//enemy death animation
        enemyHealth = 0;
        attackRadius = 0;
        IncreaseAttackDamageByMultiplier(0);//all attack damage is zero for dead enemy
        
        //For Grunts only - if enemy dies, reduce the count of active enemies in spawn handler
        if(enemyType == EnemyType.Grunt)
        {
            EnemySpawnHandler.Instance.ReduceAliveEnemyCountOnDeadEnemy();
        }else if (enemyType == EnemyType.Boss)
        {
            //fire an event here which will be listened to by the HUD and the spawn timer.
            EnemyBossController.Instance.FireOnBossDeathEvent();
        }
    }

}
