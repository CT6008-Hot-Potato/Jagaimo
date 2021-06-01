///////////////
//
//  Script Name: JagaimoMenuComponent.cs
//  Creator: James Bradbury
//  Description:
//  
/////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JagaimoMenuComponent : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private JagaimoNetworkManager jagaimoNetworkManger = null;

    //[Header("Multiplayer UI")]
    //[SerializeField]
    //private GameObject mpConnectionType = null;

    public void HostLobby()
    {
        jagaimoNetworkManger.StartHost();
    }

}
