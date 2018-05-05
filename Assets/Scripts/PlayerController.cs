using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private Animator anim = null;
    private CharacterController charController = null;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 rotateDirection = Vector3.zero;
    [SerializeField] private float walkSpeed;

    private void Start()
    {
        anim = GetComponent<Animator>();
        charController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Move();
        Rotate();
        Animation();
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
}
