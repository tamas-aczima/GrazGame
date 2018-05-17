using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerController : MonoBehaviour {

    [SerializeField] private float walkSpeed;
    [SerializeField] private float rotateSpeed;
    private Queue queue;
    private Animator anim;
    private Vector3 targetPos;
    private Vector3 targetDir = Vector3.zero;
    private bool isServed = false;
    private bool hasAllergen = false; //temporary until we have proper food to give to customer
    private bool isDead = false;
    private enum State
    {
        Queue,
        Wait,
        Served,
        Dead
    }

    private State currentState = State.Queue;

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
            case State.Queue:
                anim.SetBool("IsInPosition", false);
                if (Vector3.Distance(transform.position, targetPos) > 0.2f)
                {
                    GetTargetDir();
                    transform.Translate(targetDir * walkSpeed * Time.deltaTime);
                }
                else
                {
                    currentState = State.Wait;
                }
                break;
            case State.Wait:
                anim.SetBool("IsInPosition", true);
                if (Vector3.Distance(transform.position, targetPos) > 0.2f)
                {
                    currentState = State.Queue;
                }

                if (isServed)
                {
                    if (hasAllergen)
                    {
                        isDead = true;
                        currentState = State.Dead;
                    }
                    else
                    {
                        targetDir = Vector3.zero;
                        currentState = State.Served;
                    }   
                }
                break;
            case State.Served:
                anim.SetBool("IsInPosition", false);
                targetPos = queue.QueueLeavePosition.position;
                GetTargetDir();
                transform.Translate(targetDir * walkSpeed * Time.deltaTime);
                Vector3 rotateDir = queue.QueueLeavePosition.position - transform.position;
                float step = rotateSpeed * Time.deltaTime;
                Vector3 newDir = Vector3.RotateTowards(transform.forward, rotateDir, step, 0.0f);
                transform.rotation = Quaternion.LookRotation(newDir);
                break;
            case State.Dead:
                anim.SetBool("IsDead", true);
                break;
        }
    }

    private void GetTargetDir()
    {
        if (targetDir != Vector3.zero) return;
        targetDir = transform.position - targetPos;
        targetDir.Normalize();
        targetDir = transform.TransformDirection(targetDir);
    }

    public Vector3 TargetPos
    {
        get { return targetPos; }
        set { targetPos = value; }
    }

    public bool IsServed
    {
        get { return isServed; }
        set { isServed = value; }
    }

    public bool HasAllergen
    {
        get { return hasAllergen; }
        set { hasAllergen = value; }
    }

    public bool IsDead
    {
        get { return isDead; }
    }

    public Queue Queue
    {
        get { return queue; }
        set { queue = value; }
    }
}
