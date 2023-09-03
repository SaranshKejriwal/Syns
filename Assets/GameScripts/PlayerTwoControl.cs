using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerTwoControl : MonoBehaviour
{

    [SerializeField] private int currentPlayerTwoMovementSpeed = 3;//this private field is accessible on Inspector only, not anywhere else outside class
    [SerializeField] private int maxPlayerTwoMovementSpeed = 7;
    [SerializeField] private int minPlayerTwoMovementSpeed = 1;

    private int rotationSpeed = 10;
    private bool isMoving = true; //used by animator to render movement animation if player is moving
    private bool isEvadingEnemy = false; //can be used in future to create smart reaction to enemy

    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private float playerTwoInteractionSize = 0.5f; //needed for collision handling in Raycast function.
    //private int playerHeightOffset = 2;//needed for collision handling in CapsuleCast function.

    private float playerTwoInteractionDistance = 2f;
    
    private Vector3 currentPlayerTwoDirectionVector = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        //choose a random starting direction to start moving
        currentPlayerTwoDirectionVector = AutoMovementHandler.GetRandomDirectionVector();
    }

    // Update is called once per frame
    private void Update()
    {   //Update() is inherited from MonoBehaviour. Called on each frame. Always specify the access modifier
        
        //Update speed if Faster or Slower key binding is pressed.
        currentPlayerTwoMovementSpeed = inputHandler.GetCurrentPlayerTwoMovementSpeed(currentPlayerTwoMovementSpeed, maxPlayerTwoMovementSpeed, minPlayerTwoMovementSpeed);

        HandleMovementWithCollision();
        HandleAllInteractions();
        //HandleAttackAction();
    }


    private void HandleAllInteractions()
    {
        
        if (Physics.Raycast(transform.position, currentPlayerTwoDirectionVector, out RaycastHit rayCastHit, playerTwoInteractionDistance))
        //tells us if something is in front and returns its object parameter in rayCastHit object
        {
            //Debug.Log(rayCastHit.transform);//returns the name of the object that was hit.

            //tries to confirm if interacted component is of a specific type.
            if (rayCastHit.transform.TryGetComponent(out EnemyControlObject approachingEnemy))
            {
                Debug.Log(approachingEnemy);
                approachingEnemy.RespondToPlayerTwoInteraction();
            }


        }

    }


    private void HandleMovementWithCollision()
    {
        
        //this will always be true for PlayerTwo since currentPlayerTwoDirection cannot be zero 
        isMoving = currentPlayerTwoDirectionVector != Vector3.zero;

        //needed for collision handling - if player movement is obstructed, try x or z axis movement only
        currentPlayerTwoDirectionVector = AutoMovementHandler.GetMovementReflectionDirectionAfterCollision(currentPlayerTwoDirectionVector, transform.position, playerTwoInteractionSize);

        //rotate the object to face the updated direction of movement
        transform.forward = Vector3.Slerp(transform.forward, currentPlayerTwoDirectionVector, Time.deltaTime * rotationSpeed);
        /*
         * Tip - use transform.lookAt function to have object change line of sight to a point. Useful for enemies facing P2
            transform.up or transform.right can work for 2D games to change direction.

            Slerp() function makes the direction change from prev pos smoother, by adding smoothing to not make the direction change instantaneous.
         */

        //move the object position in the direction 
        transform.position += currentPlayerTwoDirectionVector * Time.deltaTime * currentPlayerTwoMovementSpeed;
        /*transform holds the position of the GameObj, apparently
        transform.position is a 3D vector.
        Time.deltaTime ensures that perceived change in position is independent of system framerate.
        Time.deltaTime returns the timelapse between 2 frames. Very small number.*/
    }


    public bool IsPlayerTwoMoving()
    {
        return isMoving;
    }
}
