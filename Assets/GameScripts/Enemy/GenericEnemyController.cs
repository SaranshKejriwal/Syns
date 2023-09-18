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

public abstract class GenericEnemyController : MonoBehaviour
{
    protected int enemyWalkingMovementSpeed = 3; //when enemy is walking normally
    protected int enemyHuntingMovementSpeed = 7; //when Player 2 is detected by the Enemy and Enemy is chasing Player 2
    //Note - Hunting speed should be same as the max movement speed of playerTwo, else enemy will never catch up.
    
    protected Vector3 currentEnemyMovementDirection = Vector3.zero;
    //This will be useful for Grunts. Bosses don't move.
    
    protected enemyStates currentEnemyState = enemyStates.isStanding;
    protected enemyStates defaultEnemyState = enemyStates.isStanding;//this is used to restore enemy/boss to normal state

    protected float attackRadius = 1.5f;//radius at which enemy can attack playerTwo
    protected int enemyHealth = 25;
    protected int attackDamage = 4;


    //these dictionaries will be useful for configuring behaviour of each of the enemy types against each of the player types in one base function.
    protected Dictionary<GenericPlayerController, float> enemyDetectionRadiusReference;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public abstract void UpdateEnemyRadii();//abstract forces the child classes to add a child implementation.

    // Update is called once per frame
    void Update()
    {
        
    }

   
    //this method needs revision. Reaction to PlayerOne is cancelled by subsequent Normal Reaction to far away PlayerTwo.
    protected void ReactToPlayer(GenericPlayerController player)
    {
        if(player == null || !player.CanBeAttacked() || !player.isActivePlayer())
        {            
            return;
            //player should not be null and should be attack-able. Ignore Shop and Bag.
        }

        float playerDetectionRadius = 0;
        bool hasPlayerDetectionRadius = enemyDetectionRadiusReference.TryGetValue(player, out playerDetectionRadius);
        if(!hasPlayerDetectionRadius )
        {
            Debug.LogError("Error - Cannot Get Detection of this enemy");
            return;
        }

        float distanceFromPlayer = Vector3.Distance(player.GetPlayerPosition(), transform.position);

        if(distanceFromPlayer > playerDetectionRadius)
        {
            ContinueNormalState();
            return;
            //player is beyond range. Cannot do anything further
        }else if(distanceFromPlayer > attackRadius)
        {
            //Bosses cannot move/hunt, so they have to ignore HuntPlayer() call.
            HuntPlayer(player);

        }else if(distanceFromPlayer <= attackRadius)
        {
            AttackPlayer(player);
        }


    }

    protected void HuntPlayer(GenericPlayerController player)
    {
        //this is used by grunts to chase PlayerTwo
        currentEnemyState = enemyStates.isHunting;

        player.RespondToEnemyHunt(transform.position);

        currentEnemyMovementDirection = AutoMovementHandler.GetDirectionTowardsUnobstructedDestination(PlayerTwoController.Instance.GetPlayerPosition(), transform.position);
        transform.LookAt(player.GetPlayerPosition());//look at PlayerTwo

        //fire an event here, which would prompt PlayerTwo to run away from enemy position.
        //onHuntingPlayerTwo += PlayerTwoControl.Instance.RespondToEnemyHunt;
        //PlayerTwoController.Instance.EvadeEnemyPosition(transform.position);

    }

    protected void AttackPlayer(GenericPlayerController player)
    {
        //this is used by grunts and bosses to attack both Players
        currentEnemyState = enemyStates.isAttacking;
        Debug.Log("Player in attack vicinity");

        player.RespondToEnemyAttack(transform.position);
    }

    //if Enemy is far away from Player2 
    protected void ContinueNormalState()
    {
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

    public int GetAttackDamage()
    {
        return attackDamage;
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
        else if (IsEnemyAttacking() || IsEnemyDead() || IsEnemyHit() || isEnemyStanding())
        {
            return 0;
        }
        return enemyWalkingMovementSpeed;
    }
}
