using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using Utils;

[RequireComponent(typeof(NavMeshAgent))]
public class AICore : MonoBehaviour
{
    private Transform playerTransform;
    public Transform PlayerTransform => playerTransform;
    private float curChasingSeconds = 0.0f;

    [SerializeField]
    private float playerCaughtRadius = 0.5f;
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
    [SerializeField]
    private Animator animator;



    public GameObject loseMenu;
    private bool mFaded = false;
    public CanvasGroup uiElement;

    private LinkedList<Transform> patrolPoints;
    private LinkedListNode<Transform> nextPatrolPointNode;

    private NavMeshAgent navMeshAgent;

    private enum AIState
    {
        PATROLLING,
        INVESTIGATING,
        CHASING
    }

    private AIState state = AIState.PATROLLING;


    private void Awake()
    {
        if (enemyManager == null)
        {
            enemyManager = FindObjectOfType<EnemyManager>();
            if (enemyManager == null)
            {
                GameObject g = new GameObject("EnemyManager");
                enemyManager = g.AddComponent<EnemyManager>();
            }
        }
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        playerTransform = FindObjectOfType<PlayerController>().transform;

        patrolPoints = new LinkedList<Transform>(patrolPointsList);
        nextPatrolPointNode = patrolPoints.First;

        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        navMeshAgent?.SetDestination(nextPatrolPointNode.Value.position);
    }

    private void Update()
    {
        animator.SetBool("isWalking", navMeshAgent.velocity.magnitude > 0.01f);
        
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
        navMeshAgent?.SetDestination(nextPatrolPointNode.Value.position);
        if (PathComplete())
        {
            nextPatrolPointNode = nextPatrolPointNode.Next ?? patrolPoints.First;
        }
    }

    private void Investigate()
    {
        if (PathComplete())
        {
            navMeshAgent.SetDestination(nextPatrolPointNode.Value.position);
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
        if (Vector3.Distance(playerTransform.position, transform.position) < playerCaughtRadius)
        {
            Debug.Log("caught");
            enemyManager.UpdateGameState(EnemyManager.GameState.LOSE);
            loseMenu.SetActive(true);
            FadeIn();
        }
    }
    public void FadeIn()
    {
        StartCoroutine(FadeCanvasGroup(uiElement, uiElement.alpha, 1));
    }

    public IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float lerpTime = 2f)
    {
        float _timeStartedLerping = Time.time;
        float timeSinceStarted = Time.time - _timeStartedLerping;
        float percentageComplete = timeSinceStarted / lerpTime;
        cg = loseMenu.GetComponent<CanvasGroup>();
        while (true)
        {
            timeSinceStarted = Time.time - _timeStartedLerping;
            percentageComplete = timeSinceStarted / lerpTime;

            float currentValue = Mathf.Lerp(start, end, percentageComplete);

            cg.alpha = currentValue;

            if (percentageComplete >= 1) break;

            yield return new WaitForEndOfFrame();
        }
    }

    public void Alert(Vector3 position)
    {
        //trigger surprised animation and sound effect
        if (state == AIState.PATROLLING || state == AIState.INVESTIGATING)
        {
            if (susClip)
            {
                AudioSource.PlayClipAtPoint(susClip, transform.position);
            }
            navMeshAgent?.SetDestination(position);
            state = AIState.INVESTIGATING;
            enemyManager.UpdateGameState(EnemyManager.GameState.SUS);
        }
    }

    // called by vision script on player detected
    public void Spotted()
    {
        animator.SetBool("isRunning", true);
        navMeshAgent.speed *= chaseSpeedMul;
        state = AIState.CHASING;
        enemyManager.UpdateGameState(EnemyManager.GameState.DETECTED);
    }

    private void PlayerLost()
    {
        animator.SetBool("isRunning", false);
        navMeshAgent.speed /= chaseSpeedMul;
        state = AIState.PATROLLING;
        enemyManager.UpdateGameState(EnemyManager.GameState.UNDETECTED);
    }
}
