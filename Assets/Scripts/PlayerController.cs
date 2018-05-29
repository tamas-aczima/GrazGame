using Assets.Scripts;
using Assets.Scripts.Classes;
using System;
using System.Linq;
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
    [SerializeField] private Text scoreText;
    private int score = 0;

    public Meal TakenMealToServe;

    private void Start()
    {
        anim = GetComponent<Animator>();
        charController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        UpdateScore();
    }

    private void Update()
    {
        if (!hasAuthority){
            cam.enabled = false;
            return;
        }

        cam.enabled = true;
        Move();
        Rotate();
        Animation();
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
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Customer"))
                {
                    hit.collider.gameObject.GetComponent<CustomerController>().IsServed = true;
                    score += scorePerCustomer;
                    UpdateScore();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Customer"))
                {
                    hit.collider.gameObject.GetComponent<CustomerController>().IsServed = true;
                    hit.collider.gameObject.GetComponent<CustomerController>().HasAllergen = true;
                }
            }
        }
    }

    private void UpdateScore()
    {
        scoreText.text = "Score: " + score;
    }

    private FoodGenerationScript foodGenerationScript;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        try
        {
            var restaurant = GameObject.FindGameObjectsWithTag("RestaurantCollider").FirstOrDefault(x => x == hit.collider.gameObject);

            if (restaurant != null)
            {
                foodGenerationScript = restaurant.gameObject.GetComponent<FoodGenerationScript>();
                Debug.Log("touching restaurant collider");
                if (!foodGenerationScript.MealsOffered)
                    foodGenerationScript.DoMealGenerationThings();
                InvokeRepeating("ChooseMeal", 0.5f, 1);
            }
            else
            {
                Debug.Log("exit restaurant collider");
                CancelInvoke("ChooseMeal");
                if (foodGenerationScript != null)
                    foodGenerationScript.HideOfferedMeals();
            }

        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private void ChooseMeal()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction);
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Ray ray = new Ray(gameObject.transform.position, gameObject.transform.forward);
            RaycastHit hit;

            var layerMask = LayerMask.GetMask("Meal");

            if (Physics.Raycast(ray, out hit, 20f, layerMask))
            {
                Debug.Log("layer hit: " + hit.collider.gameObject.layer);
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Meal"))
                {
                    var colliderName = hit.collider.gameObject.name;
                    if (colliderName == "MealRight" || colliderName == "MealLeft")

                        if (colliderName == "MealRight" && foodGenerationScript.MealRight != null)
                        {
                            TakenMealToServe = foodGenerationScript.currentMeals["MealRight"];
                            Destroy(foodGenerationScript.MealRight);
                            foodGenerationScript.MealRight = null;
                            foodGenerationScript.MealsOffered = false;
                            foodGenerationScript.currentMeals.Remove("MealRight");
                            Debug.Log("Right meal picked");
                        }
                        else if (colliderName == "MealLeft" && foodGenerationScript.MealLeft != null)
                        {
                            TakenMealToServe = foodGenerationScript.currentMeals["MealLeft"];
                            Destroy(foodGenerationScript.MealLeft);
                            foodGenerationScript.MealLeft = null;
                            foodGenerationScript.MealsOffered = false;
                            foodGenerationScript.currentMeals.Remove("MealLeft");
                            Debug.Log("Left meal picked");
                        }
                }
            }
        }
    }
}
