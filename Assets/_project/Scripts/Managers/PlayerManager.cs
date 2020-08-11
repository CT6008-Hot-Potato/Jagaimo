/////////////////////////////////////////////////////////////
//
//  Script Name: PlayerManager.cs
//  Creator: Charles Carter
//  Description: A manager script for the player
//  
/////////////////////////////////////////////////////////////

//This script uses these namespaces
using UnityEngine;

//The class needs these other components on the object
[RequireComponent(typeof(TaggedTracker), typeof(PlayerController))]
public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    private RoundManager _roundManager;
    private TaggedTracker _tracker;
    private PlayerController _movement;

    private void Awake()
    {
        _roundManager = _roundManager ?? FindObjectOfType<RoundManager>();
        _tracker = GetComponent<TaggedTracker>();
        _movement = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        RoundManager.CountdownEnded += isEliminated;
    }

    private void OnDisable()
    {
        RoundManager.CountdownEnded -= isEliminated;
    }

    private void isEliminated()
    {
        //If the tracker is enabled they arent tagged
        if (_tracker.enabled) return;

        //The player should do whatever the gamemode wants them to (base gamemode will want them to explode)
        //Play VFX + Sound
        //Turn all non-important scripts off (ones that allow the player to interact especially)
        //Make them in spectator camera
    }

    //Function to have the player locked in a position (so they cant move or rotate the camera)
    private void LockPlayer()
    {
        //Stop movement
        //Stop camera player camera movement

        //Note: option to have them switch to a different camera for cinematics
    }

    private void UnLockPlayer()
    {
        //Restart camera movement
        //Start player movement
    }

    //Functions to change the player when they're tagged or untagged
    public void ThisPlayerTagged()
    {
        //Play VFX + Sound
        //Lerp into first person camera mode
        //Animation for regaining potato
    }

    public void ThisPlayerUnTagged()
    {
        //Play VFX + Sound
        //Lerp into thrid person camera mode Note: this should be quicker than the lerp when you're tagged
    }
}
