using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]//this is used to create an object of this class
public class EnemyScriptableObject : ScriptableObject
{
    //This class would be needed to define the various Syns enemy types.
    //This should extend ScriptableObject, not MonoBehaviour.

    //public because ScriptableObjects should be used as Read-only data containers.
    public Transform enemyPrefab;
    public Sprite iconSprite; //not sure if this would be required.
    public string enemyType; //can replace with an enum later


}
