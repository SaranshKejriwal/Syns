using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class will be the parent of all collectibles - Gold, Health and ExitKey
//It will define the generic behaviour on player vicinity, and each class will implement an onCollection method
public class GenericCollectibleItem : MonoBehaviour
{
    //this can be PlayerOne, PlayerTwo, Sack, Shop or null
    private GenericPlayerControl itemOwner;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollectionByPlayer()
    {

    }

}
