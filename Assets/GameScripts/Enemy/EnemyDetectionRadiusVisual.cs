using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectionRadiusVisual : MonoBehaviour
{
    //radius of visible detection circle should be governed by the Enemy detection radius directly.
    [SerializeField] private EnemyGruntController parentEnemyController;

    private int floatToVisualMultiplier = 2;

    // Start is called before the first frame update
    void Start()
    {
        //floatToVisualMultiplier = 2x is emperically tested for this game only, on x and z axis
        //0.1f is needed on y to make the full circle visible
        transform.localScale = new Vector3(floatToVisualMultiplier * parentEnemyController.GetEnemyDetectionRadius(), 0.1f, floatToVisualMultiplier * parentEnemyController.GetEnemyDetectionRadius());
    }

    // Update is called once per frame
    void Update()
    {
        //if Enemy is dead, drop enemy detection radius to 0, else do nothing
        if (parentEnemyController.IsEnemyDead()) 
        {
            transform.localScale = new Vector3(0, 0.1f, 0);
        }
    }
}
