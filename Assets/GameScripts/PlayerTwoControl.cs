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
    private bool isMoving = false; //used by animator to render movement animation if player is moving
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private float playerSize = 0.5f; //needed for collision handling in Raycast function.
    //private int playerHeightOffset = 2;//needed for collision handling in CapsuleCast function.

    private int playerTwoInteractionDistance = 2;
    
    private Vector3 currentPlayerTwoDirectionVector = Vector3.zero;
    private Vector3 lastInteractionDirecctionVector = Vector3.zero;



    // Start is called before the first frame update
    void Start()
    {
        //choose a random starting direction to start moving
        currentPlayerTwoDirectionVector = GetRandomDirectionVector();
    }

    // Update is called once per frame
    private void Update()
    {        //Update() is inherited from MonoBehaviour. Called on each frame. Always specify the access modifier
        HandleMovementWithCollision();
        HandleInteractions();
        //HandleAttackAction();
    }


    private void HandleInteractions()
    {
        //Get separate direction vector, to not interfere with occlusion handling vector
        Vector2 keyInputVector = inputHandler.GetPlayerOneMovementVectorNormalized();

        Vector3 movementDirectionVector = new Vector3(keyInputVector.x, 0f, keyInputVector.y);

        if (movementDirectionVector != Vector3.zero)
        {
            lastInteractionDirecctionVector = movementDirectionVector;
            //this ensures that interaction is saved even when movement isn't happening - interaction continues even if movement stops.
        }

        if (Physics.Raycast(transform.position, lastInteractionDirecctionVector, out RaycastHit rayCastHit, playerTwoInteractionDistance))
        //tells us if something is in front and returns its object parameter in rayCastHit object
        {
            //Debug.Log(rayCastHit.transform);//returns the name of the object that was hit.

            //tries to confirm if interacted component is of a specific type.
            if (rayCastHit.transform.TryGetComponent(out TestInteractionLogic testInteract))
            {
                testInteract.Interact();
            }


        }

    }


    private void HandleMovementWithCollision()
    {
        //Update speed if Faster or Slower key binding is pressed.
        currentPlayerTwoMovementSpeed = inputHandler.GetCurrentPlayerTwoMovementSpeed(currentPlayerTwoMovementSpeed, maxPlayerTwoMovementSpeed, minPlayerTwoMovementSpeed);

        //this will always be true for PlayerTwo since currentPlayerTwoDirection cannot be zero 
        isMoving = currentPlayerTwoDirectionVector != Vector3.zero;

        //rotate the object to face the direction
        transform.forward = Vector3.Slerp(transform.forward, currentPlayerTwoDirectionVector, Time.deltaTime * rotationSpeed);
        /*
         * Tip - use transform.lookAt function to have object change line of sight to a point. Useful for enemies facing P2
            transform.up or transform.right can work for 2D games to change direction.

            Slerp() function makes the direction change from prev pos smoother, by adding smoothing to not make the direction change instantaneous.
         */

        //needed for collision handling - if player movement is obstructed, try x or z axis movement only
        currentPlayerTwoDirectionVector = GetMovementDirectionAfterCollision(currentPlayerTwoDirectionVector);
        //Note - transform.forward is called before collision handling to ensure direction is always based on key input only.

        //move the object position in the direction 
        transform.position += currentPlayerTwoDirectionVector * Time.deltaTime * currentPlayerTwoMovementSpeed;
        /*transform holds the position of the GameObj, apparently
        transform.position is a 3D vector.
        Time.deltaTime ensures that perceived change in position is independent of system framerate.
        Time.deltaTime returns the timelapse between 2 frames. Very small number.*/



        

    }


    //use - while moving diagonally and obstructed, playerOne should move in at least 1 feasible direction
    private Vector3 GetMovementDirectionAfterCollision(Vector3 currentDirectionVector)
    {
        //check if there is collision in X or Z
        bool isNotColliding = !Physics.Raycast(transform.position, currentDirectionVector, playerSize);//this is needed for collision handling
        //bool isNotColliding = !Physics.CapsuleCast(transform.position, transform.position+Vector3.up*playerHeightOffset, playerSize, directionVector, Time.deltaTime * moveSpeed);//this is needed for collision handling

        if (isNotColliding)
        {
            return currentDirectionVector; //no collisions currently, can proceed in present direction
        }
        else
        {
            //attempt seprate x-direction or z-direction movement - normalized vectors ensure consistent speed by changing to unit magnitude vectors
            Vector3 directionXAxis = new Vector3(currentDirectionVector.x, 0, 0).normalized;
            bool isNotCollidingX = !Physics.Raycast(transform.position, directionXAxis, playerSize);//check x axis block only
            //bool isNotCollidingX = !Physics.CapsuleCast(transform.position, transform.position+Vector3.up*playerHeightOffset, playerSize, directionXAxis, Time.deltaTime * moveSpeed);//this is needed for collision handling

            if (isNotCollidingX)
            {
                return new Vector3(currentDirectionVector.x, 0f, (-1) * currentDirectionVector.z);
                //Reflection on Z axis only
            }
            else
            {
                return new Vector3((-1)*currentDirectionVector.x, 0f, currentDirectionVector.z);
                //Reflection on X axis only
            }

        }


    }

    private Vector3 GetRandomDirectionVector()
    {
        //Random.Range(-1f,1f) will return any random direction in 0 to 360 degree angle from starting point
        Vector3 randomDirectionVector = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

        //run a while loop to ensure that you don't randomly get a zero vector from above step
        while (randomDirectionVector == Vector3.zero)
        {
            Debug.Log("Player Two selected Zero Vector as Starting direction; choosing new vector");
            randomDirectionVector = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        }

        return randomDirectionVector.normalized;
    }

    public bool IsPlayerTwoMoving()
    {
        return isMoving;
    }
}
