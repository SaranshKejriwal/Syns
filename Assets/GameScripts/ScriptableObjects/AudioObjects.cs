using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AudioObjects : ScriptableObject
{
    public AudioClip[] enemySounds;//array because we want to club All Enemy sounds into 1 object
    public AudioClip[] enemyBossSounds; //boss roar, attack
    public AudioClip[] ambientSounds;//crickets, rats, wind
    public AudioClip[] playerMovementSounds;//walking
    public AudioClip[] playerAttackSounds;//PlayerOne punch, range
    public AudioClip[] collectiblesSounds;//coins, heals
    public AudioClip[] gameplaySounds;//key collection, door open, victory


}
