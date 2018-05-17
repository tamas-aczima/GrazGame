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
}
