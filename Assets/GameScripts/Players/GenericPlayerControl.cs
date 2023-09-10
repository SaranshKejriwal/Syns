using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This Generic Controller PlayerClass will be the Parent of P1 and P2
//It can be used to apply generic buff items, health setters etc.
public class GenericPlayerControl : MonoBehaviour
{
    protected int playerHealth = 10;
    protected bool isActive = false; //true for playerOne and PlayerTwo only. False for Shop and Sack.

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //common function to manage Max Player health
    public void SetPlayerMaxHealth(int playerHealth)
    {
        this.playerHealth = playerHealth;
    }

    public bool isActivePlayer()
    {
        return isActive;
    }

}
