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

public class PlayerAi : MonoBehaviour, IInteractable {

    //Variables
    #region Variables
    private NavMeshAgent agent;
    public enum state {WANDERING, FLEEING, DEAD};
    private enum waypointOrdering { FORWARD, REVERSE, RANDOM }
    private waypointOrdering typeOfOrder;
    public state aiState;
    private GameObject[] waypointNodes;
    private int node = 0;
    private bool blueTeam;
    private GameObject potato;
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

    // Update is called once per frame
    void Update() {
        switch (aiState) {
            case state.WANDERING:
                playerAnimation.CheckToChangeState("JogForward");
                if (Vector3.Distance(transform.position, waypointNodes[node].transform.position) < 2)
                {
                    NavigationOrder();
                    agent.SetDestination(waypointNodes[node].transform.position);
                }

                if (Vector3.Distance(transform.position, waypointNodes[node].transform.position) < 15)
                {
                    agent.speed = agent.speed * 2;
                    aiState = state.FLEEING;
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
                    agent.speed = agent.speed * 0.5f;
                    aiState = state.WANDERING;
                }
                break;
            case state.DEAD:
                agent.SetDestination(transform.position);
                particles.CreateParticle(ScriptableParticles.Particle.BloodBurst, transform.position);
                break;
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

    public void Interact() {
        particles.CreateParticle(ScriptableParticles.Particle.GoalExplosion, transform.position);
        waypointNodes = GameObject.FindGameObjectsWithTag("Player");
        soundManager.PlaySound(ScriptableSounds.Sounds.Explosion);
        aiState = state.DEAD;        
    }
}
