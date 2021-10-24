using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Utils;

[RequireComponent(typeof(NavMeshAgent))]
public class AICore : MonoBehaviour
{
    private Transform playerTransform;
    private float curChasingSeconds = 0.0f;

    [SerializeField]
    private float losePlayerSeconds = 3.0f;
    [SerializeField]
    private float chaseSpeedMul = 2.0f;
    [SerializeField]
    private List<Transform> patrolPointsList = new List<Transform>{};
    [SerializeField]
    private AudioClip susClip;
    [SerializeField]
    private EnemyManager enemyManager;

    private LinkedList<Transform> patrolPoints;
    private LinkedListNode<Transform> nextPatrolPointNode;

    private NavMeshAgent navMeshAgent;

    private enum AIState
    {
        PATROLLING,
        INVESTIGATING,
        CHASING
    }

    private AIState state;


    private void Awake()
    {
        if (enemyManager == null)
        {
            enemyManager = FindObjectOfType<EnemyManager>();
        }
        patrolPoints = new LinkedList<Transform>(patrolPointsList);
        nextPatrolPointNode = patrolPoints.Last;

        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (state == AIState.CHASING)
        {
            Chase();
        }
        else if (state == AIState.INVESTIGATING)
        {
            Investigate();
        }
        else if (state == AIState.PATROLLING)
        {
            Patrol();
        }
        else
        {
            Debug.LogError($"Illegal state: {state} in AICore::state in gameObject: {gameObject}. Reseting to patrol state...");
            state = AIState.PATROLLING;
        }
    }

    private bool PathComplete() => !navMeshAgent.pathPending
                && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance
                && !navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f;

    public void Patrol()
    {
        if (nextPatrolPointNode.Value != null)
        {
            if (PathComplete())
            {
                navMeshAgent?.SetDestination(nextPatrolPointNode.Value.position);
                nextPatrolPointNode = nextPatrolPointNode.Next ?? patrolPoints.First;
            }
        }
    }

    private void Investigate()
    {
        if (PathComplete())
        {
            navMeshAgent?.SetDestination(nextPatrolPointNode.Value.position);
            nextPatrolPointNode = nextPatrolPointNode.Next ?? patrolPoints.First;
            state = AIState.PATROLLING;
        }
    }

    private void Chase()
    {
        if (playerTransform != null)
        {
            navMeshAgent?.SetDestination(playerTransform.position);
        }
        curChasingSeconds += Time.deltaTime;
        if (curChasingSeconds >= losePlayerSeconds)
        {
            curChasingSeconds = 0.0f;
            PlayerLost();
        }
    }

    public void Alert(Vector3 position)
    {
        //trigger surprised animation and sound effect
        if (state == AIState.PATROLLING)
        {
            AudioSource.PlayClipAtPoint(susClip, transform.position);
            navMeshAgent?.SetDestination(position);
            state = AIState.INVESTIGATING;
            enemyManager.UpdateGameState(EnemyManager.GameState.SUS);
        }
    }

    // called by vision script on player detected
    public void Spotted(Transform playerTransform)
    {
        this.playerTransform = playerTransform;
        navMeshAgent.speed *= chaseSpeedMul;
        state = AIState.CHASING;
        enemyManager.UpdateGameState(EnemyManager.GameState.DETECTED);
    }

    private void PlayerLost()
    {
        navMeshAgent.speed *= 1.0f / chaseSpeedMul;
        state = AIState.PATROLLING;
        enemyManager.UpdateGameState(EnemyManager.GameState.UNDETECTED);
    }
}
