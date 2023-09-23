using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*this traverser has 2 goals:
1. Traverse Maze one to look for the Key
2. Traverse Maze the same way again to look for the exit.
 */

/*Unlike MazeBuild Logic manager, we cannot run a while loop here.
     This class will only return a direction which PlayerTwo has to face.*/
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
    private uint numCellsOnMazeSide = 5;
    private Stack<MazeCell> mazeCellLastVisitedByPlayerTwoStack;
    


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
        numCellsOnMazeSide = LevelBuilder.Instance.GetMazeNumCellsOnSide();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //this method will be called only once at the start, to get PlayerTwo to closest Maze Cell
    //This is computationally heavier than most
    public Vector3 GetStartingCellCenter()
    {
        //this function is called first to initialise this stack
        mazeCellLastVisitedByPlayerTwoStack = new Stack<MazeCell>();//initialize stack here just before PlayerTwo calls this method.

        if (levelMazeReference == null)
        {
            Debug.LogError("Maze Reference is Null in Maze Traverser. Cannot find starting point.");
            return Vector3.zero;//nothing to check if the Maze itself is null
        }
        MazeCell startingCell = LevelBuilder.Instance.GetGameStartingCell();

        //add this starting cell as visited.
        levelMazeReference[startingCell.indexInMazeCellArray.x, startingCell.indexInMazeCellArray.z].cellWallState |= cellWallState.VisitedByPlayerTwo;
        mazeCellLastVisitedByPlayerTwoStack.Push(levelMazeReference[startingCell.indexInMazeCellArray.x, startingCell.indexInMazeCellArray.z]);

        Debug.Log("Maze Traversal started at " + startingCell.cellPositionOnMap);
        return startingCell.cellPositionOnMap;

    }

    public Vector3 GetNextCellCenterToVisit(Vector3 currentPlayerTwoPosition)
    {
        if (PlayerTwoController.Instance.CanEnterExitDoorInVicinity())
            //this is better than checking if ExitDoor collected flag is true, because this will only work if PlayerTwo is in the same cell as Exit Door.
        {            
            return ExitDoorController.Instance.GetExitDoorPosition();
            //This means that PlayerTwo should only go to Exit Door if it has the key and is in range of door.
        }

        bool nextCellCenterFound = false;//this will be used to break the while loop that pops the stack
        Vector3 nextCellPositionToVisit = Vector3.one;//logic will never return (1,1,1)

        int loopIteratorCount = 0;//this is to ensure that we don't have an infinite while loop
        int maxLoopCount = 100;
        if (levelMazeReference == null)
        {
            Debug.LogError("Maze Reference is Null in Maze Traverser. Cannot find next cell for PlayerTwo.");
            return currentPlayerTwoPosition;//nothing to check if the Maze itself is null
        }
        if (mazeCellLastVisitedByPlayerTwoStack.Count == 0 && PlayerTwoController.Instance.HasCollectedExitKey())
        {
            Debug.Log("PlayerTwo reached the end of the maze. Resetting progress");
            ResetPlayerTwoProgress();
            
            //this means that playerTwo found the Exit Door before finding the Exit key, and then traversed the entire maze. 
            //it now needs to traverse the maze again, to find the exit.
            
        }
        Debug.Log("Traversal Stack Count: " + mazeCellLastVisitedByPlayerTwoStack.Count);
        while (mazeCellLastVisitedByPlayerTwoStack.Count > 0 && !nextCellCenterFound && loopIteratorCount <= maxLoopCount)
        {
            loopIteratorCount++;//ensures no infinite looping

            MazeCell lastVisitedCell = mazeCellLastVisitedByPlayerTwoStack.Pop();
            List<MazeCell> unvisitedAccessibleCellNeighbours = GetUnvisitedAccessibleNeighboursOfCell(lastVisitedCell,numCellsOnMazeSide);
            
            if(unvisitedAccessibleCellNeighbours.Count > 0)//there is an unvisited neighbour that can be accessed
            {
                mazeCellLastVisitedByPlayerTwoStack.Push(lastVisitedCell);//haven't yet reached a dead-end. We may come back
                int randomIndexOfNeighbour = MathFunctions.GetRandomIntInRange(0, unvisitedAccessibleCellNeighbours.Count);
                MazeCell randomNeighbour = unvisitedAccessibleCellNeighbours[randomIndexOfNeighbour];

                //mark randomNeighbour as visited, add to stack and return its position
                levelMazeReference[randomNeighbour.indexInMazeCellArray.x, randomNeighbour.indexInMazeCellArray.z].cellWallState |= cellWallState.VisitedByPlayerTwo;
                mazeCellLastVisitedByPlayerTwoStack.Push(randomNeighbour);
                nextCellPositionToVisit = randomNeighbour.cellPositionOnMap;
                nextCellCenterFound = true;
            }
            else if(mazeCellLastVisitedByPlayerTwoStack.Count>0)
            {
                //PlayerTwo has hit a dead end and there are no unvisited neighbours. Needs to backtrack

                //Peek will not be possible if Stack is empty.
                nextCellPositionToVisit = mazeCellLastVisitedByPlayerTwoStack.Peek().cellPositionOnMap;
                Debug.Log("No unvisited neighbours. Backtracking required.");
                //backtrack to prev neighbour without popping the stack.
                nextCellCenterFound = true;
            }
            else
            {
                Debug.Log("Stack Peek not possible. PlayerTwo reached the end of the maze. Resetting progress");
                ResetPlayerTwoProgress();
            }
        } 
        
        if(nextCellPositionToVisit == Vector3.one)
        {
            Debug.Log("Unable to find next cell position. Stopping PlayerTwo");
            return currentPlayerTwoPosition;
        }
        Debug.Log("PlayerTwo should go to next cell: " + nextCellPositionToVisit);
        return nextCellPositionToVisit;

    }


    public Vector3 GetNextCellCenterToEvadeEnemy(Vector3 currentPlayerTwoPosition, Vector3 enemyPosition)
    {
        if (PlayerTwoController.Instance.CanEnterExitDoorInVicinity())
        //this will only work if PlayerTwo is in the same cell as Exit Door and Exit Door is open.
        {
            return ExitDoorController.Instance.GetExitDoorPosition();
            //This means that PlayerTwo should only go to Exit Door if it has the key and is in range of door.
        }

        bool nextCellCenterFound = false;//this will be used to break the while loop that pops the stack
        Vector3 nextCellPositionToVisit = Vector3.one;//logic will never return (1,1,1)

        int loopIteratorCount = 0;//this is to ensure that we don't have an infinite while loop
        int maxLoopCount = 100;
        if (levelMazeReference == null)
        {
            Debug.LogError("Maze Reference is Null in Maze Traverser. Cannot find next cell for PlayerTwo Evasion.");
            return currentPlayerTwoPosition;//nothing to check if the Maze itself is null
        }
        if (mazeCellLastVisitedByPlayerTwoStack.Count == 0 && PlayerTwoController.Instance.HasCollectedExitKey())
        {
            Debug.Log("PlayerTwo reached the end of the maze while evading. Resetting progress");
            ResetPlayerTwoProgress();

            //this means that playerTwo found the Exit Door before finding the Exit key, and then traversed the entire maze. 
            //it now needs to traverse the maze again, to find the exit.

        }
        Debug.Log("Traversal Stack Count: " + mazeCellLastVisitedByPlayerTwoStack.Count);
        while (mazeCellLastVisitedByPlayerTwoStack.Count > 0 && !nextCellCenterFound && loopIteratorCount <= maxLoopCount)
        {
            loopIteratorCount++;//ensures no infinite looping

            MazeCell lastVisitedCell = mazeCellLastVisitedByPlayerTwoStack.Pop();
            List<MazeCell> allAccessibleCellNeighbours = GetAllAccessibleNeighboursOfCell(lastVisitedCell, numCellsOnMazeSide);
            if (allAccessibleCellNeighbours.Count == 1)
            {
                Debug.Log("PlayerTwo has reached a Dead end while Evading Enemy. Should it Stop?");
            }

            if (allAccessibleCellNeighbours.Count > 1)//there is more than 1 accessible neighbour, so we're not at a dead-end
            {
                mazeCellLastVisitedByPlayerTwoStack.Push(lastVisitedCell);//haven't yet reached a dead-end. We may come back

                //check all neighbours to see which one is farthest from enemy, and go to that cell.
                //if that cell was already visited, mark it as unvisited because we would need to retraverse it, and then push it to stack.

                MazeCell farthestNeighbour = new MazeCell();
                float currentMaxDistanceFromEnemy = 0f;//initialize this to run Max algo to get cell with largest distance
                
                foreach(MazeCell cell in allAccessibleCellNeighbours)
                {
                    //run a loop to check the cell that has the max distance from enemy
                    float cellDistanceFromEnemy = Vector3.Distance(cell.cellPositionOnMap, enemyPosition);
                    if(cellDistanceFromEnemy > currentMaxDistanceFromEnemy)
                    {
                        currentMaxDistanceFromEnemy=cellDistanceFromEnemy;
                        farthestNeighbour = cell;
                    }
                }

                //if farthest neighbour was already visited, clear that flag because PlayerTwo may have to retraverse that path. 
                if (levelMazeReference[farthestNeighbour.indexInMazeCellArray.x, farthestNeighbour.indexInMazeCellArray.z].cellWallState.HasFlag(cellWallState.VisitedByPlayerTwo)) 
                {
                    levelMazeReference[farthestNeighbour.indexInMazeCellArray.x, farthestNeighbour.indexInMazeCellArray.z].cellWallState &= ~cellWallState.VisitedByPlayerTwo;
                }


                //add farthest Neighbour to stack and return its position
                mazeCellLastVisitedByPlayerTwoStack.Push(farthestNeighbour);
                nextCellPositionToVisit = farthestNeighbour.cellPositionOnMap;
                nextCellCenterFound = true;

            }
            else if (mazeCellLastVisitedByPlayerTwoStack.Count > 0)
            {
                //PlayerTwo has hit a dead end and there are no unvisited neighbours.

                //Peek will not be possible if Stack is empty.
                nextCellPositionToVisit = mazeCellLastVisitedByPlayerTwoStack.Peek().cellPositionOnMap;//get previously visitedd neighbour without popping stack
                Debug.Log("No accessible neighbours. Backtracking required.");
                //backtrack to prev neighbour without popping the stack.
                nextCellCenterFound = true;
            }
            else
            {
                Debug.Log("Stack Peek not possible. PlayerTwo reached the end of the maze. Resetting progress");
                ResetPlayerTwoProgress();
            }
        }

        if (nextCellPositionToVisit == Vector3.one)
        {
            Debug.Log("Unable to find next cell position. Stopping PlayerTwo");
            return currentPlayerTwoPosition;
        }
        Debug.Log("PlayerTwo should escape to next cell: " + nextCellPositionToVisit);
        return nextCellPositionToVisit;

    }


    //this method will check the cell-wall-state of the received cell and add the neighbours which are not blocked
    private List<MazeCell> GetUnvisitedAccessibleNeighboursOfCell(MazeCell cell, uint numCells)
    {
        // we are assuming that the maze will be square at all times, for simplicity.

        List<MazeCell> unvisitedNeighbourList = new List<MazeCell>();

        if (cell.indexInMazeCellArray.x > 0)//means that we can check left side neighbour without going out of bounds
        {
            bool currentCellHasLeftWall = cell.cellWallState.HasFlag(cellWallState.Left);
            MazeCell leftSideNeighbourOfCurrentCell = levelMazeReference[cell.indexInMazeCellArray.x - 1, cell.indexInMazeCellArray.z];
            if (!currentCellHasLeftWall && !leftSideNeighbourOfCurrentCell.cellWallState.HasFlag(cellWallState.VisitedByPlayerTwo))//has no left wall and not visited
            {
                //left side neighbour is accessible and isn't visited.
                unvisitedNeighbourList.Add(leftSideNeighbourOfCurrentCell);
            }
        }

        if (cell.indexInMazeCellArray.x < numCells - 1)//means that we can check right side neighbour without going out of bounds
        {
            bool currentCellHasRightWall = cell.cellWallState.HasFlag(cellWallState.Right);
            MazeCell rightSideNeighbourOfCurrentCell = levelMazeReference[cell.indexInMazeCellArray.x + 1, cell.indexInMazeCellArray.z];
            if (!currentCellHasRightWall && !rightSideNeighbourOfCurrentCell.cellWallState.HasFlag(cellWallState.VisitedByPlayerTwo))//has no right wall and not visited
            {
                //right neighbour is accessible and isn't visited.
                unvisitedNeighbourList.Add(rightSideNeighbourOfCurrentCell);
            }
        }

        if (cell.indexInMazeCellArray.z > 0)//means that we can check bottom side neighbour without going out of bounds
        {
            bool currentCellHasBottomWall = cell.cellWallState.HasFlag(cellWallState.Bottom);
            MazeCell bottomSideNeighbourOfCurrentCell = levelMazeReference[cell.indexInMazeCellArray.x, cell.indexInMazeCellArray.z-1];
            if (!currentCellHasBottomWall && !bottomSideNeighbourOfCurrentCell.cellWallState.HasFlag(cellWallState.VisitedByPlayerTwo))//has no bottom wall and not visited
            {
                //bottom neighbour is accessible and isn't visited.
                unvisitedNeighbourList.Add(bottomSideNeighbourOfCurrentCell);
            }
        }

        if (cell.indexInMazeCellArray.z < numCells - 1)//means that we can check top side neighbour without going out of bounds
        {
            bool currentCellHasTopWall = cell.cellWallState.HasFlag(cellWallState.Top);
            MazeCell topSideNeighbourOfCurrentCell = levelMazeReference[cell.indexInMazeCellArray.x, cell.indexInMazeCellArray.z + 1];
            if (!currentCellHasTopWall && !topSideNeighbourOfCurrentCell.cellWallState.HasFlag(cellWallState.VisitedByPlayerTwo))//has no bottom wall and not visited
            {
                //bottom neighbour is accessible and isn't visited.
                unvisitedNeighbourList.Add(topSideNeighbourOfCurrentCell);
            }
        }

        return unvisitedNeighbourList;

    }


    //this method will be called while evading Enemy. It will check the cell-wall-state of the received cell
    //and add the neighbours which are not blocked without checking Visited flag
    private List<MazeCell> GetAllAccessibleNeighboursOfCell(MazeCell cell, uint numCells)
    {
        // we are assuming that the maze will be square at all times, for simplicity.

        List<MazeCell> accessibleNeighbourList = new List<MazeCell>();

        if (cell.indexInMazeCellArray.x > 0)//means that we can check left side neighbour without going out of bounds
        {
            bool currentCellHasLeftWall = cell.cellWallState.HasFlag(cellWallState.Left);
            MazeCell leftSideNeighbourOfCurrentCell = levelMazeReference[cell.indexInMazeCellArray.x - 1, cell.indexInMazeCellArray.z];
            if (!currentCellHasLeftWall)//has no left wall and not visited
            {
                //left side neighbour is accessible and isn't visited.
                accessibleNeighbourList.Add(leftSideNeighbourOfCurrentCell);
            }
        }

        if (cell.indexInMazeCellArray.x < numCells - 1)//means that we can check right side neighbour without going out of bounds
        {
            bool currentCellHasRightWall = cell.cellWallState.HasFlag(cellWallState.Right);
            MazeCell rightSideNeighbourOfCurrentCell = levelMazeReference[cell.indexInMazeCellArray.x + 1, cell.indexInMazeCellArray.z];
            if (!currentCellHasRightWall)//has no right wall and not visited
            {
                //right neighbour is accessible and isn't visited.
                accessibleNeighbourList.Add(rightSideNeighbourOfCurrentCell);
            }
        }

        if (cell.indexInMazeCellArray.z > 0)//means that we can check bottom side neighbour without going out of bounds
        {
            bool currentCellHasBottomWall = cell.cellWallState.HasFlag(cellWallState.Bottom);
            MazeCell bottomSideNeighbourOfCurrentCell = levelMazeReference[cell.indexInMazeCellArray.x, cell.indexInMazeCellArray.z - 1];
            if (!currentCellHasBottomWall)//has no bottom wall and not visited
            {
                //bottom neighbour is accessible and isn't visited.
                accessibleNeighbourList.Add(bottomSideNeighbourOfCurrentCell);
            }
        }

        if (cell.indexInMazeCellArray.z < numCells - 1)//means that we can check top side neighbour without going out of bounds
        {
            bool currentCellHasTopWall = cell.cellWallState.HasFlag(cellWallState.Top);
            MazeCell topSideNeighbourOfCurrentCell = levelMazeReference[cell.indexInMazeCellArray.x, cell.indexInMazeCellArray.z + 1];
            if (!currentCellHasTopWall)//has no bottom wall and not visited
            {
                //bottom neighbour is accessible and isn't visited.
                accessibleNeighbourList.Add(topSideNeighbourOfCurrentCell);
            }
        }
        return accessibleNeighbourList;
    }


    /*this is to be used if PlayerTwo has traversed the complete maze, found the exit BEFORE finding the key, and now needs to relook for the exit by retraversing the entire map.
     * If player Two finds the key AND THEN finds the exit, this will never be called.*/
    private void ResetPlayerTwoProgress()
    {
        //mark all next cells as unvisited??
        for (int i = 0;i<numCellsOnMazeSide;i++)
        {

        }
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
