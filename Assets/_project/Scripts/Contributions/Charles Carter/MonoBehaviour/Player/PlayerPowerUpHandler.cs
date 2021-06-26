////////////////////////////////////////////////////////////
// File: PlayerPowerUpHandler.cs
// Author: Charles Carter
// Date Created: 05/05/21
// Brief: The script for the behaviour when the player triggers a power up
//////////////////////////////////////////////////////////// 

//This script uses these namespaces
using System.Collections;
using UnityEngine;

public class PlayerPowerUpHandler : MonoBehaviour {
    #region Variables Needed

    private RoundManager roundManager;
    private GameSettingsContainer settings;

    [SerializeField]
    private WorldBounds waterBounds;

    //The main components affected
    [SerializeField]
    private PlayerCamera _cam;
    [SerializeField]
    private PlayerController _movement;
    [SerializeField]
    private PlayerInteraction _interaction;
    [SerializeField]
    private PowerUpUi PowerUp_UI;

    #endregion

    #region Unity Methods

    private void Awake() {
        _cam = _cam ?? GetComponent<PlayerCamera>();
        _movement = _movement ?? GetComponent<PlayerController>();
        _interaction = _interaction ?? GetComponent<PlayerInteraction>();

        settings = GameSettingsContainer.instance;

        if (settings) {
            //If it doesn't have a value for the powerups bool
            if (!settings.HasGenMutator(7)) {
                //Then this script isnt needed
                enabled = false;
            }
        }
    }

    // Start is called before the first frame update
    void Start() {
        roundManager = RoundManager.roundManager;
        waterBounds = roundManager.worldBoundry;

        if (!PowerUp_UI && Debug.isDebugBuild) {
            Debug.Log("There's no powerUpUI set", this);
        }
    }

    #endregion

    #region Public Methods

    public void PowerUpTriggered(int powerUpID) {
        //Handle what happens depending on the powerup ID (it's like this due to not many powerups)
        //Power Up 0 - Flipping the camera upside down temporarily
        //Power Up 1 - Increasing player speed temporarily
        //Power Up 2 - Bullet Potato
        //Power Up 3 - Flood
        //Power Up 4 -   

        //Forcing it to be movement since this may not be fully implemented
        powerUpID = 1;

        switch (powerUpID) {
            case 0:
                //_cam.ChangeRoll();
                //StartCoroutine();
                break;
            case 1:
                if (_movement) {
                    _movement.speed += 0.3f;
                }
                break;
            case 2:
                //_interaction.BulletPotato(true);
                break;
            case 3:

                break;
            case 4:

                break;
            default:
                if (Debug.isDebugBuild) {
                    Debug.Log("Power Up Triggered", this);
                }
                break;
        }

        StartCoroutine(PowerUpTimer(powerUpID));
    }

    #endregion

    #region Private Methods

    private IEnumerator PowerUpTimer(int ID) {
        yield return new WaitForSeconds(5f);

        switch (ID) {
            case 0:
                //_cam.ChangeRoll
                //StartCoroutine();
                break;
            case 1:
                if (_movement) {
                    _movement.speed -= 0.3f;
                }
                break;
            case 2:
                //_interaction.BulletPotato(false);
                break;
            case 3:

                break;
            case 4:

                break;
            default:
                if (Debug.isDebugBuild) {
                    Debug.Log("Power Up Triggered", this);
                }
                break;
        }
    }

    #endregion
}
