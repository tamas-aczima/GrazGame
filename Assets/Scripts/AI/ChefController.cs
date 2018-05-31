using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class ChefController : NetworkBehaviour {

    private Animator anim;
    private NavMeshAgent navMeshAgent;
    [SerializeField] private float chaseSpeed;
    [SyncVar] private bool shouldChase;
    [SyncVar] private GameObject target;
    [SyncVar] private Vector3 startPos;

    public bool ShouldChase
    {
        get { return shouldChase; }
        set { shouldChase = value; }
    }

    public GameObject Target
    {
        get { return target; }
        set { target = value; }
    }

    private enum State
    {
        Idle,
        Chase,
        Return
    }

    private State currentState;

    private void Start()
    {
        anim = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        startPos = transform.position;
    }

    private void Update()
    {
        FSM();
    }

    private void FSM()
    {
        switch (currentState)
        {
            case State.Idle:
                anim.SetBool("IsChasing", false);
                if (shouldChase)
                {
                    currentState = State.Chase;
                }
                break;
            case State.Chase:
                anim.SetBool("IsChasing", true);
                navMeshAgent.SetDestination(target.transform.position);
                if (Vector3.Distance(transform.position, target.transform.position) < 0.2f)
                {
                    target.transform.position = FindObjectOfType<GameManager>().spawnPoint.position;
                    shouldChase = false;
                    currentState = State.Return;
                }
                break;
            case State.Return:
                navMeshAgent.SetDestination(startPos);
                if (Vector3.Distance(transform.position, startPos) < 0.1f)
                {
                    currentState = State.Idle;
                }
                break;
        }
    }
}
