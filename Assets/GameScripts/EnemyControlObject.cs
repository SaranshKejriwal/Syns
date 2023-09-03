using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControlObject : MonoBehaviour
{
    // Start is called before the first frame update

    private int enemyWalkingMovementSpeed = 3; //when enemy is walking normally
    private int enemyHuntingMovementSpeed = 7; //when Player 2 is detected by the Enemy and Enemy is chasing Player 2
    //Note - Hunting speed should be same as the max movement speed of playerTwo, else enemy will never catch up.

    [SerializeField] private float enemyInteractionSize = 1.5f; //needed for collision handling in Raycast function.
    [SerializeField] private float playerTwoDetectionDistance = 5;//distance under which Enemy will start hunting Player Two


    private int rotationSpeed = 10;
    private bool isEnemyMoving = true; //used by animator to render movement animation if enemy is moving normally
    private bool isEnemyHunting = false;//used by animator to render running animation if enemy is hunting playerTwo
    //Enemy will either move or hunt, not both.
    private bool isEnemyAttacking = false;//used by animator to render attacking animation if enemy is attacking playerTwo
    private bool isEnemyHit = false;

    private Vector3 currentEnemyDirectionVector = Vector3.zero;

    void Start()
    {
        currentEnemyDirectionVector = AutoMovementHandler.GetRandomDirectionVector();
    }

    // Update is called once per frame
    void Update()
    {
        ResetEnemyInteractionAnimators();//Reset all animator booleans on Enemy
        HandleNormalEnemyMovementWithCollision();
        CheckPlayerTwoVicinity();
    }


    private void HandleNormalEnemyMovementWithCollision()
    {

        //this will always be true for PlayerTwo since currentPlayerTwoDirection cannot be zero 
        isEnemyMoving = currentEnemyDirectionVector != Vector3.zero;
        isEnemyHunting = !isEnemyMoving;//enemy can either move or hunt.

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

    private void CheckPlayerTwoVicinity()
    {
        //RespondToPlayerTwoInteraction();//this has to be a conditional call when Enemy is in Proximity of P2
        Debug.Log("Enemy object identity: " + this);
        Debug.Log("Player 2 Location: " + PlayerTwoControl.Instance.GetPlayerTwoLocation());
    }

    private int GetEnemyMovementSpeed()
    {
        if (isEnemyMoving && !isEnemyHunting)
        {
            return enemyWalkingMovementSpeed;
        }
        else if(isEnemyHunting && !isEnemyMoving)
        {
            return enemyHuntingMovementSpeed;
        }
        else if(isEnemyMoving && isEnemyHunting)
        {
            Debug.Log("Error: Enemy cannot be moving and hunting simultaneously. Defaulting to Walk speed");
            return enemyWalkingMovementSpeed;
        }
        return enemyWalkingMovementSpeed;
    }


    public void RespondToPlayerOnePunch()
    {
        //function is public because it will be called for the Player class interaction handler
        Debug.Log("Player One Punched Enemy.");
        isEnemyHit = true;
    }

    public void RespondToPlayerTwoInteraction()
    {
        Debug.Log("Enemy spotted Player Two");
        isEnemyHunting = true;
        isEnemyMoving = false;//Enemy is now tracking P2, not moving randomly
    }

    private void ResetEnemyInteractionAnimators()
    {
        //isEnemyHit = false;//hit is the only interaction animator.
    }

    public bool IsEnemyMoving()
    {
        return isEnemyMoving;
    }
    public bool IsEnemyHunting()
    {
        return isEnemyHunting;
    }
    public bool IsEnemyAttacking()
    {
        return isEnemyAttacking;
    }
    public bool IsEnemyHit()
    {
        return isEnemyHit;
    }
}
