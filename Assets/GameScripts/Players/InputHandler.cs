using System; //for EventHandler
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class InputHandler : MonoBehaviour
{

    //Read the new input Manager in our custom input class.
    PlayerInputActions inputActions;

    public event EventHandler OnPunchAction;//EventHandler is a "delegate"



    // Update is called once per frame
    void Update()
    {
        
    }
    
    //Awake functions are called before the Start of all Gameobjects
    private void Awake()
    {
        //Read the new input Manager in our custom input class.
        inputActions = new PlayerInputActions();
        inputActions.PlayerOne.Enable();//PlayerOne and PlayerTwo maps were created by me.
        inputActions.PlayerTwo.Enable();

        //Event to trigger player one attack action
        inputActions.PlayerOne.Punch.performed += PlayerOne_Punch_performed;//the function is not called, but passed as a reference.

    }

    // Start is called before the first frame update
    private void Start()
    {
        //Add PlayerTwo Speed methods as subscribers
        inputActions.PlayerTwo.Faster.performed += PlayerTwoController.Instance.IncreasePlayerTwoSpeedOnFasterInputPress;
        inputActions.PlayerTwo.Slower.performed += PlayerTwoController.Instance.DecreasePlayerTwoSpeedOnSlowerInputPress;
        inputActions.Pause.Pause.performed += LevelTransitionManager.Instance.ShowLevelPauseCanvas;
    }

    private void PlayerOne_Punch_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        //Player One Logic Class will listen to this OnPunchAction event, Not the Punch.performed event

        if (OnPunchAction != null) { //check if something is listening to this event
            OnPunchAction(this, EventArgs.Empty);//no additional arguments needed yet
            //sender is the InputHandler Class itself, hence 'this'   
        }
    }

    public Vector2 GetPlayerOneMovementVectorNormalized()
    {
        Vector2 keyInputVector = new Vector2(0, 0);//Capture keyboard input to define the 2D plane where P1 will move

        //Read from the new Mapping system directly. You won't need to GetKey() manually.
        keyInputVector = inputActions.PlayerOne.Move.ReadValue<Vector2>();
        //normalization is already done in the Processor of PlayerInputActions

        //this will return a 2D vector corresponding to WASD or arrows in 2D space, as configured in PlayerInputActions Object.
        return keyInputVector;

        //Refer to PlayerInputActions Asset

    }

    public bool IsPlayerOnePunchPressed()
    {
        return inputActions.PlayerOne.Punch.IsPressed();
        
    }

}
