using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*this traverser has 2 goals:
1. Traverse Maze one to look for the Key
2. Traverse Maze the same way again to look for the exit.
 */
public class AutoMazeTraverser : MonoBehaviour
{

    private static AutoMazeTraverser instance;
    public static AutoMazeTraverser Instance
    {
        get { return instance; }
        private set { instance = value; }
    }

    private Vector3 currentPosition = Vector3.zero;//capture starting position of Player2
    private MazeCell[,] levelMazeReference;

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

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLevelMazeReference(MazeCell[,] gameMaze)
    {
        instance.levelMazeReference = gameMaze;
    }
}
