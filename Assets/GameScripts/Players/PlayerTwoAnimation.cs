using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTwoAnimation : MonoBehaviour
{
    private Animator playeTwoAnimator;
    // Start is called before the first frame update
    private const string IS_MOVING_PARAM_NAME = "isMoving";//create const to avoid string case match errors
    
    [SerializeField] private PlayerTwoController playerTwo; // to reference the logic component of PlayerOne

    //Awake is called before Start()
    private void Awake()//inherited method
    {
        //get the animator reference
        playeTwoAnimator = GetComponent<Animator>();

    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //read from Player logic component if it is moving - refer PlayerOneControl.cs
        playeTwoAnimator.SetBool(IS_MOVING_PARAM_NAME, playerTwo.IsPlayerTwoMoving());//pickup the "isMoving"  parameter from the Animator component of the player

       
    }
}
