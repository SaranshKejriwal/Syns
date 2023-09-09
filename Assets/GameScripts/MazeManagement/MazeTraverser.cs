using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

/*This class uses Depth-First-Search or Recursive BackTracking to 
 * first create a maze from a cell-grid, and then get PlayerTwo to Solve the same maze 
 * with the same technique*/

public static class MazeTraverser 
{
    public static cellWallState[,] CreateMazeFromGrid(cellWallState[,] fullCellGrid, uint numCells)
    {
        cellWallState[,] finalMaze = new cellWallState[numCells, numCells]; 
        finalMaze = fullCellGrid;//initialize with fully walled grid.


        return finalMaze;
    }

    //this will return a list of 4 neighbours at max
    private static List<MazeCellNeighbour> GetUnvisitedNeighbours(PositionInMaze P, cellWallState[,] maze, uint numCells)
    {
        // we are assuming that the maze will be square at all times, for simplicity.
 
        List<MazeCellNeighbour> unvisitedNeighbourList = new List<MazeCellNeighbour>();

        if (P.x > 0)//left side
        {
            if (!maze[P.x -1, P.z].HasFlag(cellWallState.Visited))
            {
                unvisitedNeighbourList.Add(new MazeCellNeighbour
                {
                    neighbourPosition =  new PositionInMaze
                    {
                        x = P.x -1,
                        z = P.z
                    },
                    wallSharedWithNeighbour = cellWallState.Left //because We checked the left side
                });
            }
        }

        if (P.x < numCells -1)//right side
        {
            if (!maze[P.x + 1, P.z].HasFlag(cellWallState.Visited))
            {
                unvisitedNeighbourList.Add(new MazeCellNeighbour
                {
                    neighbourPosition = new PositionInMaze
                    {
                        x = P.x + 1,
                        z = P.z
                    },
                    wallSharedWithNeighbour = cellWallState.Right //because We checked the left side
                });
            }
        }

        if (P.z > 0)//bottom side
        {
            if (!maze[P.x, P.z - 1].HasFlag(cellWallState.Visited))
            {
                unvisitedNeighbourList.Add(new MazeCellNeighbour
                {
                    neighbourPosition = new PositionInMaze
                    {
                        x = P.x,
                        z = P.z - 1,
                    },
                    wallSharedWithNeighbour = cellWallState.Bottom 
                });
            }
        }

        if (P.z < numCells -1)//top side
        {
            if (!maze[P.x, P.z + 1].HasFlag(cellWallState.Visited))
            {
                unvisitedNeighbourList.Add(new MazeCellNeighbour
                {
                    neighbourPosition = new PositionInMaze
                    {
                        x = P.x,
                        z = P.z + 1,
                    },
                    wallSharedWithNeighbour = cellWallState.Top
                });
            }
        }

        return unvisitedNeighbourList;

    }


    public static cellWallState[,] ApplyRecursiveBacktracker(cellWallState[,] initialFullGrid, uint numCells)
    {
        cellWallState[,] finalMaze = new cellWallState[numCells, numCells];
        finalMaze = initialFullGrid;//initialize with fully walled grid.

        Stack<PositionInMaze> visitedPositionStack = new Stack<PositionInMaze>();//Stack = LIFO Queue

        //Step 1 - choose a random position
        int randomStartingX = (int)Random.Range(0, numCells);
        int randomStartingZ = (int)Random.Range(0, numCells);
        PositionInMaze randomStartingPosition = new PositionInMaze {x=randomStartingX,z=randomStartingZ};
        Debug.Log("Building the maze, starting at "+randomStartingX+","+randomStartingZ);

        finalMaze[randomStartingPosition.x, randomStartingPosition.z] |= cellWallState.Visited;//starting position is now visited -> 1000 1111 on the Enum

        visitedPositionStack.Push(randomStartingPosition);

        uint debugIteratorCount = 0;//creating this to prevent this while loop from going on infinitely.

        //Step 2 - iterate over positionStack till it is empty.

        while(visitedPositionStack.Count > 0 && debugIteratorCount <10000000)
        {
            debugIteratorCount++;//break after a set number of iterations. Prevent infinite loop

            PositionInMaze currentPositionInStack = visitedPositionStack.Pop();
            List<MazeCellNeighbour> unvisitedCurrentCelNeighbours = GetUnvisitedNeighbours(currentPositionInStack,finalMaze,numCells);

            if(unvisitedCurrentCelNeighbours.Count > 0 )
            {
                //we have not reached a dead-end yet. Put currentPosition back in the Stack
                visitedPositionStack.Push(currentPositionInStack);

                int randomIndexOfNeighbour = (int)Random.Range(0, unvisitedCurrentCelNeighbours.Count);
                MazeCellNeighbour randomNeighbour = unvisitedCurrentCelNeighbours[randomIndexOfNeighbour];

                PositionInMaze randomNeighbourPosition = randomNeighbour.neighbourPosition;

                //remove shared walls between current position and selected Neighbour.

                //remove neighbour's wall
                finalMaze[randomNeighbourPosition.x, randomNeighbourPosition.z] &= ~GetNeighboursOppositeWall(randomNeighbour.wallSharedWithNeighbour);
                //remove our wall
                finalMaze[currentPositionInStack.x, currentPositionInStack.z] &= ~randomNeighbour.wallSharedWithNeighbour;

                //Step 3 - Mark neighbour as visited and Push visited Neighbour's position on the stack.
                finalMaze[currentPositionInStack.x, currentPositionInStack.z] |= cellWallState.Visited;
                visitedPositionStack.Push(randomNeighbourPosition);
            }
        }

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


}
