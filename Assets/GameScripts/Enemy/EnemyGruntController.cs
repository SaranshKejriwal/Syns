using System; //for enemy events.
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;



public class EnemyGruntController : GenericEnemyController
{    
        
    private float enemyInteractionSize = 1.5f; //needed for collision handling in Raycast function.
    //Note - Scale of Detection circle visual in pixels, is 2x this Detection radius - should be controlled by logic.


    private void Awake()
    {
        enemyType = EnemyType.Grunt;
        currentEnemyState = EnemyStates.isMoving;//Grunt should be moving by default
        defaultEnemyState = EnemyStates.isMoving;

        this.EnemyProperties = GenericEnemyController.GetFirstLevelEnemyGruntPropertiesForLevelType(LevelType.Base);
        this.currentEnemyHealth = this.EnemyProperties.maxEnemyHealth;
    }

    void Start()
    {
        currentEnemyMovementDirection = AutoMovementHandler.GetRandomDirectionVector();

        //subscribe to PlayerTwo Exit entry event.
        if (this != null)
        {
            PlayerTwoController.Instance.OnPlayerTwoExit += this.StopGruntMovement;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameMaster.Instance.IsLevelPlaying())
        {
            return;//do nothing if game is paused or level has ended.
        }

        //Get Nearest Player and react to it
        base.ReactToPlayer(base.GetNearestPlayer());
        HandleNormalEnemyMovementWithCollision();

    }

    private void HandleNormalEnemyMovementWithCollision()
    {
        if(GetEnemyMovementSpeed() > 0)
        {
            //needed for collision handling - if player movement is obstructed, try x or z axis movement only
            currentEnemyMovementDirection = AutoMovementHandler.GetMovementReflectionDirectionAfterCollision(currentEnemyMovementDirection, transform.position, enemyInteractionSize);
            //rotate the object to face the updated direction of movement
            transform.forward = Vector3.Slerp(transform.forward, currentEnemyMovementDirection, Time.deltaTime * enemyRotationSpeed);
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
    }


    public void StopGruntMovement(object boss, EventArgs e)
    {
        this.enemyWalkingMovementSpeed = 0;//don't move whwn game is won.
        this.currentEnemyState = EnemyStates.isStanding;
    }


}
