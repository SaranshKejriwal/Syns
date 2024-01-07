using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoorClosedVisualHandler : MonoBehaviour
{
    //this class only shows/hides the wooden exit door.
    //If door is open, it will drop the localScale of ClosedDoor to 0, effectively hiding it.

    [SerializeField] private ExitDoorController exitDoor;

    void Start()
    {
        if (exitDoor == null)
        {
            Debug.LogError("Exit Door is null in the Closed Door Visual script");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (exitDoor.IsExitDoorOpen())
        {
            transform.localScale = Vector3.zero;// Closed Door visual disappears

            //later, we can also play with the EulerAngles to show an ajar door.
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }
}
