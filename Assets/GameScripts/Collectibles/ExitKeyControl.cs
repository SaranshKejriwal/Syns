using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ExitKey is also collectible by Player, but PlayerTwo only
public class ExitKeyControl : GenericCollectibleItem
{
    // Start is called before the first frame update
    private int hoverGapAbovePlayerTwoVisual = 2;
    void Start()
    {
        setParentAsPlayerTwo();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setParentAsPlayerTwo()
    {
        //this will directly make the Key move relative to PlayerTwo, without calling in Update each time.
        transform.parent = PlayerTwoControl.Instance.transform;
        transform.position = GetHoverLocationAbovePlayerTwoVisual();
        
    }

    private Vector3 GetHoverLocationAbovePlayerTwoVisual()
    {
        //this inefficient method is acceptable because it will be called only once, whenever playerTwo is in ExitKey vicinity
        return new Vector3(PlayerTwoControl.Instance.GetPlayerTwoLocation().x, PlayerTwoControl.Instance.GetPlayerTwoLocation().y+hoverGapAbovePlayerTwoVisual, PlayerTwoControl.Instance.GetPlayerTwoLocation().z);
    }
}
