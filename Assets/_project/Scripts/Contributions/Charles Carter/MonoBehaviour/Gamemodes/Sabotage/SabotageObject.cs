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
    private List<CharacterManager> charsInteracting = new List<CharacterManager>();

    [Header("Generator Fixing Variables")]
    //The untagged player needs to be within the fixing area for a specific amount of time before they start to fix it
    [SerializeField]
    private float playerStopDuration = 0.5f;
    List<CharacterManager> potentialFixers = new List<CharacterManager>();

    //The timer before this object is finished after starting to fix it
    private Timer sabotageTimer;
    [SerializeField]
    private float duration = 10f;
    [SerializeField]
    private bool isBeingUsed = false;
    [SerializeField]
    private GameObject fixingIconVFX;
    [SerializeField]
    private GameObject finishedIconVFX;

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
        if (fixingIconVFX)
        {
            fixingIconVFX.SetActive(false);
        }

        if (finishedIconVFX)
        {
            finishedIconVFX.SetActive(false);
        }

        //These timers are available during the whole scene
        sabotageTimer = new Timer(duration);
    }

    // Update is called once per frame
    private void Update()
    {
        if (isBeingUsed && !isLocked)
        {
            sabotageTimer.Tick(Time.deltaTime);

            //The timer is over
            if (!sabotageTimer.isActive)
            {
                if (Debug.isDebugBuild)
                {
                    Debug.Log("Generator finished", this);
                }

                //So tell the manager the generator is finished
                sabotageManager.GeneratorFinished(this);

                if (finishedIconVFX)
                {
                    finishedIconVFX.SetActive(true);
                }

                if (fixingIconVFX)
                {
                    fixingIconVFX.SetActive(false);
                }

                isLocked = true;
            }
        }
    }

    //Something has entered the area
    private void OnTriggerEnter(Collider other)
    {
        //If it's a player and this generator isnt currently locked
        if (other.CompareTag("Player") && !isLocked)
        {
            CharacterManager charManager = other.GetComponent<CharacterManager>();

            //It has a manager and the player is untagged
            if (charManager && !charManager._tracker.isTagged && charManager._tracker.enabled)
            {
                if (Debug.isDebugBuild)
                {
                    Debug.Log("New potential fixer");
                }

                potentialFixers.Add(charManager);

                StartCoroutine(Co_PlayerInArea(playerStopDuration, charManager));
            }
        }
    }

    //Something has left the area
    private void OnTriggerExit(Collider other)
    {
        //if it's a player
        if (other.CompareTag("Player"))
        {
            //And they were in the list, remove them
            CharacterManager charManager = other.GetComponent<CharacterManager>();

            if (charManager)
            {
                //And they are untagged
                //They cant be a potential fixer and they have to stop using the generator
                if (potentialFixers.Contains(charManager))
                {
                    potentialFixers.Remove(charManager);
                }

                if (charsInteracting.Contains(charManager))
                {
                    StopUsage(charManager);               
                }
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

        //There is a sound manager, and there's only 1 character interacting with this generator
        if (soundManager && charsInteracting.Count == 1)
        {
            //Start fixing sound
            soundManager.PlaySound(ScriptableSounds.Sounds.PowerUp, transform.position);
        }

        if (fixingIconVFX)
        {
            fixingIconVFX.SetActive(true);
        }
    }

    public void StopUsage(CharacterManager charInteracting)
    {
        charsInteracting.Remove(charInteracting);

        if (soundManager)
        {
            //Stop fixing sound
            soundManager.PlaySound(ScriptableSounds.Sounds.Grabbing, transform.position);
        }

        if (charsInteracting.Count == 0)
        {
            isBeingUsed = false;

            if (fixingIconVFX && !isLocked)
            {
                fixingIconVFX.SetActive(false);
            }
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

            potentialFixers.Clear();

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

    private IEnumerator Co_PlayerInArea(float duration, CharacterManager cManager)
    {
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            //The player is moved or gets moved, restart the timer
            if (cManager.GetComponent<Rigidbody>().velocity.magnitude > 0.5)
            {
                t = 0;
            }

            //The player was tagged whilst in the area
            if (cManager._tracker.isTagged)
            {
                potentialFixers.Remove(cManager);
            }

            //Moved out of range or tagged
            if (!potentialFixers.Contains(cManager))
            {
                StopCoroutine(Co_PlayerInArea(duration, cManager));
            }

            yield return null;
        }

        StartUsage(cManager);
    }

    #endregion
}
