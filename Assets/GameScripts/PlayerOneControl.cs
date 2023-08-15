using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerOneControl : MonoBehaviour
{

    [SerializeField] private int moveSpeed = 3;//this private field is accessible on Inspector only, not anywhere else outside class
    private int rotationSpeed = 10;
    private bool isMoving = false; //used by animator to render movement animation if player is moving
    [SerializeField] private InputHandler inputHandler;

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
        
        //move the object position in the direction 
        transform.position += directionVector * Time.deltaTime *moveSpeed;//transform holds the position of the GameObj, apparently
                                                                          //transform.position is a 3D vector.
                                                                          //Time.deltaTime ensures that perceived change in position is independent of system framerate. 
                                                                          //Time.deltaTime returns the timelapse between 2 frames. Very small number

        //rotate the object to face the direction
        transform.forward = Vector3.Slerp(transform.forward, directionVector, Time.deltaTime*rotationSpeed);

        /*
         * Tip - use transform.lookAt function to have object change line of sight to a point. Useful for enemies facing P2
            transform.up or transform.right can work for 2D games to change direction.

            Slerp() function makes the direction change from prev pos smoother, by adding smoothing to not make the direction change instantaneous.
         */

        //Debug.Log(keyInputVector);

    }


    public bool IsMoving()
    {
        return isMoving;
    }
}
