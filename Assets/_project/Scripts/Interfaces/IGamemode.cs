/////////////////////////////////////////////////////////////
//
//  Script Name: IGamemode.cs
//  Creator: Charles Carter
//  Description: An interface that each gamemode has to use
//  
/////////////////////////////////////////////////////////////

//This script uses these namespaces
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGamemode
{
    //The contracted functions and variables that every gamemode will have
    bool WinCondition();
}
