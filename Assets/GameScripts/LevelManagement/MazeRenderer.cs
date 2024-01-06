using System;
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

    //If a new maze is being rendered, it is important to destroy the walls of the previous maze, else they overlap and create closed boxes.
    public event EventHandler OnNewMazeRender;
    //this event will be subscribed by the individual wall objects. All previous walls will destroy themselves before new walls are created.

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;//start by setting the Singleton instance 
        }
        else
        {
            Debug.LogError("Fatal Error: Cannot have a predefined instance of Maze Renderer");
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

    public void DrawMazeOnGame(LevelType levelType, MazeCell[,] gameMaze, uint totalMazeSideLength, uint numCellsOnSide, float singleCellSideLength)
    {
        if (gameMaze == null)
        {
            Debug.Log("Error: Maze not created. Nothing to draw.");
            return;
        }

        //fire event to destroy all previous Prefab walls
        if(OnNewMazeRender != null)
        {
            Debug.Log("Destroying previous maze walls...");
            OnNewMazeRender(this, EventArgs.Empty);
        }

        for (int i = 0; i < numCellsOnSide; i++)
        {
            for (int j = 0; j < numCellsOnSide; j++)
            {
                cellWallState mazeCell = gameMaze[i, j].cellWallState;
                //center of the maze should be (0,0,0), hence starting in -x, -z plane
                //(0,0) is bottom left corner,
                //(1,0) is horizontally rightwards,
                //(0,1) is vertically depthwards
                //Render happens in vertical strips, from B->T then, from L->R

                Vector3 mazeCellPosition = gameMaze[i, j].cellPositionOnMap;
                //Vector3 mazeCellPosition = new Vector3(-totalOffset + i * singleCellSideLength, 0, -totalOffset + j * singleCellSideLength);
                //Multiplying by cellSideLength ensure that position accounts for the length offset.
                //Note -> numCellsOnSide is uint, so numCellsOnSide/2 = 0 is numCellsOnSide=1

                //This wall render logic below is exactly in line with the logic for printing the maze in Debug log.
                DrawBottomWall(mazeCell, mazeCellPosition, singleCellSideLength);
                DrawLeftWall(mazeCell, mazeCellPosition, singleCellSideLength);//left wall of 1 cell is right wall of another cell.

                if(i == numCellsOnSide - 1)
                {
                    //Only the rightmost column needs explicit Right-side wall
                    DrawRightWall(mazeCell, mazeCellPosition, singleCellSideLength);

                }
                if(j == numCellsOnSide - 1)
                {
                    //Only the topmost row needs explicit top wall.
                    DrawTopWall(mazeCell, mazeCellPosition, singleCellSideLength);
                }

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
