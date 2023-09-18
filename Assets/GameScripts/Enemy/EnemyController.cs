using System; //for enemy events.
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class EnemyController : GenericEnemyController
{    
        
    private float enemyInteractionSize = 1.5f; //needed for collision handling in Raycast function.
    //Note - Scale of Detection circle visual in pixels, is 2x this Detection radius - should be controlled by logic.

    private int rotationSpeed = 10;//needed for Moving Grunts only


    public event EventHandler onHuntingPlayerTwo;//will be listened by PlayerTwo

    private void Awake()
    {
        currentEnemyState = enemyStates.isMoving;//Grunt should be moving by default
        defaultEnemyState = enemyStates.isMoving;
        enemyWalkingMovementSpeed = base.enemyWalkingMovementSpeed;
        enemyHuntingMovementSpeed = base.enemyHuntingMovementSpeed;
        enemyHealth = base.enemyHealth;
        attackDamage = base.attackDamage;
    }

    void Start()
    {
        this.UpdateEnemyRadii();
        currentEnemyMovementDirection = AutoMovementHandler.GetRandomDirectionVector();
    }

    // Update is called once per frame
    void Update()
    {
        base.ReactToPlayer(PlayerOneController.Instance);
        base.ReactToPlayer(PlayerTwoController.Instance);
        HandleNormalEnemyMovementWithCollision();
    }

    public override void UpdateEnemyRadii()
    {
        //this function will update both the radius-lookup dictionaries for each Enemy object.
        enemyDetectionRadiusReference = new Dictionary<GenericPlayerController, float>()
        {
            { PlayerOneController.Instance, 2f },
            { PlayerTwoController.Instance, 10f }
        };

    }


    private void HandleNormalEnemyMovementWithCollision()
    {

        //needed for collision handling - if player movement is obstructed, try x or z axis movement only
        currentEnemyMovementDirection = AutoMovementHandler.GetMovementReflectionDirectionAfterCollision(currentEnemyMovementDirection, transform.position, enemyInteractionSize);
        //rotate the object to face the updated direction of movement
        transform.forward = Vector3.Slerp(transform.forward, currentEnemyMovementDirection, Time.deltaTime * rotationSpeed);
        /*
         * Tip - use transform.lookAt function to have object change line of sight to a point. Useful for enemies facing P2
            transform.up or transform.right can work for 2D games to change direction.

            Slerp() function makes the direction change from prev pos smoother, by adding smoothing to not make the direction change instantaneous.
         */

        //move the object position in the direction 
        transform.position += currentEnemyMovementDirection * Time.deltaTime * GetEnemyMovementSpeed();
        /*transform.position is a 3D vector.
        Time.deltaTime ensures that perceived change in position is independent of system framerate.
        Time.deltaTime returns the timelapse between 2 frames. Very small number.*/
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


    //Needed to configure the radius of the visible circle.
    public float GetEnemyDetectionRadiusOfPlayerTwo()
    {
        float playerTwoDetectionRadius = 0f;
        enemyDetectionRadiusReference.TryGetValue(PlayerTwoController.Instance, out playerTwoDetectionRadius);
        return playerTwoDetectionRadius;
    }
}
