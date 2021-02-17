using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "PowerUps", menuName = "ScriptableObjects/PowerUps", order = 2)]
public class ScriptablePowerUps : ScriptableObject
{
    [Serializable]
    public class PowerUp
    {
        public string name;
        public Sprite SpriteImage; 
    }
    public PowerUp[] powerUps;
}
