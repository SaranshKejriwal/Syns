using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerOneController : GenericPlayerController
{
    private static PlayerOneController instance;
    public static PlayerOneController Instance
    //this instance "Property" will be tracked by ALL enemies, while keeping actual PlayerTwo object private
    {
        get { return instance; }//not very different from getters and setters
        private set { instance = value; }//we do not want any other object to modify PlayerTwo entirely.
    }

    [SerializeField] private int moveSpeed = 5;//this private field is accessible on Inspector only, not anywhere else outside class
    private int rotationSpeed = 10;
    private bool isMoving = false; //used by animator to render movement animation if player is moving
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private float playerOneInteractionSize = 0.5f; //needed for collision handling in Raycast function.
    //private int playerHeightOffset = 2;//needed for collision handling in CapsuleCast function.

    private int playerOneInteractionDistance = 1;
    private Vector3 lastInteractionDirectionVector = Vector3.zero;
    // Start is called before the first frame update

    private EnemyController approachedEnemy;//to be replaced by Enemy logic object later

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;//start by setting the Singleton instance 
        }
        else
        {
            Debug.LogError("Fatal Error: Cannot have a predefined instance of PlayerOne");
        }
        instance.playerHealth = 35;//Much higher than PlayerTwo
        instance.isActive = true;//player is Active.
        instance.playerType = PlayerType.PlayerOne;//set PlayerType of its parent class member
    }

    void Start()
    {
        //Move PlayerOne slightly left of starting Cell
        float cellLength = LevelBuilder.Instance.GetCellSideLength();
        Vector3 startingSpawnOffset = new Vector3(-cellLength / 5f, 0, 0);
        transform.localPosition = RecursiveMazeTraverser.Instance.GetStartingCellCenter() +startingSpawnOffset ;


        //listen to events on Start(), not Awake()
        inputHandler.OnPunchAction += InputHandler_OnPunchAction;
        
    }

    private void InputHandler_OnPunchAction(object sender, System.EventArgs e)
    {
        //What will this object do when PunchAction is pressed?

        //if Raycast hits Enemy in HandleInteractions(), approachedEnemy is updated. When PlayerOne punches, Enemy reaction is called
        if (approachedEnemy != null)
        {
            //only nearest enemy responds, ONLY when Player One Punches
            approachedEnemy.RespondToPlayerOnePunch();//straightforward non-singleton approach.

        }
        
    }

    // Update is called once per frame
    private void Update()
    {        //Update() is inherited from MonoBehaviour. Called on each frame. Always specify the access modifier
        HandleMovementWithCollision();
        HandleInteractions();
        //HandleAttackAction();
    }
    

    private void HandleInteractions()
    {
        //Get separate direction vector, to not interfere with occlusion handling vector
        Vector2 keyInputVector = inputHandler.GetPlayerOneMovementVectorNormalized();
        Vector3 movementDirectionVector = new Vector3(keyInputVector.x, 0f, keyInputVector.y);

        if(movementDirectionVector != Vector3.zero )
        {
            lastInteractionDirectionVector = movementDirectionVector;
            //this ensures that interaction is saved even when movement isn't happening - interaction continues even if movement stops.
        }

        if(Physics.Raycast(transform.position, lastInteractionDirectionVector, out RaycastHit rayCastHit,  playerOneInteractionDistance))
        //tells us if something is in front and returns its object parameter in rayCastHit object
        {
            //Debug.Log(rayCastHit.transform);//returns the name of the object that was hit.

            //tries to confirm if interacted component is of a specific type.
            if(rayCastHit.transform.TryGetComponent(out EnemyController interactedEnemy))
            {               
                approachedEnemy = interactedEnemy;//assign this nearest Enemy to interacted Enemy objecct for P1.
            }
            else
            {
                //raycast is no longer hitting enemy. Reset all previously selected/interacted objects to null
                approachedEnemy = null;
            }


        }
        else
        {
            //raycast has not hit anything. Reset all selected/interacted objects to null
            approachedEnemy = null;
        }
        //Debug.Log(approachedTestInteractObject);
    }

    public bool IsPlayerOnePunching()
    {
       return inputHandler.IsPlayerOnePunchPressed();
    }

    private void HandleMovementWithCollision()
    {
        Vector2 keyInputVector = inputHandler.GetPlayerOneMovementVectorNormalized();
        Vector3 directionVector = new Vector3(keyInputVector.x, 0f, keyInputVector.y);

        isMoving = directionVector != Vector3.zero;//if no keyInput, this condition is false 
        //this boolean is used in the animator
                
        //rotate the object to face the direction
        transform.forward = Vector3.Slerp(transform.forward, directionVector, Time.deltaTime * rotationSpeed);


        //needed for collision handling - if player movement is obstructed, try x or z axis movement only
        directionVector = GetMovementDirectionAlongObstruction(directionVector);
        //Note - transform.forward is called before collision handling to ensure direction is always based on key input only.

        //move the object position in the direction 
        transform.position += directionVector * Time.deltaTime * moveSpeed;
        /*transform holds the position of the GameObj, apparently
        transform.position is a 3D vector.
        Time.deltaTime ensures that perceived change in position is independent of system framerate.
        Time.deltaTime returns the timelapse between 2 frames. Very small number.*/



        /*
         * Tip - use transform.lookAt function to have object change line of sight to a point. Useful for enemies facing P2
            transform.up or transform.right can work for 2D games to change direction.

            Slerp() function makes the direction change from prev pos smoother, by adding smoothing to not make the direction change instantaneous.
         */

    }


    //use - while moving diagonally and obstructed, playerOne should move in at least 1 feasible direction
    private Vector3 GetMovementDirectionAlongObstruction(Vector3 currentDirectionVector)
    {
        bool isNotColliding = !Physics.Raycast(transform.position, currentDirectionVector, playerOneInteractionSize);//this is needed for collision handling
        //bool isNotColliding = !Physics.CapsuleCast(transform.position, transform.position+Vector3.up*playerHeightOffset, playerSize, directionVector, Time.deltaTime * moveSpeed);//this is needed for collision handling

        if (isNotColliding)
        {
            return currentDirectionVector; //no collisions currently, can proceed in present direction
        }
        else
        {
            //attempt seprate x-direction or z-direction movement - normalized vectors ensure consistent speed by changing to unit magnitude vectors
            Vector3 directionXAxis = new Vector3(currentDirectionVector.x, 0, 0).normalized;
            Vector3 directionZAxis = new Vector3(0,0, currentDirectionVector.z).normalized;
            bool isNotCollidingX = !Physics.Raycast(transform.position, directionXAxis, playerOneInteractionSize);//check x axis block only
            //bool isNotCollidingX = !Physics.CapsuleCast(transform.position, transform.position+Vector3.up*playerHeightOffset, playerSize, directionXAxis, Time.deltaTime * moveSpeed);//this is needed for collision handling

            if (isNotCollidingX) {
                return directionXAxis;//Player is free to move in x axis only
            }
            else
            {
                return directionZAxis;//if Player is blocked on x, then try z
                //if both are 0, then player is completely stuck in that direction.
            }

        }

        
    }

    public override Vector3 GetPlayerPosition()
    {
        return transform.position;
    }

    public bool IsPlayerOneMoving()
    {
        return isMoving;
    }
}
