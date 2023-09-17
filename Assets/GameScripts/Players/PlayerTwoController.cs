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
    [SerializeField] private int maxPlayerTwoMovementSpeed = 10;
    [SerializeField] private int minPlayerTwoMovementSpeed = 1;

    private int rotationSpeed = 10;
    private bool isPlayerTwoMoving = true; //used by animator to render movement animation if player is moving
    //private bool isEvadingEnemy = false; //can be used in future to create smart reaction to enemy
    private bool canBeAttacked = true;//this will always be true except when PlayerTwo reaches Exit.

    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private float playerTwoInteractionSize = 0.5f; //needed for collision handling in Raycast function.
    //private int playerHeightOffset = 2;//needed for collision handling in CapsuleCast function.

    private float playerTwoInteractionDistance = 2f;
    private readonly float mazeCellCenterErrorMargin = 1f;
    //this is the distance that PlayerTwo has to reach from cell center, for maze traverser to trigger next cell in Stack
    
    private Vector3 currentPlayerTwoDirectionVector = Vector3.zero;//direction where it is heading
    
    //ExitKey and Door related objects
    private bool hasCollectedExitKey = false;//will be set in the ExitKeyController.
    private bool canEnterExitDoorInVicinity = false;//this will be true when PlayerTwo is in the same cell as 
    private bool retraversalRequiredAfterKeyCollect = false;//this will be true in case Player Two reaches Exit before the Key
    
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
         and then spawn at the nearest one.*/
        //nextIntendedDestination = RecursiveMazeTraverser.Instance.GetNearestMazeCellCenterToStart(transform.position);
        transform.localPosition = RecursiveMazeTraverser.Instance.GetStartingCellCenter();
        MoveToNextCell();
        //currentPlayerTwoDirectionVector = AutoMovementHandler.GetDirectionTowardsUnobstructedDestination(nextIntendedDestination, transform.position);
    }

    // Update is called once per frame
    private void Update()
    {   //Update() is inherited from MonoBehaviour. Called on each frame. Always specify the access modifier

        if (!isPlayerTwoMoving)
        {
            return;//this will only happen if PlayerTwo Wins.
        }

        //Update speed if Faster or Slower key binding is pressed.
        currentPlayerTwoMovementSpeed = inputHandler.GetCurrentPlayerTwoMovementSpeed(currentPlayerTwoMovementSpeed, maxPlayerTwoMovementSpeed, minPlayerTwoMovementSpeed);

        //using Vector3.Distance to ensure some margin of error.
        if(Vector3.Distance(nextIntendedDestination, transform.position) <= mazeCellCenterErrorMargin)
        {
            MoveToNextCell();
        }
        CheckStopOnEnteringOpenExit();
        HandleMovementWithCollision();//returns same vector unless obstructed.
        HandleAllInteractions();

    }

    private void MoveToNextCell()
    {
        //if player has reached the intended maze cell, update the maze cell to the next accessible neighbour.
        nextIntendedDestination = RecursiveMazeTraverser.Instance.GetNextCellCenterToVisit(transform.position);

        //move Player Two to next destination
        currentPlayerTwoDirectionVector = AutoMovementHandler.GetDirectionTowardsUnobstructedDestination(nextIntendedDestination, transform.position);

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
        //currentPlayerTwoDirectionVector = AutoMovementHandler.GetMovementReflectionDirectionAfterCollision(currentPlayerTwoDirectionVector, transform.position, playerTwoInteractionSize);
        //removing this since PlayerTwo will never be colliding with any obstacles, and will move in cell centers only.

        //rotate the object to face the updated direction of movement
        transform.forward = Vector3.Slerp(transform.forward, currentPlayerTwoDirectionVector, Time.deltaTime * rotationSpeed);
        /*
         * Tip - use transform.lookAt function to have object change line of sight to a point. Useful for enemies facing P2
            transform.up or transform.right can work for 2D games to change direction.

            Slerp() function makes the direction change from prev pos smoother, by adding smoothing to not make the direction change instantaneous.
         */

        //move the object position in the direction, if PLayerTwo is supposed to be moving
        if (isPlayerTwoMoving)
        {
            transform.position += currentPlayerTwoDirectionVector * Time.deltaTime * currentPlayerTwoMovementSpeed;
        }

        /*transform holds the position of the GameObj, apparently
        transform.position is a 3D vector.
        Time.deltaTime ensures that perceived change in position is independent of system framerate.
        Time.deltaTime returns the timelapse between 2 frames. Very small number.*/
    }

    //This function should stop PlayerTwo when it reaches Exit.
    private void CheckStopOnEnteringOpenExit()
    {
        if (!isPlayerTwoMoving) 
        {
            return;//if PlayerTwo has already entered Exit, need not check.
        }
        MazeCell exitDoorContainerCell = ExitDoorController.Instance.GetExitDoorContainerCell();

        if (exitDoorContainerCell.cellPositionOnMap != nextIntendedDestination)
        {            
            return;//do nothing if PlayerTwo hasn't stepped into Exit Door container cell.
        }
        if(!hasCollectedExitKey && exitDoorContainerCell.cellPositionOnMap == nextIntendedDestination)
        {
            //this means that PlayerTwo reached Exit Door cell before getting the Key; retraversal will be required
            retraversalRequiredAfterKeyCollect = true;
            return;//do nothing if Player hasn't collected the key                   
        }
        Vector3 disappearanceOffsetAfterEntry = new Vector3(0, 0, 1.5f);//this offset is for making PlayerTwo disappear inside Exit door
        Vector3 exitDoorEntryLocation = ExitDoorController.Instance.GetExitDoorPosition();
        Vector3 disappearanceLocationAfterEntry = exitDoorEntryLocation + disappearanceOffsetAfterEntry; 

        if (!canEnterExitDoorInVicinity && exitDoorContainerCell.cellPositionOnMap == nextIntendedDestination)
        {
            //Debug.Log("PlayerTwo should be in Opened Exit Door");
            canEnterExitDoorInVicinity = true;
            //Go towards Exit if it is in the same Cell.
            currentPlayerTwoDirectionVector = AutoMovementHandler.GetDirectionTowardsUnobstructedDestination(exitDoorEntryLocation, transform.position);
            
        }

        if (canEnterExitDoorInVicinity && Vector3.Distance(exitDoorEntryLocation, transform.position) <= mazeCellCenterErrorMargin)
        {
            ExitDoorController.Instance.CheckExitDoorCollectedStatus();
            currentPlayerTwoDirectionVector = AutoMovementHandler.GetDirectionTowardsUnobstructedDestination(disappearanceLocationAfterEntry, transform.position);
            canBeAttacked = false;//PlayerTwo cannot be attacked by enemies while approaching exit.            

        }
        //these 2 if conditions are added to show smooth transition between door open and exit.
        if (Vector3.Distance(disappearanceLocationAfterEntry, transform.position) <= mazeCellCenterErrorMargin)
        {
            //ExitDoorController.Instance.CheckExitDoorCollectedStatus();
            isPlayerTwoMoving = false;
            currentPlayerTwoMovementSpeed = 0;//stop Player Two.
            LevelBuilder.Instance.LevelVictory();//Close Level
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
        //currentPlayerTwoDirectionVector = AutoMovementHandler.GetMovementReflectionDirectionAfterCollision(enemyEvasionPlayerTwoDirectionVector, transform.position, playerTwoInteractionSize);
        transform.position += currentPlayerTwoDirectionVector * Time.deltaTime * maxPlayerTwoMovementSpeed;//playerTwo is running

    }

    public bool IsPlayerTwoMoving()
    {
        return isPlayerTwoMoving;
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
