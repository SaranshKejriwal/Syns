using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSoundsManager : MonoBehaviour
{

    [SerializeField] private AudioObjects gameSoundsReference;

    // Start is called before the first frame update
    void Start()
    {
        //PlaySound(gameSoundsReference.enemySounds[0],new Vector3(0,0,0));//for testing only  
    }

    // Update is called once per frame
    void Update()
    {
        //Recommendation is to use events when some state changes, not keep reading the state in Update()
    }

    //play a single, known sound 
    public void PlaySound(AudioClip clip, Vector3 SoundLocationVector, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(clip, SoundLocationVector, volume);
    }

    //if a selection of sounds is available, pick a random index from the array and play that.
    public void PlaySound(AudioClip[] clipArray, Vector3 SoundLocationVector, float volume = 1f)
    {
        int randomIndex = MathFunctions.GetRandomIntInRange(0, clipArray.Length);
        AudioSource.PlayClipAtPoint(clipArray[randomIndex], SoundLocationVector, volume);
    }
}
