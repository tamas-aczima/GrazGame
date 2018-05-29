﻿using Assets.Scripts;
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
    [SerializeField] private Text scoreText;
    [SerializeField] private Text minuteText;
    [SerializeField] private Text secondText;
    private int score = 0;
    private GameManager gameManager;

    public Meal TakenMealToServe;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        anim = GetComponent<Animator>();
        charController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        UpdateScore();
    }

    private void Update()
    {
        if (!hasAuthority)
        {
            cam.enabled = false;
            return;
        }

        cam.enabled = true;
        Move();
        Rotate();
        Animation();
        ChooseMeal();
        ServeCustomer();
        UpdateTimer();
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

    private void UpdateTimer()
    {
        if (minuteText.text != ((int) gameManager.GameTimer / 60).ToString())
        {
            minuteText.GetComponent<Animator>().Play("Animation");
        }

        minuteText.text = ((int)gameManager.GameTimer / 60).ToString();

        if (((int) gameManager.GameTimer / 60) < 1 && ((int) gameManager.GameTimer % 60) <= 30 && secondText.text != ((int) gameManager.GameTimer % 60).ToString())
        {
            secondText.GetComponent<Animator>().Play("Animation");
        }

        if (((int)gameManager.GameTimer % 60).ToString().Length == 1)
        {
            secondText.text = "0" + ((int)gameManager.GameTimer % 60).ToString();
        }
        else
        {
            secondText.text = ((int)gameManager.GameTimer % 60).ToString();
        }
    }

    private void ServeCustomer()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Customer") && hit.collider.gameObject.GetComponent<CustomerController>() != null && !hit.collider.gameObject.GetComponent<CustomerController>().IsDead)
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
        scoreText.GetComponent<Animator>().Play("Animation");
        scoreText.text = score.ToString();
    }

    private void ChooseMeal()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
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
                            parentFoodScript.currentMeals.Remove("MealRight");
                            Debug.Log("Right meal picked");
                        }
                        else if (colliderName == "MealLeft" && parentFoodScript.MealLeft != null)
                        {
                            TakenMealToServe = parentFoodScript.currentMeals["MealLeft"];
                            Destroy(parentFoodScript.MealLeft);
                            parentFoodScript.MealLeft = null;
                            parentFoodScript.currentMeals.Remove("MealLeft");
                            Debug.Log("Left meal picked");
                        }
                }
            }
        }
    }
}
