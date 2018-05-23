using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerController : MonoBehaviour {

    [SerializeField] private float walkSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private Canvas speechBubble;
    [SerializeField] private AudioSource servedSource;
    [SerializeField] private AudioSource deathSource;
    [SerializeField] private AudioClip[] deathClips;
    private Queue queue;
    private Animator anim;
    private Vector3 targetPos;
    private Vector3 targetDir = Vector3.zero;
    private bool isFirst = false;
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
                    GetTargetDir(targetPos, transform.position);
                    transform.Translate(targetDir * walkSpeed * Time.deltaTime);
                }
                else
                {
                    currentState = State.Wait;
                }
                break;
            case State.Wait:
                anim.SetBool("IsInPosition", true);
                if (isFirst)
                {
                    speechBubble.gameObject.SetActive(true);
                }

                if (Vector3.Distance(transform.position, targetPos) > 0.2f)
                {
                    currentState = State.Queue;
                }

                if (isServed)
                {
                    if (hasAllergen)
                    {
                        isDead = true;
                        if (!deathSource.isPlaying)
                        {
                            deathSource.clip = deathClips[Random.Range(0, deathClips.Length)];
                            deathSource.PlayOneShot(deathSource.clip);
                        }
                        currentState = State.Dead;
                    }
                    else
                    {
                        targetDir = Vector3.zero;
                        if (!servedSource.isPlaying)
                        {
                            servedSource.PlayOneShot(servedSource.clip);
                        }
                        currentState = State.Served;
                    }   
                }
                break;
            case State.Served:
                speechBubble.gameObject.SetActive(false);
                anim.SetBool("IsInPosition", false);
                targetPos = queue.QueueLeavePosition.position;
                GetTargetDir(transform.position, targetPos);
                transform.Translate(targetDir * walkSpeed * Time.deltaTime);
                Vector3 rotateDir = queue.QueueLeavePosition.position - transform.position;
                float step = rotateSpeed * Time.deltaTime;
                Vector3 newDir = Vector3.RotateTowards(transform.forward, rotateDir, step, 0.0f);
                transform.rotation = Quaternion.LookRotation(newDir);
                if (Vector3.Distance(transform.position, queue.QueueLeavePosition.position) < 0.1f)
                {
                    Destroy(gameObject);
                }
                break;
            case State.Dead:
                anim.SetBool("IsDead", true);
                speechBubble.gameObject.SetActive(false);
                break;
        }
    }

    private void GetTargetDir(Vector3 startPos, Vector3 endPos)
    {
        if (targetDir != Vector3.zero) return;
        targetDir = endPos - startPos;
        targetDir.Normalize();
        targetDir = transform.TransformDirection(targetDir);
    }

    public Vector3 TargetPos
    {
        get { return targetPos; }
        set { targetPos = value; }
    }

    public bool IsFirst
    {
        get { return isFirst; }
        set { isFirst = value; }
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
