using System;
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

    private int currentPlayerTwoMovementSpeed = 3;//maintained as separate Class property because it can be controlled by user.
    private float minPlayerTwoMovementSpeed = 1f;//these speeds are maintained as float to allow HUD to compute ratio
    //max

    private int rotationSpeed = 10;
    private bool isPlayerTwoMoving = true; //used by animator to render movement animation if player is moving. Will always be true anyway
    
    
    [SerializeField] private InputHandler inputHandler;

    //private float playerTwoInteractionDistance = 2f;
    private readonly float mazeCellCenterErrorMargin = 1.5f;
    //this is the distance that PlayerTwo has to reach from cell center, for maze traverser to trigger next cell in Stack
    
    private Vector3 currentPlayerTwoDirectionVector = Vector3.zero;//direction where it is heading
    
    //ExitKey and Door related objects
    private bool isEnteringExitDoorInVicinity = false;//this will be true when PlayerTwo is in the same cell as Exit Door

    protected Vector3 nextIntendedDestination = Vector3.zero;
    //this will be used to dictate the target for PlayerTwo only, not Applicable for PlayerOne

    private GenericEnemyController enemyToEvade;
    //this object will be populated by the last enemy which is hunting/attacking P2.
    //if this is not null, andt hunting/attacking P2, then P2 should be in evasive mode.
    //Risk - multiple enemies can set themselves as the enemyToEvade.


    //Player Two XP growth is based on distance from PlayerOne in each frame.
    private float minPlayerOneDistanceForXP = 20f;

    //This event will be fired when PlayerTwo enters the open Exit door.
    public event EventHandler OnPlayerTwoExit;

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
        instance.currentPlayerHealth = 15;
        instance.PlayerControllerProperties.maxPlayerHealth = 15;
        instance.isActive = true;
        instance.playerType = PlayerType.PlayerTwo;
        instance.playerState = PlayerState.isMoving;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Add subscribers to OnPlayerTwoEvent
        OnPlayerTwoExit += LevelBuilder.Instance.RecordLevelVictory;

        //OnPlayerTwoExit += EnemyGruntController.StopGruntMovement;
        //Grunts subscribe to this event in their own class.
    }

    // Update is called once per frame
    private void Update()
    {   //Update() is inherited from MonoBehaviour. Called on each frame. Always specify the access modifier

        if (!GameMaster.Instance.IsLevelPlaying())
        {
            return;//do nothing if game is paused or level has ended.
        }

        if (!isPlayerTwoMoving)
        {
            return;//this will only happen if PlayerTwo Wins.
        }

        //Update speed if Faster or Slower key binding is pressed - this is handled by Event now

        //using Vector3.Distance to ensure some margin of error.
        if(Vector3.Distance(nextIntendedDestination, transform.position) <= mazeCellCenterErrorMargin)
        {         
            DefineNextPlayerTwoDestination();           
                       
        }

        CheckVicinityToOpenExitDoor();
        CheckPlayerTwoExitToFireEvent();

        MovePlayerTwo();//returns same vector unless obstructed.

        IncreasePlayerTwoXP();

    }

    public void PlacePlayerTwoOnLevelStart()
    {
        //this will only be called at the start of a level
        transform.localPosition = RecursiveMazeTraverser.Instance.GetStartingCellCenter();
        DefineNextPlayerTwoDestination();

        //display starting speed in HUD
        LevelHUDStatsManager.Instance.UpdateHUDPlayerTwoSpeedbar(currentPlayerTwoMovementSpeed, PlayerControllerProperties.maxPlayerMovementSpeed);

        //reset all flags of PlayerTwo and ExitDoor coordination
        isPlayerTwoMoving = true;
        isEnteringExitDoorInVicinity = false;
        canBeAttacked = true;
    }

    private void DefineNextPlayerTwoDestination()
    {
        if (ShouldBeEvadingEnemy())
        {
            //if player has reached the intended maze cell, update the maze cell to the next accessible neighbour to avoid enemy.
            nextIntendedDestination = RecursiveMazeTraverser.Instance.GetNextCellCenterToEvadeEnemy(enemyToEvade.GetEnemyPosition());
        }
        else
        {
            //if player has reached the intended maze cell, update the maze cell to the next accessible, unvisited neighbour.
            nextIntendedDestination = RecursiveMazeTraverser.Instance.GetNextCellCenterToVisit();
        }

        //nextIntendedDestination = RecursiveMazeTraverser.Instance.GetNextCellCenterToVisit();


        //move Player Two to next destination
        currentPlayerTwoDirectionVector = AutoMovementHandler.GetDirectionTowardsUnobstructedDestination(nextIntendedDestination, transform.position);
    }


    //this method is currently useless. Should be removed.
    /*private void HandleAllInteractions()
    {

        
        if (Physics.Raycast(transform.position, currentPlayerTwoDirectionVector, out RaycastHit rayCastHit, playerTwoInteractionDistance))
        //tells us if something is in front and returns its object parameter in rayCastHit object
        {
            //Debug.Log(rayCastHit.transform);//returns the name of the object that was hit.

            //tries to confirm if interacted component is of a specific type.
            if (rayCastHit.transform.TryGetComponent(out EnemyController approachingEnemy))
            {
                Debug.Log(approachingEnemy);
                //approachingEnemy.RespondToPlayerTwoInteraction();
            }

        }

    }*/

    //moves the PlayerTwo object to the next cell center.
    private void MovePlayerTwo()
    {               

        //PlayerTwo will never be colliding with any obstacles, and will move in cell centers only.

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
    /*private void CheckStopOnEnteringOpenExit()
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
            return;//do nothing if Player hasn't collected the key                   
        }
        Vector3 disappearanceOffsetAfterEntry = new Vector3(0, 0, 1.5f);//this offset is for making PlayerTwo disappear inside Exit door
        Vector3 exitDoorEntryLocation = ExitDoorController.Instance.GetExitDoorPosition() + new Vector3(0, 0, -1f);//to make it look like P2 entered the door and didn't go from the side.
        Vector3 disappearanceLocationAfterEntry = exitDoorEntryLocation + disappearanceOffsetAfterEntry; 

        if (!canEnterExitDoorInVicinity && exitDoorContainerCell.cellPositionOnMap == nextIntendedDestination)
        {
            canEnterExitDoorInVicinity = true;
            //ExitDoorController.Instance.OpenExitDoor(null, null);
            //Go towards Exit if it is in the same Cell.
            currentPlayerTwoDirectionVector = AutoMovementHandler.GetDirectionTowardsUnobstructedDestination(exitDoorEntryLocation, transform.position);
            Debug.Log("0");

        }

        //move player inside door, such that it starts to disappear
        if (canEnterExitDoorInVicinity)
        {
            currentPlayerTwoDirectionVector = AutoMovementHandler.GetDirectionTowardsUnobstructedDestination(disappearanceLocationAfterEntry, transform.position);
            canBeAttacked = false;//PlayerTwo cannot be attacked by enemies while approaching exit.
            LevelBuilder.Instance.RecordLevelVictory(null, null);//Close Level
            Debug.Log("1");

        }
        //these 2 if conditions are added to show smooth transition between door open and exit.
        if (Vector3.Distance(disappearanceLocationAfterEntry, transform.position) <= mazeCellCenterErrorMargin)
        {
            //ExitDoorController.Instance.CheckExitDoorCollectedStatus();
            isPlayerTwoMoving = false;
            currentPlayerTwoMovementSpeed = 0;//stop Player Two animation.
            //LevelBuilder.Instance.LevelVictory();//Close Level
            Debug.Log("2");
        }
    }*/

    //check exit door availability only when PlayerTwo is in Exit Door cell and door is open, and move PlayerTwo towards door
    private void CheckVicinityToOpenExitDoor()
    {
        if (!isPlayerTwoMoving || isEnteringExitDoorInVicinity)
        {
            return;//if PlayerTwo already entered/entering Exit, need not check.
        }

        if (!HasCollectedExitKey())
        {
            return;//do nothing if Player hasn't collected the key                   
        }

        MazeCell exitDoorContainerCell = ExitDoorController.Instance.GetExitDoorContainerCell();
        if (Vector3.Distance(transform.position,exitDoorContainerCell.cellPositionOnMap) > mazeCellCenterErrorMargin)
        {
            return; //Do nothing if PlayerTwo is far away from door cell, even if door is open
        }

        //it is assumed that P2 will still be mobile and flickering when it enters the door, but the victory screen will cover that.
        currentPlayerTwoDirectionVector = AutoMovementHandler.GetDirectionTowardsUnobstructedDestination(ExitDoorController.Instance.GetExitDoorPosition(), transform.position);
        isEnteringExitDoorInVicinity = true;

        //show Exit Door open visual.
        ExitDoorController.Instance.OpenExitDoorForBothPlayers();
       
    }

    //This method will fire the event corresponding to PlayerTwoExit 
    private void CheckPlayerTwoExitToFireEvent()
    {

        if (!isEnteringExitDoorInVicinity || !isPlayerTwoMoving)
        {
            return;//Player two isn't entering the exit yet. Can't fire the event.
            //OR PlayerTwo has already entered the exit. Nothing to check
        }

        //This includes the disappearance vector
        Vector3 exitDoorEntryVector = ExitDoorController.Instance.GetExitDoorPosition() + new Vector3(0, 0, 2f);

        //if PlayerTwo is within range of disappearing within Exit Door, fire the event, else return.
        if (Vector3.Distance(exitDoorEntryVector, transform.position) > mazeCellCenterErrorMargin)
        {
            return;//PlayerTwo is still too far to fire the event.
        }

        canBeAttacked = false;//PlayerTwo cannot be attacked by enemies while approaching exit.
        isPlayerTwoMoving = false;//Stop Player Two

        //Fire an event here - listeners will open Exit Door, Publish victory, stop all grunts.
        if (OnPlayerTwoExit != null)
        {
            OnPlayerTwoExit(this, EventArgs.Empty);
        }
    }

    //this will subscribe to inputHandler Faster pressed event and increase PlayerTwo speed.
    public void IncreasePlayerTwoSpeedOnFasterInputPress(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if(currentPlayerTwoMovementSpeed < PlayerControllerProperties.maxPlayerMovementSpeed)
        {
            currentPlayerTwoMovementSpeed+=1;
            LevelHUDStatsManager.Instance.UpdateHUDPlayerTwoSpeedbar(currentPlayerTwoMovementSpeed, PlayerControllerProperties.maxPlayerMovementSpeed);
        }
    }

    //this will subscribe to inputHandler Faster pressed event and decrease PlayerTwo speed.
    public void DecreasePlayerTwoSpeedOnSlowerInputPress(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {

        if (currentPlayerTwoMovementSpeed > minPlayerTwoMovementSpeed)
        {
            currentPlayerTwoMovementSpeed-=1;
            LevelHUDStatsManager.Instance.UpdateHUDPlayerTwoSpeedbar(currentPlayerTwoMovementSpeed, PlayerControllerProperties.maxPlayerMovementSpeed);
        }
    }

    private void IncreasePlayerTwoXP()
    {
        float distanceFromPlayerOne = Vector3.Distance(transform.position, PlayerOneController.Instance.GetPlayerPosition());

        if(distanceFromPlayerOne <= minPlayerOneDistanceForXP)
        {
            return;//no XP gain if Player Two is near Player One.
        }

        IncreasePlayerXP(Time.deltaTime);//1xp for each second away from P1

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
        return ExitKeyController.Instance.IsCollected();
    }

    public bool CanEnterExitDoorInVicinity()
    {
        return isEnteringExitDoorInVicinity;
    }


    protected override void KillPlayer()
    {
        Debug.Log(this + " is dead.");
        instance.playerState = PlayerState.isDead;

        instance.canBeAttacked = false; //no point in attacking if player is already dead.

        //If PlayerTwo Dies, level is lost.
        LevelBuilder.Instance.LevelDefeat();
    }

    public override void SetEnemyInFocus(GenericEnemyController enemy)
    {
        //PlayerTwo will evade only 1 enemy at a time.
        instance.enemyToEvade = enemy;
    }

    private bool ShouldBeEvadingEnemy()
    {

        if(enemyToEvade == null)
        {
            //no enemy has approached P2 yet. Nothing to evade
            return false;
        }
        if (enemyToEvade.IsEnemyDead() || enemyToEvade.IsEnemyMoving())
        {
            return false;//enemy is already dead or has stopped chasing PlayerTwo
        }
        if(enemyToEvade.IsEnemyHunting() || enemyToEvade.IsEnemyAttacking())
        {
            if (enemyToEvade.IsTargetingPlayerTwo())//needs revision
            {
                //evade only if Enemy is targeting PlayerTwo. Else not
                Debug.Log("PlayerTwo Should be Evading enemy");
                return true;
            }

        }


        return false;
    }

}
