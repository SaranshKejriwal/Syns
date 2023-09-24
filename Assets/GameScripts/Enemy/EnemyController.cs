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

    private void Awake()
    {
        currentEnemyState = enemyStates.isMoving;//Grunt should be moving by default
        defaultEnemyState = enemyStates.isMoving;
        enemyWalkingMovementSpeed = base.enemyWalkingMovementSpeed;
        enemyHuntingMovementSpeed = base.enemyHuntingMovementSpeed;
        enemyHealth = base.enemyHealth;
        IncreaseAttackDamageByMultiplier(1);//same attack damage as base
    }
    public override void UpdateEnemyRadii()
    {
        //this function will update both the radius-lookup dictionaries for each Enemy object.
        enemyDetectionRadiusReference = new Dictionary<GenericPlayerController, float>()
        {
            { PlayerOneController.Instance, 10f },//playerOne is less detectable than playerTwo
            { PlayerTwoController.Instance, 10f }
        };

    }
    void Start()
    {
        this.UpdateEnemyRadii();
        currentEnemyMovementDirection = AutoMovementHandler.GetRandomDirectionVector();
    }

    // Update is called once per frame
    void Update()
    {
        //Get Nearest Player and react to it
        base.ReactToPlayer(base.GetNearestPlayer());
        HandleNormalEnemyMovementWithCollision();

        //for grunt only. If level is completed, Kill all grunts
        CheckDeathOnLevelCompletion();
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

       



    public void RespondToPlayerTwoInteraction()
    {
        Debug.Log("Enemy Responding to Player Two");
        //isEnemyHunting = true;
        //isEnemyMoving = false;//Enemy is now tracking P2, not moving randomly
    }

    private void CheckDeathOnLevelCompletion()
    {
        if (!LevelBuilder.Instance.IsLevelCompleted())
        {
            return;//stay alive as long as level is not completed.
        }
        KillEnemy();
    }


}
