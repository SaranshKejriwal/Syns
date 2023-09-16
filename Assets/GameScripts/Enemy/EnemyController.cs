using System; //for enemy events.
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

enum enemyStates 
{
    isMoving,//moving randomly
    isHunting,//chasing PlayerTwo
    isAttacking,//attacking PlayerOne or PlayerTwo
    isHit, //attacked by PlayerOne
    isDead //killed by PlayerOne
}

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update

    private int enemyWalkingMovementSpeed = 3; //when enemy is walking normally
    private int enemyHuntingMovementSpeed = 7; //when Player 2 is detected by the Enemy and Enemy is chasing Player 2
    //Note - Hunting speed should be same as the max movement speed of playerTwo, else enemy will never catch up.

    [SerializeField] private float enemyInteractionSize = 1.5f; //needed for collision handling in Raycast function.
    //Note - Scale of Detection circle visual is 2x this Detection radius - should be controlled by logic.

    private int rotationSpeed = 10;

    //instead of having multiple booleans to represent mutually exclusive states, we can use a single Enum
    //this way, we won't have to toggle 3 booleans to set one state.
    private enemyStates currentEnemyState = enemyStates.isMoving;

    private Vector3 currentEnemyDirectionVector = Vector3.zero;

    //PlayerTwo interaction Objects
    [SerializeField] private float enemyDetectionRadiusOfPlayerTwo = 10f;//distance under which Enemy will start hunting Player Two
    private float currentDistanceFromPlayerTwo = 1000f;//initialize distance from player2 at very high value
    private float enemyAttackRadius = 1f;//radius at which enemy can attack playerTwo

    public event EventHandler onHuntingPlayerTwo;//will be listened by PlayerTwo

    void Start()
    {
        currentEnemyDirectionVector = AutoMovementHandler.GetRandomDirectionVector();
    }

    // Update is called once per frame
    void Update()
    {
        HandleNormalEnemyMovementWithCollision();
        //ReactToPlayerTwo();
    }


    private void HandleNormalEnemyMovementWithCollision()
    {

        //needed for collision handling - if player movement is obstructed, try x or z axis movement only
        currentEnemyDirectionVector = AutoMovementHandler.GetMovementReflectionDirectionAfterCollision(currentEnemyDirectionVector, transform.position, enemyInteractionSize);
        //rotate the object to face the updated direction of movement
        transform.forward = Vector3.Slerp(transform.forward, currentEnemyDirectionVector, Time.deltaTime * rotationSpeed);
        /*
         * Tip - use transform.lookAt function to have object change line of sight to a point. Useful for enemies facing P2
            transform.up or transform.right can work for 2D games to change direction.

            Slerp() function makes the direction change from prev pos smoother, by adding smoothing to not make the direction change instantaneous.
         */

        //move the object position in the direction 
        transform.position += currentEnemyDirectionVector * Time.deltaTime * GetEnemyMovementSpeed();
        /*transform holds the position of the GameObj, apparently
        transform.position is a 3D vector.
        Time.deltaTime ensures that perceived change in position is independent of system framerate.
        Time.deltaTime returns the timelapse between 2 frames. Very small number.*/
    }


    private void ReactToPlayerTwo()
    {
        if(!PlayerTwoController.Instance.CanBeAttacked())
        {
            return;//Enemy need not react to PlayerTwo if PlayerTwo cannot be attacked after reaching exit.
        }
        //Get current distance from Player 2
        currentDistanceFromPlayerTwo = Vector3.Distance(transform.position, PlayerTwoController.Instance.GetPlayerPosition());
        if (currentDistanceFromPlayerTwo > enemyDetectionRadiusOfPlayerTwo)
        {
            //current distance is farther than enemy detection radius. Enemy to continue normal motion and not attack Player 2.
            ContinueNormalMotion();
            return;
        }else if (currentDistanceFromPlayerTwo > enemyAttackRadius)
        {
            HuntPlayerTwo();
        }else if(currentDistanceFromPlayerTwo <= enemyAttackRadius)
        {
            //Enemy within reach of Player Two
            AttackPlayerTwo();
        }

        //if distance is close, change current Enemy direction to chase player 2


    }

    private int GetEnemyMovementSpeed()
    {
        if (IsEnemyMoving())
        {
            return enemyWalkingMovementSpeed;
        }
        else if(IsEnemyHunting())
        {
            return enemyHuntingMovementSpeed;
        }
        return enemyWalkingMovementSpeed;
    }


    public void RespondToPlayerOnePunch()
    {
        //function is public because it will be called for the Player class interaction handler
        Debug.Log("Player One Punched Enemy.");
        currentEnemyState = enemyStates.isHit;
    }

    public void RespondToPlayerTwoInteraction()
    {
        Debug.Log("Enemy Responding to Player Two");
        //isEnemyHunting = true;
        //isEnemyMoving = false;//Enemy is now tracking P2, not moving randomly
    }

    private void ResetEnemyInteractionAnimators()
    {
        //isEnemyHit = false;//hit is the only interaction animator.
    }

    //if Enemy is far away from Player2 
    private void ContinueNormalMotion()
    {
        currentEnemyState = enemyStates.isMoving;
    }

    private void HuntPlayerTwo()
    {
        //Note - Buff animation can be added before running.
        
        //start running in the direction of PlayerTwo
        currentEnemyState = enemyStates.isHunting;

        //get enemy to move towards PlayerTwo, based on positions of both.
        currentEnemyDirectionVector = AutoMovementHandler.GetDirectionTowardsUnobstructedDestination(PlayerTwoController.Instance.GetPlayerPosition(), transform.position);
        transform.LookAt(PlayerTwoController.Instance.GetPlayerPosition());//look at PlayerTwo

        //fire an event here, which would prompt PlayerTwo to run away from enemy position.
        //onHuntingPlayerTwo += PlayerTwoControl.Instance.RespondToEnemyHunt;
        PlayerTwoController.Instance.EvadeEnemyPosition(transform.position);


    }

    private void AttackPlayerTwo()
    {
        currentEnemyState = enemyStates.isAttacking;
        //does attack need to be stationary?
        Debug.Log("PlayerTwo in attack vicinity");
        //fire an event here, which would impact PlayerTwo Health
    }

    private void AttackPlayerOne()
    {
        currentEnemyState = enemyStates.isAttacking;
        //Only if PlayerOne attacks enemy, enemy should Attack Player One.
        //fire an event here, which would impact PlayerOne Health
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

    //Needed to configure the radius of the visible circle.
    public float GetEnemyDetectionRadiusOfPlayerTwo()
    {
        return enemyDetectionRadiusOfPlayerTwo;
    }

    public void SetEnemyDetectionRadiusOfPlayerTwo(float NewDetectionRadius)
    {
        enemyDetectionRadiusOfPlayerTwo = NewDetectionRadius;
    }
}
