////////////////////////////////////////////////////////////
// File: UIScriptableObject
// Author: James Bradbury
// Brief:  A test script for profiling UI elements
//////////////////////////////////////////////////////////// 


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ScriptableUI", menuName = "ScriptableObjects/ScriptableUI", order = 1)]
public class UIScriptableObject : ScriptableObject
{

    public Sprite Potato, PotatoAlpha, PotatoFuse, MaskAlpha;   // Exposes sprites to the inspector so that sprites can be allocated     

}
