using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    private static MazeRenderer instance;
    public static MazeRenderer Instance
    //this instance "Property" will be tracked by ALL enemies, while keeping actual PlayerTwo object private
    {
        get { return instance; }//not very different from getters and setters
        private set { instance = value; }//we do not want any other object to modify PlayerTwo entirely.
    }

    [SerializeField] private Transform mazeWallLogicPrefab = null;//needs wall prefab to create instances of the walls


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;//start by setting the Singleton instance 
        }
        else
        {
            Debug.Log("Fatal Error: Cannot have a predefined instance of Maze Renderer");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DrawMazeOnGame(cellWallState[,] gameMaze, uint totalMazeSideLength, uint numCellsOnSide, float singleCellSideLength)
    {
        if (gameMaze == null)
        {
            Debug.Log("Error: Maze not created. Nothing to draw.");
            return;
        }
        float totalOffset = (float)totalMazeSideLength / 2; //center should be (0,0,0), hence shift the whole thing to (-offset, -offset)
        for (int i = 0; i < numCellsOnSide; i++)
        {
            for (int j = 0; j < numCellsOnSide; j++)
            {
                cellWallState cell = gameMaze[i, j];
                //center of the maze should be (0,0,0), hence starting in -x, -z plane
                Vector3 cellPosition = new Vector3(-totalOffset + i * singleCellSideLength, 0, -totalOffset + j * singleCellSideLength);
                //Multiplying by cellSideLength ensure that position accounts for the length offset.
                //Note -> numCellsOnSide is uint, so numCellsOnSide/2 = 0 is numCellsOnSide=1

                DrawTopWall(cell, cellPosition, singleCellSideLength);
                DrawLeftWall(cell, cellPosition, singleCellSideLength);
                DrawRightWall(cell, cellPosition, singleCellSideLength);//redundant, because top and left should suffice, but the if-statements below weren't working
                DrawBottomWall(cell, cellPosition, singleCellSideLength);//redundant, because top and left should suffice, but the if-statements below weren't working
                //Note - This iteration starts from bottom-left to top right, with i controlling x


            }
        }

    }

    //Draws only top walls
    private void DrawTopWall(cellWallState cell, Vector3 cellPosition, float singleCellSideLength)
    {
        if (cell.HasFlag(cellWallState.Top))
        {

            Transform topWall = Instantiate(mazeWallLogicPrefab, transform);
            topWall.localPosition = cellPosition + new Vector3(0, 0, singleCellSideLength / 2);
            //top wall is higher than center of cell, by half the cell side

        }
    }

    //Draws only bottom walls
    private void DrawBottomWall(cellWallState cell, Vector3 cellPosition, float singleCellSideLength)
    {
        if (cell.HasFlag(cellWallState.Bottom))
        {
            //no need to check enum because this is drawn for maze edge only
            Transform bottomWall = Instantiate(mazeWallLogicPrefab, transform);
            bottomWall.localPosition = cellPosition + new Vector3(0, 0, -singleCellSideLength / 2);
            //bottom wall is lower than center of cell, by half the cell side
        }
    }

    //Draws only left walls
    private void DrawLeftWall(cellWallState cell, Vector3 cellPosition, float singleCellSideLength)
    {
        if (cell.HasFlag(cellWallState.Left))
        {
            Transform leftWall = Instantiate(mazeWallLogicPrefab, transform);
            leftWall.localPosition = cellPosition + new Vector3(-singleCellSideLength / 2, 0, 0);
            //left wall is to the left of center of cell, by half the cell side

            //rotate the leftWall by 90 degrees
            leftWall.eulerAngles = new Vector3(0, 90, 0);//rotate 90 degrees on y axis only

        }
    }

    //Draws only right walls
    private void DrawRightWall(cellWallState cell, Vector3 cellPosition, float singleCellSideLength)
    {
        if (cell.HasFlag(cellWallState.Right))
        {
            Transform rightWall = Instantiate(mazeWallLogicPrefab, transform);
            rightWall.localPosition = cellPosition + new Vector3(singleCellSideLength / 2, 0, 0);
            //right wall is to the right of center of cell, by half the cell side

            //rotate the leftWall by 90 degrees
            rightWall.eulerAngles = new Vector3(0, 90, 0);//rotate 90 degrees on y axis only

        }
    }
}
