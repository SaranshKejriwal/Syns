using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInteractionLogic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        //function is public because it will be called for the Player class interaction handler
        Debug.Log("Interaction Test Capsule Object - Interact function called.");
    }
}
