using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class provides all the buffs that the player can select after a level completion.

/*Enemy buff types - MaxHealth Grunt/Boss, Damage Grunt/Boss, Speed Grunt... Detection Radius Grunt, Spawn timer*/
public class EnemyBuffManager : MonoBehaviour
{
    private static EnemyBuffManager instance;
    public static EnemyBuffManager Instance
    //this instance "Property" will be tracked by ALL enemies, while keeping actual PlayerTwo object private
    {
        get { return instance; }//not very different from getters and setters
        private set { instance = value; }//we do not want any other object to modify PlayerTwo entirely.
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;//start by setting the Singleton instance 
        }
        else
        {
            Debug.LogError("Fatal Error: Cannot have a predefined instance of EnemyBuff Manager");
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
}
