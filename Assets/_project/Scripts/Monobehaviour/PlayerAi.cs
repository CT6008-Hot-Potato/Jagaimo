/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///PlayerAi.cs
///Developed by Charlie Bullock
///This class is responsible for all the various aspects of the player Ai for all gamemodes.
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//This class is using:
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAi : MonoBehaviour
{
    private NavMeshAgent agent;
    private RoundManager roundManager;
    public enum gamemode {CLASSIC,INFECTED,FOOTBALL,SABOTAGE};
    public enum state {WANDERING, SEEKING, CHASING,FIXING,ESCAPING,SCORING,DEFENDING};
    private enum waypointOrdering { FORWARD, REVERSE, RANDOM }
    private waypointOrdering typeOfOrder;
    public gamemode mode;
    public state aiState;
    private GameObject[] waypointNodes;
    public bool tagged;
    private GameObject potato;
    private GameObject currentlyTaggedPlayer;
    private SoundManager soundManager;
    private PlayerAnimation playerAnimation;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        typeOfOrder = (waypointOrdering)Random.Range(0, 2);
        waypointNodes = GameObject.FindGameObjectsWithTag("Node");
        roundManager = RoundManager.roundManager;
        potato = GameObject.FindGameObjectWithTag("Potato");
        soundManager = FindObjectOfType<SoundManager>();
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (typeOfOrder)
        {
            case waypointOrdering.FORWARD:

                break;
            case waypointOrdering.REVERSE:

                break;
            case waypointOrdering.RANDOM:

                break;
        }
        switch (mode)
        {
            case gamemode.CLASSIC:
                switch (aiState)
                {
                    case state.WANDERING:
                        break;
                    case state.SEEKING:
                        break;
                    case state.CHASING:
                        break;
                    case state.ESCAPING:
                        break;
                }
                break;
            case gamemode.INFECTED:
                switch (aiState)
                {
                    case state.WANDERING:
                        break;
                    case state.SEEKING:
                        break;
                    case state.CHASING:
                        break;
                    case state.FIXING:
                        break;
                    case state.ESCAPING:
                        break;
                }
                break;
            case gamemode.FOOTBALL:
                if (aiState == state.SCORING)
                {

                }
                else if (aiState == state.SCORING)
                {

                }
                break;
            case gamemode.SABOTAGE:
                switch (aiState)
                {
                    case state.SEEKING:
                        break;
                    case state.CHASING:
                        break;
                    case state.FIXING:
                        break;
                    case state.ESCAPING:
                        break;
                }
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!tagged && collision.transform.tag == "Potato")
        {
            tagged = true;
            currentlyTaggedPlayer = gameObject;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (tagged)
        {
            switch (mode)
            {
                case gamemode.CLASSIC:
                    break;
                case gamemode.INFECTED:
                    break;
                case gamemode.FOOTBALL:
                    break;
                case gamemode.SABOTAGE:
                    break;
            }
        }
        else
        {
            switch (mode)
            {
                case gamemode.CLASSIC:
                    break;
                case gamemode.INFECTED:
                    break;
                case gamemode.FOOTBALL:
                    break;
                case gamemode.SABOTAGE:
                    break;
            }
        }
    }

    //This function causes a short times delay for the gap between ranged attacking and waiting to ranged attack
    IEnumerator RangedAttackWait(GameObject target)
    {
        //gameObject.transform.LookAt
        yield return new WaitForSeconds(0.25f);

        potato.GetComponent<Rigidbody>().AddForce((gameObject.transform.forward * 1500) * Time.deltaTime, ForceMode.Impulse);
        SoundManager soundManager = GameObject.FindObjectOfType<SoundManager>();

        if (soundManager != null)
        {
            soundManager.PlaySound(ScriptableSounds.Sounds.Throwing);
        }
    }

    //This method will set the order of the navigation waypoint array starting value to it's correct one if it is in forward or reverse order
    private void NavigationOrder()
    {
        //switch (typeOfOrder)
        //{
        //    case waypointOrdering.Forward:
        //        if (nodes >= waypointPositions.Length)
        //        {
        //            nodes = 0;
        //        }
        //        break;
        //    case waypointOrdering.Reverse:
        //        if (nodes < 0)
        //        {
        //            nodes = waypointPositions.Length - 1;
        //        }
        //        break;
        //}
    }
}
