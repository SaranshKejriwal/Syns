using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This static class can be used for automated movements of Player Two and Enemies only
public static class AutoMovementHandler 
{
    public static Vector3 GetRandomDirectionVector()
    {
        //Get any random direction in 0 to 360 degree angle from starting point
        Vector3 randomDirectionVector = new Vector3(MathFunctions.GetRandomFloatInRange(-1f, 1f), 0, MathFunctions.GetRandomFloatInRange(-1f, 1f));
           
        //run a while loop to ensure that you don't randomly get a zero vector from above step
        while (randomDirectionVector == Vector3.zero)
        {
            randomDirectionVector = new Vector3(MathFunctions.GetRandomFloatInRange(-1f, 1f), 0, MathFunctions.GetRandomFloatInRange(-1f, 1f));
        }
        return randomDirectionVector.normalized;
    }

    //use - compute reflection direction of automatically moving entity - playerTwo or Enemy, after obstruction.
    public static Vector3 GetMovementReflectionDirectionAfterCollision(Vector3 currentDirectionVector, Vector3 currentPositionVector, float objectInteractionSize)
    {
        //check if there is collision in X or Z
        bool isNotColliding = !Physics.Raycast(currentPositionVector, currentDirectionVector, objectInteractionSize);//this is needed for collision handling
        //bool isNotColliding = !Physics.CapsuleCast(transform.position, transform.position+Vector3.up*playerHeightOffset, playerSize, directionVector, Time.deltaTime * moveSpeed);//this is needed for collision handling

        if (isNotColliding)
        {
            return currentDirectionVector; //no collisions currently, can proceed in present direction
        }
        else
        {
            //attempt seprate x-direction or z-direction movement - normalized vectors ensure consistent speed by changing to unit magnitude vectors
            Vector3 directionXAxis = new Vector3(currentDirectionVector.x, 0, 0).normalized;
            bool isNotCollidingX = !Physics.Raycast(currentPositionVector, directionXAxis, objectInteractionSize);//check x axis block only
            //bool isNotCollidingX = !Physics.CapsuleCast(transform.position, transform.position+Vector3.up*playerHeightOffset, playerSize, directionXAxis, Time.deltaTime * moveSpeed);//this is needed for collision handling

            if (isNotCollidingX)
            {
                return new Vector3(currentDirectionVector.x, 0f, (-1) * currentDirectionVector.z);
                //Reflection on Z axis only
            }
            else
            {
                return new Vector3((-1) * currentDirectionVector.x, 0f, currentDirectionVector.z);
                //Reflection on X axis only
            }

        }

    }

}
