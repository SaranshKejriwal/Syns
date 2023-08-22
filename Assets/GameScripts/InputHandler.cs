using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
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
        inputActions.PlayerTwo.Enable();

    }

    public Vector2 GetPlayerOneMovementVectorNormalized()
    {
        Vector2 keyInputVector = new Vector2(0, 0);//Capture keyboard input to define the 2D plane where P1 will move

        //Read from the new Mapping system directly. You won't need to GetKey() manually.
        keyInputVector = inputActions.PlayerOne.Move.ReadValue<Vector2>();
        //normalization is already done in the Processor of PlayerInputActions

        //this will return a 2D vector corresponding to WASD or arrows in 2D space, as configured in PlayerInputActions Object.
        return keyInputVector;

        //Refer to PlayerInputActions

    }

    public bool IsPlayerOnePunchPressed()
    {
        return inputActions.PlayerOne.Punch.IsPressed();
        
    }

    //Update Player Two speed if Faster or Slower key binding is pressed.
    public int GetCurrentPlayerTwoMovementSpeed(int currentSpeed, int maxSpeed, int minSpeed)
    {
        if(inputActions.PlayerTwo.Faster.IsPressed() && currentSpeed < maxSpeed)
        {
            //increase current Speed as long as it is under upper limit
            currentSpeed++;
            Debug.Log("increasing Player Two speed");
        }
        if(inputActions.PlayerTwo.Slower.IsPressed() && currentSpeed > minSpeed)
        {
            //reduce current Speed as long as it is above lower limit
            currentSpeed--;
            Debug.Log("decreasing Player Two speed");
        }
        Debug.Log("current Player Two speed = "+ currentSpeed);
        return currentSpeed;
    }
}
