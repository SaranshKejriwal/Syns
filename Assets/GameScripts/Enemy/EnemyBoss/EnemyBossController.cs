using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossController : GenericEnemyController
{
    private static EnemyBossController instance;
    public static EnemyBossController Instance
    {
        /*Enemy Boss will be a singleton. It also controls EnemySpawn Handler.*/
        get { return instance; }
        private set { instance = value; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;//start by setting the Singleton instance 
        }
        else
        {
            Debug.LogError("Fatal Error: Cannot have a predefined instance of Enemy Boss");
        }
        instance.currentEnemyState = enemyStates.isStanding;//Boss should not be moving
        instance.defaultEnemyState = enemyStates.isStanding;
        instance.enemyWalkingMovementSpeed = 0;
        instance.enemyHuntingMovementSpeed = 0;
        instance.enemyHealth = 75;
        instance.attackRadius = 4f;
        instance.attackDamage = 10;
        currentEnemyMovementDirection = Vector3.zero; //because Boss doesn't move
    }
    public override void UpdateEnemyRadii()
    {
        //this function will update both the radius-lookup dictionaries for each Enemy object.
        enemyDetectionRadiusReference = new Dictionary<GenericPlayerController, float>()
        {
            { PlayerOneController.Instance, 8f }, //boss can detect both players
            { PlayerTwoController.Instance, 8f } //detection radius is equal to attack radius deliberately.
        };

        //All values will be 0 for generic class
    }
    // Start is called before the first frame update
    void Start()
    {
        instance.UpdateEnemyRadii();
        //To Spawn - Find a mazeCell that is in a dead-end for spawning
        //We don't want to see the butt of the Boss, to the cell should have 1 opening except the top one.
        //Dead-end cell should be at the very edge of the level
        MazeCell containerCell = LevelBuilder.Instance.GetDeadEndCellForBossSpawn();
        transform.position = containerCell.cellPositionOnMap;

        //Face the opening of the cell only
        transform.eulerAngles = new Vector3(0, GetRotationAngleInCell(containerCell), 0);

        //scale down to not overflow from the cell.

    }

    // Update is called once per frame
    void Update()
    {
        //the order of players is important. Boss will first check PlayerTwo, and then focus on PlayerOne because PlayerTwo will evade anyway
        base.ReactToPlayer(PlayerTwoController.Instance);
        base.ReactToPlayer(PlayerOneController.Instance);
    }


    private int GetRotationAngleInCell(MazeCell cell)
    {
        if (!cell.cellWallState.HasFlag(cellWallState.Top))
        {
            Debug.LogError("Boss Container Cell should not open at the top");
            return 0;
        }
        if (!cell.cellWallState.HasFlag(cellWallState.Bottom))
        {
            return 180;
        }
        if (!cell.cellWallState.HasFlag(cellWallState.Left))
        {
            return 270;
        }
        if (!cell.cellWallState.HasFlag(cellWallState.Right))
        {
            return 90;
        }

        return 0;
    }

}
