using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetectionRadiusVisual : MonoBehaviour
{
    //radius of visible detection circle should be governed by the PlayerTwo detection radius.
    [SerializeField] private EnemyController parentEnemyController;

    private int floatToVisualMultiplier = 2;

    // Start is called before the first frame update
    void Start()
    {
        //floatToVisualMultiplier = 2x is emperically tested for this game only, on x and z axis
        //0.1f is needed on y to make the full circle visible
        transform.localScale = new Vector3(floatToVisualMultiplier * parentEnemyController.GetEnemyDetectionRadiusOfPlayerTwo(), 0.1f, floatToVisualMultiplier * parentEnemyController.GetEnemyDetectionRadiusOfPlayerTwo());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
