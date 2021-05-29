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
    private bool bloodEffect = true;
    private float bloodCountdown = 1.5f;
    private GameObject playerToFollow;
    private GameObject[] players;
    #endregion Variables

    //Assign various variables in the start function for this player ai
    void Start() {
        agent = GetComponent<NavMeshAgent>();
        typeOfOrder = waypointOrdering.RANDOM   ;
        potato = GameObject.FindGameObjectWithTag("Potato");
        players = GameObject.FindGameObjectsWithTag("Player");
        soundManager = FindObjectOfType<SoundManager>();
        playerAnimation = GetComponent<PlayerAnimation>();
        aiState = state.WANDERING; 
        GetWaypoints();
        NavigationOrder();
        agent.SetDestination(waypointNodes[node].transform.position);
    }

    //Function for getting waypoints
    private void GetWaypoints() {
        waypointNodes = GameObject.FindGameObjectsWithTag("Node");

        if (waypointNodes[0] == null) {
            Debug.Log("Nodes empty");
        }
    }

    //Update function for the states of the ai
    void Update() {

        //If a player is currently being followed and is further than ten metres away then leave it
        if (playerToFollow != null) {
            if (Vector3.Distance(transform.position, playerToFollow.transform.position) > 10) {
                playerToFollow = null;
                if (aiState == state.WANDERING)
                {
                    agent.SetDestination(waypointNodes[node].transform.position);
                }
            }
        }

        //Switch statement for the ai states
        switch (aiState) {
            //Wandering state (default)
            case state.WANDERING:
                //Play walking animation
                playerAnimation.CheckToChangeState("JogForward");

                //Run away when the potato is within eleven metres
                if (Vector3.Distance(transform.position, potato.transform.position) < 11) {
                    agent.speed = agent.speed * 2;
                    aiState = state.FLEEING;
                }

                //Get a player if they are ten metres or less
                if (playerToFollow == null) {
                    for (int i = 0;i < players.Length;i++) {
                        if (Vector3.Distance(transform.position, players[i].transform.position) <= 10) { 
                            playerToFollow = players[i];
                            break;                    
                        }
                    }
                }

                //If player being followed move within five metres of them
                if (playerToFollow != null) {
                    transform.LookAt(new Vector3 (playerToFollow.transform.position.x, transform.position.y, playerToFollow.transform.position.z));
                    if (Vector3.Distance(transform.position, playerToFollow.transform.position) > 5) {
                        agent.SetDestination(playerToFollow.transform.position);

                    }
                }
                //Else go to waypoint node destination
                else if (Vector3.Distance(transform.position, waypointNodes[node].transform.position) < 3) {
                    NavigationOrder();
                    agent.SetDestination(waypointNodes[node].transform.position);
                }

                break;
            //Fleeing state
            case state.FLEEING:
                //Play running animation
                playerAnimation.CheckToChangeState("Running");
                
                //If more than thirty three metres from node then
                if (Vector3.Distance(transform.position, waypointNodes[node].transform.position) < 33) {
                    Vector3 directionToPlayer = transform.position - potato.transform.position;
                    Vector3 newPosition = transform.position + directionToPlayer;
                    agent.SetDestination(newPosition);
                }
                //Set agent speed back to nowmal and wander
                else
                {
                    agent.speed = agent.speed * 0.5f;
                    aiState = state.WANDERING;
                }
                break;
            //Dead state
            case state.DEAD:
                //Play blood effect for some time
                if (bloodEffect) {
                    bloodCountdown -= Time.deltaTime;

                    if (bloodCountdown < 0) {
                        bloodEffect = false;
                    }
                    //Stay at position
                    agent.SetDestination(transform.position);
                    particles.CreateParticle(ScriptableParticles.Particle.BloodBurst, new Vector3 (transform.position.x, transform.position.y - 1.8f, transform.position.z));
                }
                break;
        }
    }

    //This method will set the order of the navigation waypoint array starting value to it's correct one if it is in forward or reverse order
    private void NavigationOrder() {
        switch (typeOfOrder) {
            //Go forward through the nodes
            case waypointOrdering.FORWARD:
                node++;
                if (node >= waypointNodes.Length) {
                    node = 0;
                    typeOfOrder = waypointOrdering.REVERSE;
                }
                break;
            //Go reverse through the nodes
            case waypointOrdering.REVERSE:
                node--;
                if (node < 0) {
                    node = waypointNodes.Length - 1;
                    typeOfOrder = waypointOrdering.RANDOM;
                }
                break;
            //Go randomly through the nodes
            case waypointOrdering.RANDOM:
                node = (int)Random.Range(0, waypointNodes.Length - 1);
                break;
        }
    }

    //Interact function called when the potato hits the trigger
    public void Interact() {
        particles.CreateParticle(ScriptableParticles.Particle.GoalExplosion, transform.position);
        waypointNodes = GameObject.FindGameObjectsWithTag("Player");
        playerAnimation.CheckToChangeState("FallingBackDeath");
        soundManager.PlaySound(ScriptableSounds.Sounds.Explosion);
        aiState = state.DEAD;        
    }
}
