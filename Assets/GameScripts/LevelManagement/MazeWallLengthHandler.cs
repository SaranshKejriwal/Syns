using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*this class checks the number of Cells and the total length of the play area, 
 * to compute the total length of the wall for each cell*/
public class MazeWallLengthHandler : MonoBehaviour
{
    float mazeCellLength;

    // Start is called before the first frame update
    void Start()
    {
        mazeCellLength = LevelBuilder.Instance.GetCellSideLength();
        //this gets the length of each cell of the maze, based on number of cells of LevelBuilder
        
        transform.localScale = new Vector3(mazeCellLength/2, 1, 1);//y and z are managed by Prefab, not this class
        //Scale is being corrected in Start() because MazeBuilder will create its instances during Awake().

        //mazeCellLength/2 is taken to increase scale on either side, by half of cell Length.
    }

    private void Update()
    {
        //this can be used to increase height of the wall to hide the contents behind it.        
        //increaseWallHeightToHideBehind();
    }

    private void increaseWallHeightToHideBehind()
    {
        Vector3 playerOnePosition = PlayerOneController.Instance.GetPlayerPosition();
        //compare only z axis. If Wall is ahead of PlayerOne, make it larger.
        if (transform.position.z > playerOnePosition.z)
        {
            transform.localScale = new Vector3(mazeCellLength / 2, 3 * mazeCellLength, 1);//y and z are managed by Prefab, not this class
        }
        else
        {
            transform.localScale = new Vector3(mazeCellLength / 2, 1, 1);//y and z are managed by Prefab, not this class
        }
    }

}
