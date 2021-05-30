////////////////////////////////////////////////////////////
// File: SabotageEscapeCollider.cs
// Author: Charles Carter
// Date Created: 25/05/21
// Brief: The collider which determines if the players have escaped in the sabotage gamemode
//////////////////////////////////////////////////////////// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SabotageEscapeCollider : MonoBehaviour
{
    #region Variables Needed

    [SerializeField]
    private RoundManager roundManager;
    [SerializeField]
    private SabotageGamemode gamemode;

    #endregion

    #region Unity Methods

    private void Start()
    {
        roundManager = roundManager ?? RoundManager.roundManager;
        gamemode = roundManager.GetComponent<SabotageGamemode>();
    }

    private void OnTriggerEnter(Collider other)
    {
        CharacterManager isCharacter = other.GetComponent<CharacterManager>();

        if (other.CompareTag("Player") && isCharacter)
        {
            gamemode.CharacterEscapes(isCharacter);
        }
    }

    #endregion
}
