using System;
using System.Collections.Generic;
using UnityEngine;

//This class uses Depth-First-Search or Recursive BackTracking to Construct a maze
//Difficulty can vary based on width of this maze

//Each node corresponds to a bit-square in a maze
[Flags]
public enum cellWallState
{
    //0000 = No walls on this node
    //1111 = All walls on this node are untouched - Up,Dn,Rt,Lf,
    Left = 1, Right = 2, Bottom = 4, Top = 8,
    //0001, 0010, 0100,1000

    VisitedByMazeBuilder = 128, //tracks which nodes have been touched. 128 to give is a bit away from all the others 1000 1111
    //this indicated "Visited" by Maze builder, not by PlayerTwo

    VisitedByPlayerTwo = 64
    //this indicated "Visited" by PlayerTwo. This will be reset once, between finding key and finding exit.
    //63 to give is a bit away from all the others 0100 1111
}
/*Flags attribute allows us to have multiple true states for this Enum, not just one, using | operator
//eg. newWallState = nodeWallState.Left | nodeWallState.Right; equiv to 0011

Add a state:- newWallState |= nodeWallState.Up; 1011
Remove a state:- newWallState &= ~nodeWallState.Right; 0101
Check state:- HasFlags(nodeWallState.Right)

*/

public struct MazeCell
{
    public cellWallState cellWallState;
    public Vector3 cellPositionOnMap;
    public PositionInMaze indexInMazeCellArray;
}

public struct PositionInMaze
{
    public int x;//track our current position in the recursive backtrack, using this structure
    public int z;
}

//we should consider removing this struct. MazeCell struct covers this and then some.
public struct MazeCellNeighbour
{
    public PositionInMaze indexInMazeCellArray; //we traverse from our pos to neighbour pos, and break the common wall
    public cellWallState cellWallState;
}

/*This class uses Depth-First-Search or Recursive BackTracking to 
 * first create a maze from a cell-grid, and then get PlayerTwo to Solve the same maze 
 * with the same technique*/

public static class MazeBuildLogicManager 
{

    //this will return a list of 4 neighbours at max
    private static List<MazeCellNeighbour> GetUnvisitedNeighbours(PositionInMaze P, MazeCell[,] mazeInProgress, uint numCells)
    {
        // we are assuming that the maze will be square at all times, for simplicity.
 
        List<MazeCellNeighbour> unvisitedNeighbourList = new List<MazeCellNeighbour>();

        if (P.x > 0)//check left side neighbour
        {
            if (!mazeInProgress[P.x -1, P.z].cellWallState.HasFlag(cellWallState.VisitedByMazeBuilder))
            {
                unvisitedNeighbourList.Add(new MazeCellNeighbour
                {
                    indexInMazeCellArray =  new PositionInMaze
                    {
                        x = P.x -1,
                        z = P.z
                    },
                    cellWallState = cellWallState.Left //because We checked the left side
                });
            }
        }

        if (P.x < numCells -1)//check right side neighbour
        {
            if (!mazeInProgress[P.x + 1, P.z].cellWallState.HasFlag(cellWallState.VisitedByMazeBuilder))
            {
                unvisitedNeighbourList.Add(new MazeCellNeighbour
                {
                    indexInMazeCellArray = new PositionInMaze
                    {
                        x = P.x + 1,
                        z = P.z
                    },
                    cellWallState = cellWallState.Right //because We checked the right side
                });
            }
        }

        if (P.z > 0)//check bottom side neighbour
        {
            if (!mazeInProgress[P.x, P.z - 1].cellWallState.HasFlag(cellWallState.VisitedByMazeBuilder))
            {
                unvisitedNeighbourList.Add(new MazeCellNeighbour
                {
                    indexInMazeCellArray = new PositionInMaze
                    {
                        x = P.x,
                        z = P.z - 1,
                    },
                    cellWallState = cellWallState.Bottom 
                });
            }
        }

        if (P.z < numCells -1)//check top side neighbour
        {
            if (!mazeInProgress[P.x, P.z + 1].cellWallState.HasFlag(cellWallState.VisitedByMazeBuilder))
            {
                unvisitedNeighbourList.Add(new MazeCellNeighbour
                {
                    indexInMazeCellArray = new PositionInMaze
                    {
                        x = P.x,
                        z = P.z + 1,
                    },
                    cellWallState = cellWallState.Top
                });
            }
        }

        return unvisitedNeighbourList;

    }


