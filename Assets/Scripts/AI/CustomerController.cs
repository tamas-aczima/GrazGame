using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CustomerController : NetworkBehaviour {

    [SerializeField] private float walkSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private Canvas speechBubble;
    [SerializeField] private Text speechText;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip deatchClip;
    [SerializeField] private AudioClip servedClip;
    [SerializeField] private AudioClip sadClip;
    [SerializeField] private AudioClip angryClip;
    private Queue queue;
    private Animator anim;
    [SyncVar] private Vector3 targetPos;
    [SyncVar] private Vector3 targetDir = Vector3.zero;
    [SyncVar] private bool isFirst = false;
    [SyncVar] private bool isServed = false;
    [SyncVar] private bool isDead = false;
    [SyncVar] private bool wrongFood = false;
    private enum State
    {
        Queue,
        Wait,
        Served,
        Finished,
        Dead
    }

    private State currentState = State.Queue;

    [SyncVar] public int maxAllergens;
    [SyncVar] public int numberOfAllergens;

    public SyncListInt allergens = new SyncListInt();
    private List<Allergens> allergies = new List<Allergens>();
    [SyncVar] public int country;
    [SyncVar] public int mealType;

    private Meal servedMeal;

    private void Start()
    {
        anim = GetComponent<Animator>();
        for (int i = 0; i < allergens.Count; i++)
        {
            allergies.Add((Allergens)allergens[i]);
        }
        SetText();
    }

    private void SetText()
    {
        speechText.text = "I'm looking for a " + (MealTypes)mealType + " " + (Countries)country + " meal";

        switch (allergies.Count)
        {
            case 1:
                speechText.text += " but I'm allergic to " + allergies[0];
                break;
            case 2:
                speechText.text += " but I'm allergic to " + allergies[0] + " and " + allergies[1];
                break;
        }

        speechText.text += ".";
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

                if (servedMeal != null)
                {
                    foreach (Allergens allergen in allergies)
                    {
                        foreach (Allergens mealAllergen in servedMeal.Allergens)
                        {
                            if (allergen == mealAllergen)
                            {
                                Debug.Log("Deaaaaad");
                                isDead = true;
                                anim.SetBool("IsDead", true);
                                currentState = State.Dead;
                                if (!audioSource.isPlaying)
                                {
                                    audioSource.clip = deatchClip;
                                    audioSource.PlayOneShot(audioSource.clip);
                                }
                            }
                        }
                    }

                    if ((Countries)country != servedMeal.Country || (MealTypes)mealType != servedMeal.MealType)
                    {
                        wrongFood = true;
                    }

                    if (wrongFood)
                    {
                        anim.SetBool("IsUnhappy", true);
                        anim.SetInteger("SadOrAngry", Random.Range(0, 2));
                        if (!audioSource.isPlaying)
                        {
                            audioSource.clip = System.Convert.ToBoolean(anim.GetInteger("SadOrAngry")) ? angryClip : sadClip;
                            audioSource.PlayOneShot(audioSource.clip);
                        }
                    }
                    else
                    {
                        anim.SetBool("IsHappy", true);
                        if (!audioSource.isPlaying)
                        {
                            audioSource.clip = servedClip;
                            audioSource.PlayOneShot(audioSource.clip);
                        }
                    }

                    currentState = State.Served;
                }
                break;
            case State.Served:
                if (isDead) currentState = State.Dead;

                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && !anim.IsInTransition(0))
                {
                    isServed = true;
                    targetDir = Vector3.zero;
                    currentState = State.Finished;
                }
                break;
            case State.Finished:
                speechBubble.gameObject.SetActive(false);
                anim.SetBool("IsInPosition", false);
                if (!isServer) return;
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

    public bool IsDead
    {
        get { return isDead; }
    }

    public Queue Queue
    {
        get { return queue; }
        set { queue = value; }
    }

    public Meal ServedMeal
    {
        get { return servedMeal; }
        set { servedMeal = value; }
    }
}
