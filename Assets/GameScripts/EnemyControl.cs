using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private int enemyWalkingMovementSpeed = 4; //when enemy is walking normally
    private int enemyHuntingMovementSpeed = 8; //when Player 2 is detected by the Enemy and Enemy is chasing Player 2
    //Note - Hunting speed should exceed the max movement speed of playerTwo, else enemy will never catch up.

    [SerializeField] private float enemySize = 1f; //needed for collision handling in Raycast function.

    private int rotationSpeed = 10;
    private bool isEnemyMoving = true; //used by animator to render movement animation if enemy is moving normally
    private bool isEnemyHunting = false;//used by animator to render movement animation if enemy is hunting playerTwo
    //Enemy will either move or hunt, not both.

    private Vector3 currentEnemyDirectionVector = Vector3.zero;

    void Start()
    {
        currentEnemyDirectionVector = AutoMovementHandler.GetRandomDirectionVector();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CheckPlayerTwoHunt()
    {

    }
}
