using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerOneControl : MonoBehaviour
{

    [SerializeField] private int moveSpeed = 5;//this private field is accessible on Inspector only, not anywhere else outside class
    private int rotationSpeed = 10;
    private bool isMoving = false; //used by animator to render movement animation if player is moving
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private float playerSize = 0.5f; //needed for collision handling in Raycast function.
    private int playerHeightOffset = 2;//needed for collision handling in CapsuleCast function.


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {        //Update() is inherited from MonoBehaviour. Called on each frame. Always specify the access modifier

        Vector2 keyInputVector = inputHandler.GetMovementVectorNormalized();
        Vector3 directionVector = new Vector3(keyInputVector.x, 0f, keyInputVector.y);

        isMoving = directionVector != Vector3.zero;//if no keyInput, this condition is false 


        //bool isNotColliding = !Physics.Raycast(transform.position, directionVector, playerSize);//this is needed for collision handling
        //bool isNotColliding = !Physics.CapsuleCast(transform.position, transform.position+Vector3.up*playerHeightOffset, playerSize, directionVector, Time.deltaTime * moveSpeed);//this is needed for collision handling


        //rotate the object to face the direction
        transform.forward = Vector3.Slerp(transform.forward, directionVector, Time.deltaTime * rotationSpeed);


        //needed for collision handling - if player movement is obstructed, try x or z axis movement only
        directionVector = GetMovementDirectionAfterCollision(directionVector);
        //Note - transform.forward is called before collision handling to ensure direction is always based on key input only.

        //move the object position in the direction 
        transform.position += directionVector * Time.deltaTime * moveSpeed;
        /*transform holds the position of the GameObj, apparently
        transform.position is a 3D vector.
        Time.deltaTime ensures that perceived change in position is independent of system framerate.
        Time.deltaTime returns the timelapse between 2 frames. Very small number.*/


        
        /*
         * Tip - use transform.lookAt function to have object change line of sight to a point. Useful for enemies facing P2
            transform.up or transform.right can work for 2D games to change direction.

            Slerp() function makes the direction change from prev pos smoother, by adding smoothing to not make the direction change instantaneous.
         */

        //Debug.Log(keyInputVector);

    }
    
    
    //use - while moving diagonally and obstructed, playerOne should move in at least 1 feasible direction
    private Vector3 GetMovementDirectionAfterCollision(Vector3 currentDirectionVector)
    {
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
            Vector3 directionZAxis = new Vector3(0,0, currentDirectionVector.z).normalized;
            bool isNotCollidingX = !Physics.Raycast(transform.position, directionXAxis, playerSize);//check x axis block only
            //bool isNotCollidingX = !Physics.CapsuleCast(transform.position, transform.position+Vector3.up*playerHeightOffset, playerSize, directionXAxis, Time.deltaTime * moveSpeed);//this is needed for collision handling

            if (isNotCollidingX) {
                return directionXAxis;//Player is free to move in x axis only
            }
            else
            {
                return directionZAxis;//if Player is blocked on x, then try z
                //if both are 0, then player is completely stuck in that direction.
            }

        }

        
    }


    public bool IsMoving()
    {
        return isMoving;
    }
}
