using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Utils;

[RequireComponent(typeof(NavMeshAgent))]
public class AICore : MonoBehaviour
{
    [SerializeField]
    private float investigateSuspiciousLocationSeconds = 3.0f;
    [SerializeField]
    private List<Transform> patrolPointsList = new List<Transform>{};

    private LinkedList<Transform> patrolPoints;
    private LinkedListNode<Transform> nextPatrolPointNode;

    private bool isSuspecting;
    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        patrolPoints = new LinkedList<Transform>(patrolPointsList);
        nextPatrolPointNode = patrolPoints.Last;

        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void FixedUpdate()
    {
        if (isSuspecting)
        {
            Maybe<Vector3> seenPlayerPos = SeenAt();
            if (seenPlayerPos is Maybe<Vector3>.Some somePos)
            {
                PlayerFound(somePos.Value);
            }
        }
        else
        {
            Maybe<Vector3> heardPlayerPos = HeardAt();
            if (heardPlayerPos is Maybe<Vector3>.Some somePos)
            {
                Suspect(somePos.Value);
            }
        }
    }
    
    // TODO: implement hearing mechanic
    private Maybe<Vector3> HeardAt()
    {
        //placeholder
        return new Maybe<Vector3>.None();
    }


    // TODO: implement vision mechanic
    private Maybe<Vector3> SeenAt()
    {
        //placeholder
        return new Maybe<Vector3>.None();
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

    public void Alert(Vector3 position)
    {

    }

    public void Suspect(Vector3 suspectPosition)
    {
        // set game state to player pos suspected
        // approach destination and observe for some amount of time
        // if nothing seen, isSuspecting = false
    }

    public void PlayerFound(Vector3 playerSpottedPosition)
    {
        // set game state to player spotted
        // update the destination to last seen
    }
}
