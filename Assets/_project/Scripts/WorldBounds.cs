/////////////////////////////////////////////////////////////
//
//  Script Name: WorldBounds.cs
//  Creator: Charles Carter
//  Description: A script for when something leaves the map
//  
/////////////////////////////////////////////////////////////

//This script uses these namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A trigger placed below the test world
public class WorldBounds : MonoBehaviour
{
    //The specified position moving by Charlie Bullock
    [SerializeField]
    private Vector3 position;

    [SerializeField]
    private RoundManager roundManager;
    [SerializeField]
    private List<CharacterManager> charactersInWater = new List<CharacterManager>();
    [SerializeField]
    private float killDuration = 0.25f;

    private void Start()
    {
        roundManager = RoundManager.roundManager;
    }

    //When something enters the bounds
    private void OnTriggerEnter(Collider other)
    {
        //Position isnt set to anything
        if (position == Vector3.zero)
        {
            if (Debug.isDebugBuild)
            {
                Debug.Log("Set position value" + other.name, this);
            }

            return;
        }

        //If it's not an object which shouldnt be moved 
        if (!other.CompareTag("PositionStay"))
        {
            //If it's a player
            if (other.CompareTag("Player")  && roundManager._currentGamemode.Return_Mode() != GAMEMODE_INDEX.FOOTBALL)
            {
                //Get their manager
                CharacterManager character = other.GetComponent<CharacterManager>();

                //If they have a manager
                if (character)
                {
                    //Start the timer on them being outside the bounds
                    charactersInWater.Add(other.GetComponent<CharacterManager>());
                    StartCoroutine(PlayerKill(character));
                }
            }
            else
            {
                other.transform.position = position;
                if (other.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    if (Debug.isDebugBuild)
                    {
                        Debug.Log("Something left the map: " + other.name, this);
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CharacterManager manager = other.GetComponent<CharacterManager>();

        if (manager && charactersInWater.Contains(manager))
        {
            charactersInWater.Remove(manager);
        }
    }

    private IEnumerator PlayerKill(CharacterManager character)
    {
        for (float t = 0; t < killDuration; t += Time.deltaTime)
        {
            if (charactersInWater.Contains(character))
            {
                yield return null;
            }
            else
            {
                StopCoroutine(PlayerKill(character));
            }
        }

        character.ForceElimination();
    }
}