    public static MazeCell[,] ApplyRecursiveBacktrackerToMakeMaze(uint totalMazeSideLength, uint numCellsOnSide, float singleCellSideLength)
    {
        MazeCell[,] finalMaze = CreateStartingGrid(totalMazeSideLength, numCellsOnSide, singleCellSideLength);//initialize with fully walled grid.

        Stack<PositionInMaze> lastVisitedPositionStack = new Stack<PositionInMaze>();//Stack = LIFO Queue

        //Step 1 - choose a random position
        int randomStartingX = MathFunctions.GetRandomIntInRange(0, numCellsOnSide);
        int randomStartingZ = MathFunctions.GetRandomIntInRange(0, numCellsOnSide);
        PositionInMaze randomStartingPosition = new PositionInMaze {x=randomStartingX,z=randomStartingZ};
        Debug.Log("Building the maze with Recursive Backtracker, starting at "+randomStartingX+","+randomStartingZ);

        finalMaze[randomStartingPosition.x, randomStartingPosition.z].cellWallState |= cellWallState.VisitedByMazeBuilder;//starting position is now visited -> 1000 1111 on the Enum

        lastVisitedPositionStack.Push(randomStartingPosition);

        uint debugLoopIteratorCount = 0;//creating this to prevent this while loop from going on infinitely.
        uint maxLoopCount = 1000;

        //Step 2 - iterate over positionStack till it is empty.

        while(lastVisitedPositionStack.Count > 0 && debugLoopIteratorCount < maxLoopCount)
        {
            debugLoopIteratorCount++;//break after a set number of iterations. Prevent infinite loop

            PositionInMaze currentPositionInMaze = lastVisitedPositionStack.Pop();
            List<MazeCellNeighbour> unvisitedCurrentCellNeighbours = GetUnvisitedNeighbours(currentPositionInMaze,finalMaze,numCellsOnSide);

            if(unvisitedCurrentCellNeighbours.Count > 0 )
            {
                //we have not reached a dead-end yet. Put currentPosition back in the Stack
                lastVisitedPositionStack.Push(currentPositionInMaze);

                int randomIndexOfNeighbour = MathFunctions.GetRandomIntInRange(0, unvisitedCurrentCellNeighbours.Count);
                MazeCellNeighbour randomNeighbour = unvisitedCurrentCellNeighbours[randomIndexOfNeighbour];

                PositionInMaze randomNeighbourPosition = randomNeighbour.indexInMazeCellArray;

                //remove shared walls between current position and selected Neighbour.

                //remove neighbour's wall
                finalMaze[randomNeighbourPosition.x, randomNeighbourPosition.z].cellWallState &= ~GetNeighboursOppositeWall(randomNeighbour.cellWallState);
                //remove our wall
                finalMaze[currentPositionInMaze.x, currentPositionInMaze.z].cellWallState &= ~randomNeighbour.cellWallState;

                //Step 3 - Mark neighbour as visited and Push visited Neighbour's position on the stack.
                finalMaze[randomNeighbourPosition.x, randomNeighbourPosition.z].cellWallState |= cellWallState.VisitedByMazeBuilder;
                lastVisitedPositionStack.Push(randomNeighbourPosition);
            }
            else
            {
                /* 
                 * This is the elegant part, because this is where the backtracking actually happens.
                 * If a cell has no unvisited neighbours, then its position is popped and the cell that was visited before this one is checked.
                 * If that one has any unvisited neighbour, then it gets added to the stack
                 */
            }
        }

        Debug.Log("Maze generation completed in " + debugLoopIteratorCount + " iterations");
        return finalMaze;
    }


    private static cellWallState GetNeighboursOppositeWall(cellWallState currentPositionWall)
    {
        switch(currentPositionWall)
        {
            case cellWallState.Right: return cellWallState.Left;
            case cellWallState.Left: return cellWallState.Right;
            case cellWallState.Top: return cellWallState.Bottom;
            case cellWallState.Bottom:  return cellWallState.Top;
            default: return cellWallState.Left;//this is trivial because one of the above states will be passed as arg anyway
        }
    }

    //return a 2D array which corresponds to the maze 
    private static MazeCell[,] CreateStartingGrid(uint totalMazeSideLength, uint numCells, float singleCellSideLength)
    {
        float totalOffset = (float)totalMazeSideLength / 2; //center should be (0,0,0), hence shift the whole thing to (-offset, -offset)
        //numCells indicates the number of cells on width and height; lesser numCells means bigger cells.
        MazeCell[,] gameMaze = new MazeCell[numCells, numCells];
        cellWallState initialCellState = cellWallState.Top | cellWallState.Bottom | cellWallState.Right | cellWallState.Left;//1111
        //no Visited states have been added here.
        for (int i = 0; i < numCells; i++)
        {
            for (int j = 0; j < numCells; j++)
            {
                //construct all walls of the cell, to have a solid matrix at first.
                gameMaze[i, j].cellWallState = initialCellState;
                gameMaze[i, j].cellPositionOnMap = new Vector3(-totalOffset + i * singleCellSideLength, 0, -totalOffset + j * singleCellSideLength);

                //this is important as it captures the array index of the Maze within the object itself. 
                gameMaze[i, j].indexInMazeCellArray = new PositionInMaze { x = i, z = j };
            }
        }

        return gameMaze;
    }
}
