using Assets.Scripts;
using Assets.Scripts.Classes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Animator anim = null;
    private CharacterController charController = null;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 rotateDirection = Vector3.zero;
    [SerializeField] private float walkSpeed;
    [SerializeField] private Camera cam;
    [SerializeField] private float rayDistance;

    public Meal TakenMealToServe;

    private void Start()
    {
        anim = GetComponent<Animator>();
        charController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
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
        rotateDirection += new Vector3(0, Input.GetAxis("Mouse X"), 0);
        transform.rotation = Quaternion.Euler(rotateDirection);
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

    private FoodGenerationScript foodGenerationScript;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        var restaurant = GameObject.FindGameObjectWithTag("RestaurantCollider");
        if (hit.collider.gameObject == restaurant)
        {
            Debug.Log("touching restaurant collider");
            foodGenerationScript = restaurant.gameObject.GetComponent<FoodGenerationScript>();
            if (!foodGenerationScript.MealsOffered)
                foodGenerationScript.DoMealGenerationThings();
        }
        else
        {
            Debug.Log("exit restaurant collider");
        }

        InvokeRepeating("ChooseMeal", 0.5f, 1);
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

                        if (colliderName == "MealRight")
                        {
                            TakenMealToServe = foodGenerationScript.currentMeals["MealRight"];
                            Destroy(foodGenerationScript.MealRight);
                            foodGenerationScript.MealRight = null;
                            foodGenerationScript.MealsOffered = false;
                            Debug.Log("Right meal picked");
                        }
                        else if (colliderName == "MealLeft")
                        {
                            TakenMealToServe = foodGenerationScript.currentMeals["MealLeft"];
                            Destroy(foodGenerationScript.MealLeft);
                            foodGenerationScript.MealLeft = null;
                            foodGenerationScript.MealsOffered = false;
                            Debug.Log("Left meal picked");
                        }
                }
            }
        }
    }
}
