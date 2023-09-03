using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{

    private Animator enemyAnimator;
    // Start is called before the first frame update
    private const string IS_MOVING_PARAM_NAME = "isEnemyMoving";//create const to avoid string case match errors
    private const string IS_HUNTING_PARAM_NAME = "isEnemyHunting";
    private const string IS_HIT_PARAM_NAME = "isEnemyHit";
    private const string IS_ATTACKING_PARAM_NAME = "isEnemyAttacking";

    [SerializeField] private EnemyControlObject enemyLogicObject; // to reference the logic component of Enemy, in the Prefab itself

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
    }
}
