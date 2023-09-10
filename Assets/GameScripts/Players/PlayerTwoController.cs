using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerTwoController : GenericPlayerControl
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

    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private float playerTwoInteractionSize = 0.5f; //needed for collision handling in Raycast function.
    //private int playerHeightOffset = 2;//needed for collision handling in CapsuleCast function.

    private float playerTwoInteractionDistance = 2f;
    
    private Vector3 currentPlayerTwoDirectionVector = Vector3.zero;

    
    //Awake will be called before Start()
    private void Awake()
    {
        if (instance == null) 
        { 
            instance = this;//start by setting the Singleton instance 
        }
        else
        {
            Debug.Log("Fatal Error: Cannot have a predefined instance of PlayerTwo");
        }
        instance.playerHealth = 15;
        instance.isActive = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        //choose a random starting direction to start moving
        currentPlayerTwoDirectionVector = AutoMovementHandler.GetRandomDirectionVector();

        /*Player Two movement should not be random. It should mmap out all the collectible items on the map
         and then go for the nearest one.*/
    }

    // Update is called once per frame
    private void Update()
    {   //Update() is inherited from MonoBehaviour. Called on each frame. Always specify the access modifier
        
        //Update speed if Faster or Slower key binding is pressed.
        currentPlayerTwoMovementSpeed = inputHandler.GetCurrentPlayerTwoMovementSpeed(currentPlayerTwoMovementSpeed, maxPlayerTwoMovementSpeed, minPlayerTwoMovementSpeed);

        HandleMovementWithCollision();
        HandleAllInteractions();
        //HandleAttackAction();
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
            if (rayCastHit.transform.TryGetComponent(out ExitKeyController exitKey))
            {
                Debug.Log(exitKey);
            }


        }

    }


    private void HandleMovementWithCollision()
    {
        
        //this will always be true for PlayerTwo since currentPlayerTwoDirection cannot be zero 
        isMoving = currentPlayerTwoDirectionVector != Vector3.zero;

        //needed for collision handling - if player movement is obstructed, try x or z axis movement only
        currentPlayerTwoDirectionVector = AutoMovementHandler.GetMovementReflectionDirectionAfterCollision(currentPlayerTwoDirectionVector, transform.position, playerTwoInteractionSize);

        //rotate the object to face the updated direction of movement
        transform.forward = Vector3.Slerp(transform.forward, currentPlayerTwoDirectionVector, Time.deltaTime * rotationSpeed);
        /*
         * Tip - use transform.lookAt function to have object change line of sight to a point. Useful for enemies facing P2
            transform.up or transform.right can work for 2D games to change direction.

            Slerp() function makes the direction change from prev pos smoother, by adding smoothing to not make the direction change instantaneous.
         */

        //move the object position in the direction 
        transform.position += currentPlayerTwoDirectionVector * Time.deltaTime * currentPlayerTwoMovementSpeed;
        /*transform holds the position of the GameObj, apparently
        transform.position is a 3D vector.
        Time.deltaTime ensures that perceived change in position is independent of system framerate.
        Time.deltaTime returns the timelapse between 2 frames. Very small number.*/
    }

    public void RespondToEnemyHunt(object sender, System.EventArgs e)
    {
        Debug.Log("PlayerTwo responding to Enemy Event.");
    }

    public void EvadeEnemyPosition(Vector3 EnemyPosition)
    {
        //Debug.Log("PlayerTwo evading Enemy Position " + EnemyPosition);
        Vector3 enemyEvasionPlayerTwoDirectionVector = (transform.position - EnemyPosition).normalized;

        //separate enemy Evasion vector is created to ensure that PlayerTwo doesn't break through walls
        currentPlayerTwoDirectionVector = AutoMovementHandler.GetMovementReflectionDirectionAfterCollision(enemyEvasionPlayerTwoDirectionVector, transform.position, playerTwoInteractionSize);
        transform.position += currentPlayerTwoDirectionVector * Time.deltaTime * maxPlayerTwoMovementSpeed;

    }

    public bool IsPlayerTwoMoving()
    {
        return isMoving;
    }

    public Vector3 GetPlayerTwoLocation()
    {
        return transform.position;
    }
}
