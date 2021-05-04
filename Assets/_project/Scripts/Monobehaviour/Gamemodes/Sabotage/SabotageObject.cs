////////////////////////////////////////////////////////////
// File: SabotageObject
// Author: Charles Carter
// Date Created: 21/04/21
// Brief: The script for the behaviour for the players to fix objects
//////////////////////////////////////////////////////////// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SabotageObject : MonoBehaviour, IInteractable
{
    #region Interfact Contract

    //This generator was hit by a potato, so it can be damaged for a second
    void IInteractable.Interact() => TemporarilyBreak();

    #endregion

    #region Variables Needed

    [Header("Main Components Needed")]

    [SerializeField]
    private RoundManager rManager;
    [SerializeField]
    private SabotageGamemode gamemode;
    [SerializeField]
    private SoundManager soundManager;
    [SerializeField]
    private SabotageEscapeManager sabotageManager;
    [SerializeField]
    private List<CharacterManager> charsInteracting;

    [Header("Generator Fixing Variables")]

    //The timer before this object is finished
    private Timer sabotageTimer;
    [SerializeField]
    private float duration = 10;
    [SerializeField]
    private bool isBeingUsed = false;

    [Header("Generator Breaking Variables")]

    //The length the generator crashes for when hit by the potato
    [SerializeField]
    private float crash_duration = 10;
    [SerializeField]
    private bool isLocked = false;
    [SerializeField]
    private ParticleSystem SmokeVFX;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        soundManager = soundManager ?? FindObjectOfType<SoundManager>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        //These timers are available during the whole scene
        sabotageTimer = new Timer(duration);
    }

    // Update is called once per frame
    private void Update()
    {
        if (isBeingUsed)
        {
            sabotageTimer.Tick(Time.deltaTime);

            //The timer is over
            if (!sabotageTimer.isActive)
            {
                sabotageManager.GeneratorFinished(this);
            }
        }
    }

    #endregion

    #region Public Methods

    public void SetGamemode(SabotageGamemode sabotage)
    {
        gamemode = sabotage;
    }

    public void StartUsage(CharacterManager charInteracting)
    {
        //Guard clause incase someone wants to interact with it during break
        if (isLocked) return;

        isBeingUsed = true;
        charsInteracting.Add(charInteracting);

        if (soundManager)
        {
            //Start fixing sound
        }
    }

    public void StopUsage(CharacterManager charInteracting)
    {
        charsInteracting.Remove(charInteracting);

        if (soundManager)
        {
            //Stop fixing sound
        }

        if (charsInteracting.Count == 0)
        {
            isBeingUsed = false;
        }
    }

    public void SetGamemodeObject(SabotageGamemode sabotageGamemode)
    {
        gamemode = sabotageGamemode;
    }

    #endregion

    #region Private Methods

    //The generator is hit by the potato
    private void TemporarilyBreak()
    {
        if (isBeingUsed)
        {
            foreach (CharacterManager character in charsInteracting)
            {
                //Make them stop interacting with the generator

                //Currently just doing it here for easy management
                StopUsage(character);
            }

            if (soundManager)
            {
                //Play "Generator Broken" Sound
            }

            //Maybe a smoke puff VFX
            if (SmokeVFX)
            {
                SmokeVFX.Play();
            }

            //Lock the usage for a certain amount of time
            StartCoroutine(Co_LockGenerator(crash_duration));
        }
    }

    //The generator broke or needs to be locked for a time
    private IEnumerator Co_LockGenerator(float duration)
    {
        isLocked = true;

        yield return new WaitForSeconds(duration);

        isLocked = false;

        //Making sure that the vfx stops
        if (SmokeVFX)
        {
            SmokeVFX.Stop();
        }
    }

    #endregion
}
