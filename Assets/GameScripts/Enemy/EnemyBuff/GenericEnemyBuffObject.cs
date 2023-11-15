using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this abstract class will represent the types of buffs that can be applied to Enemies on Level Completion
public abstract class GenericEnemyBuffObject : ScriptableObject
{
    protected string buffDescription;//Description shown on the UI

    protected abstract void SetBuffName();

    protected abstract void SetBuffDescription();

    //this method will access the GenericEnemyController or EnemyBossController or SpawnHandler class
    //and apply the buffs necessary to newly spawned enemies.
    protected abstract void ApplyBuff();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
