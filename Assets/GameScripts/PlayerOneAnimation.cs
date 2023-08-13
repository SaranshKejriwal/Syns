using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOneAnimation : MonoBehaviour
{

    private Animator playerOneAnimator;
    // Start is called before the first frame update
    private const string IS_MOVING_PARAM_NAME = "isMoving";//create const to avoid string case match errors

    [SerializeField] private PlayerOneControl playerOne; // to reference the logic component of PlayerOne

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
        playerOneAnimator.SetBool(IS_MOVING_PARAM_NAME, playerOne.IsMoving());//pickup the "isMoving"  parameter from the Animator component of the player
    }
}
