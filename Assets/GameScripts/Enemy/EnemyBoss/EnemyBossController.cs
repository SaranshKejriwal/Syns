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
        instance.enemyWalkingMovementSpeed = 0;
        instance.enemyHuntingMovementSpeed = 0;
        instance.enemyHealth = 75;
        instance.attackDamage = 10;
    }

    // Start is called before the first frame update
    void Start()
    {

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
