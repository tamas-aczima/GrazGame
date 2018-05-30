using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefController : MonoBehaviour {

    private Animator anim;

	private enum State
    {
        Idle,
        Chase
    }

    private State currentState;

    private void Start()
    {
        anim = GetComponent<Animator>();
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
                break;
            case State.Chase:
                anim.SetBool("IsChasing", true);
                break;
        }
    }
}
