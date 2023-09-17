using System.Collections;
using System.Collections.Generic;
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

public class GenericEnemyController : MonoBehaviour
{
    protected int enemyWalkingMovementSpeed = 3; //when enemy is walking normally
    protected int enemyHuntingMovementSpeed = 7; //when Player 2 is detected by the Enemy and Enemy is chasing Player 2
    //Note - Hunting speed should be same as the max movement speed of playerTwo, else enemy will never catch up.
    protected enemyStates currentEnemyState = enemyStates.isStanding;

    protected float attackRadius = 1f;//radius at which enemy can attack playerTwo
    protected int enemyHealth = 25;
    protected int attackDamage = 4;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public int GetAttackDamage()
    {
        return attackDamage;
    }
}
