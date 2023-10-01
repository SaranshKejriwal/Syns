using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyAnimation : MonoBehaviour
{

    private Animator enemyAnimator;
    // Start is called before the first frame update
    private const string IS_MOVING_PARAM_NAME = "isEnemyMoving";//create const to avoid string case match errors
    private const string IS_HUNTING_PARAM_NAME = "isEnemyHunting";
    private const string IS_HIT_PARAM_NAME = "isEnemyHit";
    private const string IS_ATTACKING_PARAM_NAME = "isEnemyAttacking";
    private const string IS_STANDING_PARAM_NAME = "isEnemyStanding";
    private const string IS_DEAD_PARAM_NAME = "isEnemyDead";

    //this parameter will be used to play a random attack animation out of the 4 that we have.
    private const string ENEMY_ATTACK_NUMBER_PARAM_NAME = "enemyAttackNumber";
    private const int MIN_INCLUDED_ENEMY_ATTACK_NUMBER = 1;
    private const int MAX_EXCLUDED_ENEMY_ATTACK_NUMBER = 5;//we have 4 attacks in our animator.
    /*
     * Reference - https://assetstore.unity.com/packages/3d/characters/animals/free-stylized-bear-rpg-forest-animal-228910
     * 
     * Attack1 - Right Claw - Value = 1
     * Attack2 - Left Claw - Value = 2
     * Attack3 - Bite - Value = 3
     * Attack5 - Stomp - Value = 4
     */

    [SerializeField] private GenericEnemyController enemyLogicObject; // to reference the logic component of Enemy, in the Prefab itself

    //Awake is called before Start
    private void Awake()//inherited method
    {
        //get the animator reference
        enemyAnimator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {   //read from Enemy logic component if it is moving - refer PlayerOneControl.cs
        enemyAnimator.SetBool(IS_MOVING_PARAM_NAME, enemyLogicObject.IsEnemyMoving());//pickup the "isMoving"  parameter from the Animator component of the player
        enemyAnimator.SetBool(IS_HUNTING_PARAM_NAME, enemyLogicObject.IsEnemyHunting());//pickup the "isMoving"  parameter from the Animator component of the player
        enemyAnimator.SetBool(IS_ATTACKING_PARAM_NAME, enemyLogicObject.IsEnemyAttacking());//pickup the "isMoving"  parameter from the Animator component of the player
        enemyAnimator.SetBool(IS_HIT_PARAM_NAME, enemyLogicObject.IsEnemyHit());
        enemyAnimator.SetBool(IS_STANDING_PARAM_NAME, enemyLogicObject.isEnemyStanding());
        enemyAnimator.SetBool(IS_DEAD_PARAM_NAME, enemyLogicObject.IsEnemyDead());
        
        //this will set a random attack 
        enemyAnimator.SetInteger(ENEMY_ATTACK_NUMBER_PARAM_NAME, MathFunctions.GetRandomIntInRange(MIN_INCLUDED_ENEMY_ATTACK_NUMBER, MAX_EXCLUDED_ENEMY_ATTACK_NUMBER));
    }
    
    //this function is set as the Animation Event in the Enemy Prefabs attack animation. It is only called when Attack animation completes.
    public void EnemyStompAttackAnimationCompletion()
    {
        //this method seems to be called twice, if your Animator is setup improperly. Ensure that transitions from 1 animation to another are simplified.
        //Note - Exit Time is needed on transition after Attack Animation Completion.
        Debug.Log("Enemy Stomp attack completion called only once?");

        enemyLogicObject.HandleStompAnimationCompletionEvent();//this method will be called once when animation ends.
    }

    public void EnemyRightClawAttackAnimationCompletion()
    {
        Debug.Log("Enemy Right Claw attack completion called only once?");
    }
    public void EnemyLeftClawAttackAnimationCompletion()
    {
        Debug.Log("Enemy Left Claw attack completion called only once?");
    }

    public void EnemyBiteAttackAnimationCompletion()
    {
        Debug.Log("Enemy Bite attack completion called only once?");
    }
}
