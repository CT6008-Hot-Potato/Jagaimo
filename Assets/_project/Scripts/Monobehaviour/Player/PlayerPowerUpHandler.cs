////////////////////////////////////////////////////////////
// File: PlayerPowerUpHandler.cs
// Author: Charles Carter
// Date Created: 05/05/21
// Brief: The script for the behaviour when the player triggers a power up
//////////////////////////////////////////////////////////// 

using UnityEngine;

public class PlayerPowerUpHandler : MonoBehaviour
{
    #region Variables Needed

    GameSettingsContainer settings;

    //The main components affected
    [SerializeField]
    private PlayerCamera _cam;
    [SerializeField]
    private PlayerController _movement;
    [SerializeField]
    private PowerUpUi PowerUp_UI;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        _cam = _cam ?? GetComponent<PlayerCamera>();
        _movement = _movement ?? GetComponent<PlayerController>();

        settings = GameSettingsContainer.instance;

        if (settings)
        {
            //If it doesn't have a value for the powerups bool
            if (!settings.HasGenMutator(7))
            {
                //Then this script isnt needed
                enabled = false;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!PowerUp_UI && Debug.isDebugBuild)
        {
            Debug.Log("There's no powerUpUI set", this);
        }
    }

    #endregion

    #region Public Methods

    public void PowerUpTriggered(int powerUpID)
    {
        //Handle what happens depending on the powerup ID (it's like this due to not many powerups)
        //Power Up 0 - Flipping the camera upside down temporarily
        //Power Up 1 - Increasing player speed temporarily
        //Power Up 2 - Bullet Potato
        //Power Up 3 - Flood
        //Power Up 4 - 

        switch (powerUpID)
        {
            default:
                if (Debug.isDebugBuild)
                {
                    Debug.Log("Power Up Triggered", this);
                }
                break;
        }
    }

    #endregion

    #region Private Methods

    #endregion
}
