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

public class PlayerAi : MonoBehaviour {

    //Variables
    #region Variables
    private NavMeshAgent agent;
    private RoundManager roundManager;
    public enum gamemode {CLASSIC,INFECTED,FOOTBALL,SABOTAGE};
    public enum state {WANDERING, SEEKING, CHASING,FIXING,ESCAPING,SCORING,DEFENDING};
    private enum waypointOrdering { FORWARD, REVERSE, RANDOM }
    private waypointOrdering typeOfOrder;
    public gamemode mode;
    public state aiState;
    private GameObject[] waypointNodes;
    private int node = 0;
    public bool tagged;
    private bool blueTeam;
    private GameObject potato;
    private GameObject currentlyTaggedPlayer;
    private SoundManager soundManager;
    private PlayerAnimation playerAnimation;
    private FootballObjectContainer footballObject;
    #endregion Variables

    //Assign various variables in the start function for this player ai
    void Start() {
        agent = GetComponent<NavMeshAgent>();
        typeOfOrder = (waypointOrdering)Random.Range(0, 2);
        roundManager = RoundManager.roundManager;
        potato = GameObject.FindGameObjectWithTag("Potato");
        soundManager = FindObjectOfType<SoundManager>();
        playerAnimation = GetComponent<PlayerAnimation>();
        mode = gamemode.CLASSIC;
        aiState = state.WANDERING; 
        GetWaypoints();
        NavigationOrder();
        agent.SetDestination(waypointNodes[node].transform.position);
    }

    //Function for getting waypoints
    private void GetWaypoints() {
        switch (mode)
        {
            case gamemode.CLASSIC:
                waypointNodes = GameObject.FindGameObjectsWithTag("Node");
                break;
            case gamemode.INFECTED:
                waypointNodes = GameObject.FindGameObjectsWithTag("Node");
                break;
            case gamemode.FOOTBALL:
                footballObject = FootballObjectContainer.footballObjectContainer;
                break;
            case gamemode.SABOTAGE:
                waypointNodes = GameObject.FindGameObjectsWithTag("SabotageNode");
                break;
        }
        if (waypointNodes[0] == null)
        {
            Debug.Log("Nodes empty");
        }
    }

    public void SetupFootball(Transform goalPosition,bool isBlueTeam)
    {
        waypointNodes[0] = goalPosition.gameObject;
        blueTeam = isBlueTeam;
    }

    // Update is called once per frame
    void Update() {

        switch (mode) {
            case gamemode.CLASSIC:
                switch (aiState) {
                    case state.WANDERING:
                        if (Vector3.Distance(transform.position,waypointNodes[node].transform.position) < 2)
                        {
                            NavigationOrder();
                            agent.SetDestination(waypointNodes[node].transform.position);
                        }
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
                switch (aiState) {
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
                if (aiState == state.SCORING) {

                }
                else if (aiState == state.SCORING) {

                }
                break;
            case gamemode.SABOTAGE:
                switch (aiState) {
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

    private void OnCollisionEnter(Collision collision) {
        if (!tagged && collision.transform.tag == "Potato") {
            tagged = true;
            currentlyTaggedPlayer = gameObject;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (tagged) {
            switch (mode) {
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
        else {
            switch (mode) {
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
    IEnumerator RangedAttackWait(GameObject target) {
        //gameObject.transform.LookAt
        yield return new WaitForSeconds(0.25f);

        potato.GetComponent<Rigidbody>().AddForce((gameObject.transform.forward * 1500) * Time.deltaTime, ForceMode.Impulse);
        SoundManager soundManager = GameObject.FindObjectOfType<SoundManager>();

        if (soundManager != null) {
            soundManager.PlaySound(ScriptableSounds.Sounds.Throwing);
        }
    }

    //This method will set the order of the navigation waypoint array starting value to it's correct one if it is in forward or reverse order
    private void NavigationOrder()
    {
        switch (typeOfOrder)
        {
            case waypointOrdering.FORWARD:
                node++;
                if (node >= waypointNodes.Length)
                {
                    node = 0;
                }
                break;
            case waypointOrdering.REVERSE:
                node--;
                if (node < 0)
                {
                    node = waypointNodes.Length - 1;
                }
                break;
            case waypointOrdering.RANDOM:
                node = (int)Random.Range(0, waypointNodes.Length - 1);
                break;
        }
    }
}
