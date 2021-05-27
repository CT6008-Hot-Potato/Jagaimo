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
    public enum state {WANDERING, FLEEING, DEAD};
    private enum waypointOrdering { FORWARD, REVERSE, RANDOM }
    private waypointOrdering typeOfOrder;
    public state aiState;
    private GameObject[] waypointNodes;
    private int node = 0;
    public bool tagged;
    private bool blueTeam;
    private GameObject potato;
    private GameObject currentlyTaggedPlayer;
    private SoundManager soundManager;
    private PlayerAnimation playerAnimation;
    [SerializeField]
    private ScriptableParticles particles;
    private CharacterManager characterManager;
    #endregion Variables

    //Assign various variables in the start function for this player ai
    void Start() {
        agent = GetComponent<NavMeshAgent>();
        typeOfOrder = (waypointOrdering)Random.Range(0, 2);
        potato = GameObject.FindGameObjectWithTag("Potato");
        soundManager = FindObjectOfType<SoundManager>();
        playerAnimation = GetComponent<PlayerAnimation>();
        characterManager = GetComponent<CharacterManager>();
        characterManager.isPlayer = true;
        aiState = state.WANDERING; 
        GetWaypoints();
        NavigationOrder();
        agent.SetDestination(waypointNodes[node].transform.position);
    }

    //Function for getting waypoints
    private void GetWaypoints() {
        waypointNodes = GameObject.FindGameObjectsWithTag("Node");
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
        switch (aiState) {
            case state.WANDERING:
                playerAnimation.CheckToChangeState("Walking");
                if (Vector3.Distance(transform.position, waypointNodes[node].transform.position) < 2)
                {
                    NavigationOrder();
                    agent.SetDestination(waypointNodes[node].transform.position);
                }
                break;
            case state.FLEEING:
                playerAnimation.CheckToChangeState("Running");
                if (Vector3.Distance(transform.position, waypointNodes[node].transform.position) < 33)
                {
                    Vector3 directionToPlayer = transform.position - potato.transform.position;
                    Vector3 newPosition = transform.position + directionToPlayer;
                    agent.SetDestination(newPosition);
                }
                else
                {
                    aiState = state.WANDERING;
                }
                break;
            case state.DEAD:
                agent.SetDestination(transform.position);
                particles.CreateParticle(ScriptableParticles.Particle.BloodBurst, transform.position);
                break;
        }
    }

    private void OnCollisionEnter(Collision collision) {

        if (collision.gameObject == potato) {
            particles.CreateParticle(ScriptableParticles.Particle.GoalExplosion, transform.position);
            waypointNodes = GameObject.FindGameObjectsWithTag("Player");
             tagged = true;
             currentlyTaggedPlayer = gameObject;
             aiState = state.DEAD;
             potato.GetComponent<Rigidbody>().isKinematic = true;
        }

       
    }

    private void OnTriggerEnter(Collider other) {

        if (other.gameObject == potato) {
            aiState = state.FLEEING;
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
                    typeOfOrder = waypointOrdering.REVERSE;
                }
                break;
            case waypointOrdering.REVERSE:
                node--;
                if (node < 0)
                {
                    node = waypointNodes.Length - 1;
                    typeOfOrder = waypointOrdering.RANDOM;
                }
                break;
            case waypointOrdering.RANDOM:
                node = (int)Random.Range(0, waypointNodes.Length - 1);
                break;
        }
    }
}
