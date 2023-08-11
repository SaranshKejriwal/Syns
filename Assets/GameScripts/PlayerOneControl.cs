using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerOneControl : MonoBehaviour
{

    [SerializeField] private int moveSpeed = 2;//this private field is accessible on Inspector only, not anywhere else outside class
    private int rotationSpeed = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {        //Update() is inherited from MonoBehaviour. Called on each frame. Always specify the access modifier

        Vector2 keyInputVector = new Vector2(0, 0);//Capture keyboard input to define the 2D plane where P1 will move
        /*
         * use Input.GetKey() for movement, where you need to keep the key pressed.
         * use Input.GetKeyDown() for attack/jump, where you'll need to tap it once.
         */

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        { 
            keyInputVector.y++;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            keyInputVector.x--;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            keyInputVector.y--;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            keyInputVector.x++;
        }
        keyInputVector = keyInputVector.normalized;//this ensures that (1,1) does not cover sqrt(2) distance and make P1 faster. Changes to 1/sqrt(2) in each axis


        //keyInputVector has accepted user input, which can be translated to movement
        //translation will be helpful in managing key rebinding 
        Vector3 directionVector = new Vector3(keyInputVector.x, 0f, keyInputVector.y);
        
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
}
