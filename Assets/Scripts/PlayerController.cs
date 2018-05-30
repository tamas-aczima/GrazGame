using Assets.Scripts;
using Assets.Scripts.Classes;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{

    private Animator anim = null;
    private CharacterController charController = null;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 rotateDirectionX = Vector3.zero;
    private float rotateDirectionY = 0f;
    [SerializeField] private float yCamRange;
    [SerializeField] private float walkSpeed;
    [SerializeField] private Camera cam;
    [SerializeField] private float rayDistance;
    [SerializeField] private int scorePerCustomer;

    private int score = 0;
    private GameManager gameManager;

    public Meal TakenMealToServe;

    public override void OnStartLocalPlayer()
    {
        AudioListener listener = gameObject.AddComponent(typeof(AudioListener)) as AudioListener;
        gameManager = FindObjectOfType<GameManager>();
        anim = GetComponent<Animator>();
        charController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        TakenMealToServe = null;
    }

    private void Start()
    {
        if (!isLocalPlayer)
        {
            cam.enabled = false;
        }
    }

    private void Update()
    {
        if (!isLocalPlayer) return;
        Move();
        Rotate();
        Animation();
        ChooseMeal();
        ServeCustomer();
    }

    private void Move()
    {
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection = transform.TransformDirection(moveDirection);
        charController.Move(moveDirection * walkSpeed * Time.deltaTime);
    }

    private void Rotate()
    {
        rotateDirectionX += new Vector3(0, Input.GetAxis("Mouse X"), 0);
        transform.rotation = Quaternion.Euler(rotateDirectionX);

        rotateDirectionY -= Input.GetAxis("Mouse Y");
        rotateDirectionY = Mathf.Clamp(rotateDirectionY, -yCamRange, yCamRange);
        cam.transform.rotation = Quaternion.Euler(new Vector3(rotateDirectionY, rotateDirectionX.y, 0));
    }

    private void Animation()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            anim.SetBool("IsMoving", true);
        }
        if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        {
            anim.SetBool("IsMoving", false);
        }

        anim.SetFloat("MoveX", Input.GetAxis("Horizontal"));
        anim.SetFloat("MoveZ", Input.GetAxis("Vertical"));
    }

    private void ServeCustomer()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && TakenMealToServe != null)
        {
            CmdServeCustomer();
        }
    }

    [Command]
    private void CmdServeCustomer()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Customer") && hit.collider.gameObject.GetComponent<CustomerController>() != null && !hit.collider.gameObject.GetComponent<CustomerController>().IsDead)
            {
                hit.collider.gameObject.GetComponent<CustomerController>().ServedMeal = TakenMealToServe;
                score += scorePerCustomer;
            }
        }
    }

    private void ChooseMeal()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;

            var layerMask = LayerMask.GetMask("Meal");

            if (Physics.Raycast(ray, out hit, 1f, layerMask))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Meal"))
                {
                    var colliderName = hit.collider.gameObject.name;
                    var parentFoodScript = hit.collider.gameObject.GetComponentInParent<FoodGenerationScript>();
                                        
                    if (colliderName == "MealRight" || colliderName == "MealLeft")

                        if (colliderName == "MealRight" && parentFoodScript.MealRight != null)
                        {
                            TakenMealToServe = parentFoodScript.currentMeals["MealRight"];
                            Destroy(parentFoodScript.MealRight);
                            parentFoodScript.MealRight = null;
                            parentFoodScript.InfoMealRight = null;
                            parentFoodScript.currentMeals.Remove("MealRight");
                            Debug.Log("Right meal picked");
                        }
                        else if (colliderName == "MealLeft" && parentFoodScript.MealLeft != null)
                        {
                            TakenMealToServe = parentFoodScript.currentMeals["MealLeft"];
                            Destroy(parentFoodScript.MealLeft);
                            parentFoodScript.MealLeft = null;
                            parentFoodScript.InfoMealLeft = null;
                            parentFoodScript.currentMeals.Remove("MealLeft");
                            Debug.Log("Left meal picked");
                        }
                }
            }
        }
    }
}
