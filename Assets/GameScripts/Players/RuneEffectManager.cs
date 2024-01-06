using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class holds the level of each rune and the effect applied by each rune, if a player has it.
public class RuneEffectManager : MonoBehaviour
{//MonoBehaviour because this script will be used in the Shop object while upgrading Runes.
    private static RuneEffectManager instance;
    public static RuneEffectManager Instance
    {
        /*Enemy Boss will be a singleton. It also controls EnemySpawn Handler.*/
        get { return instance; }
        private set { instance = value; }
    }

    private uint GreedRuinGlimmerLevel = 1;//Level of Greed Ruin from 1-5


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;//start by setting the Singleton instance 
        }
        else
        {
            Debug.LogError("Fatal Error: Cannot have a predefined instance of Rune Effect Manager");
        }
    }

    public float GetGreedRuinEffect(bool playerHasGreedRuin)
    {
        if (!playerHasGreedRuin)
        {
            return 1;
        }
        else
        {
            //each level up increases the Gold value on collect by 15%
            return (1 + GreedRuinGlimmerLevel * 0.15f);
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
