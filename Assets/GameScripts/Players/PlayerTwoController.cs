using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerTwoController : GenericPlayerController
{

    //create Singleton object of this class which ALL other entities will refer to - PlayerOne or Enemies.
    private static PlayerTwoController instance;
    public static PlayerTwoController Instance
    //this instance "Property" will be tracked by ALL enemies, while keeping actual PlayerTwo object private
    { 
        get { return instance; }//not very different from getters and setters
        private set { instance = value; }//we do not want any other object to modify PlayerTwo entirely.
    }

    [SerializeField][Range(1,8)] private int currentPlayerTwoMovementSpeed = 3;//this private field is accessible on Inspector only, not anywhere else outside class
    [SerializeField] private int maxPlayerTwoMovementSpeed = 7;
    [SerializeField] private int minPlayerTwoMovementSpeed = 1;

    private int rotationSpeed = 10;
    private bool isMoving = true; //used by animator to render movement animation if player is moving
    //private bool isEvadingEnemy = false; //can be used in future to create smart reaction to enemy
    private bool canBeAttacked = true;//this will always be true except when PlayerTwo reaches Exit.

    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private float playerTwoInteractionSize = 0.5f; //needed for collision handling in Raycast function.
    //private int playerHeightOffset = 2;//needed for collision handling in CapsuleCast function.

    private float playerTwoInteractionDistance = 2f;
    private float mazeCellCenterErrorMargin = 0.5f;
    //this is the distance that PlayerTwo has to reach from cell center, for maze traverser to trigger next cell in Stack
    
    private Vector3 currentPlayerTwoDirectionVector = Vector3.zero;//direction where it is heading
    
    //ExitKey and Door related objects
    private bool hasCollectedExitKey = false;//will be set in the ExitKeyController.
    private bool canEnterExitDoorInVicinity = false;//this will be true when PlayerTwo is in the same cell as 

    
    //Awake will be called before Start()
    private void Awake()
    {
        if (instance == null) 
        { 
            instance = this;//start by setting the Singleton instance 
        }
        else
        {
            Debug.LogError("Fatal Error: Cannot have a predefined instance of PlayerTwo");
        }
        instance.playerHealth = 15;
        instance.isActive = true;
        instance.playerType = PlayerType.PlayerTwo;
    }

    // Start is called before the first frame update
    void Start()
    {
        //choose a random starting direction to start moving
        //currentPlayerTwoDirectionVector = AutoMovementHandler.GetRandomDirectionVector();

        /*Player Two movement should not be random. It should mmap out all the maze cells on the map
         and then go for the nearest one.*/
        nextIntendedDestination = RecursiveMazeTraverser.Instance.GetNearestMazeCellCenterToStart(transform.position);
        currentPlayerTwoDirectionVector = AutoMovementHandler.GetDirectionTowardsUnobstructedDestination(nextIntendedDestination, transform.position);
    }

    // Update is called once per frame
    private void Update()
    {   //Update() is inherited from MonoBehaviour. Called on each frame. Always specify the access modifier
        
        //Update speed if Faster or Slower key binding is pressed.
        currentPlayerTwoMovementSpeed = inputHandler.GetCurrentPlayerTwoMovementSpeed(currentPlayerTwoMovementSpeed, maxPlayerTwoMovementSpeed, minPlayerTwoMovementSpeed);

        //using Vector3.Distance to ensure some margin of error.
        if(Vector3.Distance(nextIntendedDestination, transform.position) <= mazeCellCenterErrorMargin)
        {
            //if player has reached the intended maze cell, update the maze cell to the next accessible neighbour.
            nextIntendedDestination = RecursiveMazeTraverser.Instance.GetNextCellCenterToVisit(transform.position);
            
            //move Player Two to next destination
            currentPlayerTwoDirectionVector = AutoMovementHandler.GetDirectionTowardsUnobstructedDestination(nextIntendedDestination, transform.position);
        }
        CheckStopOnEnteringOpenExit();
        HandleMovementWithCollision();//returns same vector unless obstructed.
        HandleAllInteractions();

    }


    private void HandleAllInteractions()
    {
        
        if (Physics.Raycast(transform.position, currentPlayerTwoDirectionVector, out RaycastHit rayCastHit, playerTwoInteractionDistance))
        //tells us if something is in front and returns its object parameter in rayCastHit object
        {
            //Debug.Log(rayCastHit.transform);//returns the name of the object that was hit.

            //tries to confirm if interacted component is of a specific type.
            if (rayCastHit.transform.TryGetComponent(out EnemyController approachingEnemy))
            {
                Debug.Log(approachingEnemy);
                approachingEnemy.RespondToPlayerTwoInteraction();
            }

        }

    }


    private void HandleMovementWithCollision()
    {               

        //needed for collision handling - if player movement is obstructed, try x or z axis movement only
        currentPlayerTwoDirectionVector = AutoMovementHandler.GetMovementReflectionDirectionAfterCollision(currentPlayerTwoDirectionVector, transform.position, playerTwoInteractionSize);

        //rotate the object to face the updated direction of movement
        transform.forward = Vector3.Slerp(transform.forward, currentPlayerTwoDirectionVector, Time.deltaTime * rotationSpeed);
        /*
         * Tip - use transform.lookAt function to have object change line of sight to a point. Useful for enemies facing P2
            transform.up or transform.right can work for 2D games to change direction.

            Slerp() function makes the direction change from prev pos smoother, by adding smoothing to not make the direction change instantaneous.
         */

        //move the object position in the direction, if PLayerTwo is supposed to be moving
        if (isMoving)
        {
            transform.position += currentPlayerTwoDirectionVector * Time.deltaTime * currentPlayerTwoMovementSpeed;
        }
        else
        {
            currentPlayerTwoMovementSpeed = 0;
        }
        /*transform holds the position of the GameObj, apparently
        transform.position is a 3D vector.
        Time.deltaTime ensures that perceived change in position is independent of system framerate.
        Time.deltaTime returns the timelapse between 2 frames. Very small number.*/
    }

    //This function should stop PlayerTwo when it reaches Exit.
    private void CheckStopOnEnteringOpenExit()
    {
        MazeCell exitDoorContainerCell = ExitDoorController.Instance.GetExitDoorContainerCell();
        if (!hasCollectedExitKey || exitDoorContainerCell.cellPositionOnMap != nextIntendedDestination)
        {
            
            return;//do nothing if Player hasn't collected the key
                   //or if PlayerTwo won't step into Exit Door container cell.
        }        
        Vector3 disappearanceOffsetAfterEntry = new Vector3(0, 0, 1f);//this offset is for making PlayerTwo disappear inside Exit door
        Vector3 exitDoorEntryLocation = ExitDoorController.Instance.GetExitDoorPosition() + disappearanceOffsetAfterEntry;

        if (exitDoorContainerCell.cellPositionOnMap == nextIntendedDestination)
        {
            Debug.Log("PlayerTwo should be in Opened Exit Door");
            canEnterExitDoorInVicinity = true;
            //Go towards Exit if it is in the same Cell.
            currentPlayerTwoDirectionVector = AutoMovementHandler.GetDirectionTowardsUnobstructedDestination(exitDoorEntryLocation, transform.position);
            canBeAttacked = false;//PlayerTwo cannot be attacked by enemies while approaching exit.
        }

        if (Vector3.Distance(exitDoorEntryLocation, transform.position) <= mazeCellCenterErrorMargin)
        {
            isMoving = false;
            currentPlayerTwoMovementSpeed = 0;//stop Player Two.
        }
    }

    public void RespondToEnemyHunt(object sender, System.EventArgs e)
    {
        Debug.Log("PlayerTwo responding to Enemy Event.");
    }

    public void EvadeEnemyPosition(Vector3 EnemyPosition)
    {
        //Debug.Log("PlayerTwo evading Enemy Position " + EnemyPosition);
        Vector3 enemyEvasionPlayerTwoDirectionVector = AutoMovementHandler.GetDirectionAwayFromLocationToEvade(EnemyPosition,transform.position);

        //separate enemy Evasion vector is created to ensure that PlayerTwo doesn't break through walls
        currentPlayerTwoDirectionVector = AutoMovementHandler.GetMovementReflectionDirectionAfterCollision(enemyEvasionPlayerTwoDirectionVector, transform.position, playerTwoInteractionSize);
        transform.position += currentPlayerTwoDirectionVector * Time.deltaTime * maxPlayerTwoMovementSpeed;//playerTwo is running

    }

    public bool IsPlayerTwoMoving()
    {
        return isMoving;
    }

    public override Vector3 GetPlayerPosition()
    {
        return transform.position;
    }

    public bool HasCollectedExitKey()
    {
        return instance.hasCollectedExitKey;
    }

    public bool CanBeAttacked()
    {
        return canBeAttacked;
    }

    public bool CanEnterExitDoorInVicinity()
    {
        return canEnterExitDoorInVicinity;
    }

    public void SetHasCollectedExitKey(bool hasCollectedExitKey)
    {
        instance.hasCollectedExitKey = hasCollectedExitKey;
        ExitDoorController.Instance.EnableExitDoorForPlayerTwo();
        //update ExitDoor status to be correctly collectable by PlayerTwo.
    }

}
