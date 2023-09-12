using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*this traverser has 2 goals:
1. Traverse Maze one to look for the Key
2. Traverse Maze the same way again to look for the exit.
 */
public class RecursiveMazeTraverser : MonoBehaviour
{

    private static RecursiveMazeTraverser instance;
    public static RecursiveMazeTraverser Instance
    {
        get { return instance; }
        private set { instance = value; }
    }

    //private Vector3 currentPosition = Vector3.zero;//capture starting position of Player2
    private MazeCell[,] levelMazeReference;
    
    //booleans related to playerTwoMotion
    private bool hasPlayerTwoStartedMazeTraversal = false;
    /*Unlike MazeBuild Logic manager, we cannot run a while loop here.
     This class will only return a direction which PlayerTwo has to face.
    This bool will tell us if PlayerTwo has hit the center of the nearest, unobstructed maze cell,
    and can MazeTraverser now dictate PlayerTwo's direction?*/



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;//start by setting the Singleton instance 
        }
        else
        {
            Debug.LogError("Fatal Error: Cannot have a predefined instance of AutoMazeTraverser");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //update current Position with PlayerTwo's current position.
        levelMazeReference = LevelBuilder.Instance.GetLevelMaze();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetNearestMazeCellCenterToStart()
    {
        if (levelMazeReference == null)
        {
            Debug.LogError("Maze Reference is Null in Maze Traverser. Cannot find starting point.");
            return;//nothing to check if the Maze itself is null
        }
        float closestCellCenterDistance = Mathf.Infinity;//using min number algo to find closest point


    }

    public void GetNextCellCenterToVisit()
    {
        if (levelMazeReference == null)
        {
            Debug.LogError("Maze Reference is Null in Maze Traverser. Cannot find next cell for PlayerTwo.");
            return;//nothing to check if the Maze itself is null
        }
    }

    /*this is to be used if PlayerTwo has traversed the complete maze, found the exit BEFORE finding the key, and now needs to relook for the exit by retraversing the entire map.
     * If player Two finds the key AND THEN finds the exit, this will never be called.*/
    private void ResetPlayerTwoProgress()
    {
        
    }

    //This will be called 
    public void GameCompleted()
    {
        
    }

    public void SetLevelMazeReference(MazeCell[,] gameMaze)
    {
        instance.levelMazeReference = gameMaze;
    }
}
