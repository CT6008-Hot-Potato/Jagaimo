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
        RoundManager.CountdownEnded += CheckIfTagged;
    }

    private void OnDisable()
    {
        RoundManager.CountdownEnded -= CheckIfTagged;
    }

    private void CheckIfTagged()
    {
        //If the tracker is enabled they arent tagged
        if (_tracker.enabled) return;

        //The player should do whatever the gamemode wants them to (base gamemode will want them to explode)

    }

    private void LockPlayer()
    {

    }

    //Functions to change the player when they're tagged or untagged
    public void ThisPlayerTagged()
    {

    }

    public void ThisPlayerUnTagged()
    {

    }
}
