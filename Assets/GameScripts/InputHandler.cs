using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{

    //Read the new input Manager in our custom input class.
    PlayerInputActions inputActions;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        //Read the new input Manager in our custom input class.
        inputActions = new PlayerInputActions();
        inputActions.PlayerOne.Enable();//PlayerOne and PlayerTwo maps were created by me.

    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 keyInputVector = new Vector2(0, 0);//Capture keyboard input to define the 2D plane where P1 will move
        /*
         * use Input.GetKey() for movement, where you need to keep the key pressed.
         * use Input.GetKeyDown() for attack/jump, where you'll need to tap it once.
         */

        //Read from the new Mapping system directly. You won't need to GetKey() manually.
        //keyInputVector = inputActions.PlayerOne.Move.ReadValue<Vector2>();
        //keyInputVector = keyInputVector.normalized;
        //return keyInputVector;

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

        return keyInputVector;
    }
}
