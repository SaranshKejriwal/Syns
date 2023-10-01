using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOneAnimation : MonoBehaviour
{

    private Animator playerOneAnimator;
    // Start is called before the first frame update
    private const string IS_MOVING_PARAM_NAME = "isMoving";//create const to avoid string case match errors
    private const string IS_PUNCH_PARAM_NAME = "isPunching";
    private const string IS_LEFT_PUNCH_PARAM_NAME = "isLeftPunch";


    [SerializeField] private PlayerOneController playerOne; // to reference the logic component of PlayerOne

    //Awake is called before Start()
    private void Awake()//inherited method
    {
        //get the animator reference
        playerOneAnimator = GetComponent<Animator>();

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //read from Player logic component if it is moving - refer PlayerOneControl.cs
        playerOneAnimator.SetBool(IS_MOVING_PARAM_NAME, playerOne.IsPlayerOneMoving());//pickup the "isMoving"  parameter from the Animator component of the player

        playerOneAnimator.SetBool(IS_PUNCH_PARAM_NAME, playerOne.IsPlayerOnePunching());//pickup the "isMoving"  parameter from the Animator component of the player

        //randomly pick left arm or right arm
        playerOneAnimator.SetBool(IS_LEFT_PUNCH_PARAM_NAME, MathFunctions.GetCoinToss());//returns true/false randomly
    }

}
