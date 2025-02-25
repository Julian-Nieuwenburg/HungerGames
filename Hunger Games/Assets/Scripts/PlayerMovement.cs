using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public float walkSpeed = 4f;
    public float sprintSpeed = 14f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public Transform groundCheck;
    public float groundDistance = 0.9f;
    public LayerMask groundMask;
    public float slopeLimit = 45f;

    private Vector2 input;
    private Animator animator;
    private Vector3 velocity;
    private bool isGrounded;
    private bool sprinting;
    private bool jumping;

    void Start()
    {
        animator = GetComponent<Animator>();
        controller.slopeLimit = slopeLimit;
    }

    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        sprinting = Input.GetButton("Sprint");
        jumping = Input.GetButtonDown("Jump") && isGrounded;

        bool isMoving = input.magnitude > 0;
        animator.SetBool("isWalking", isMoving && !sprinting && isGrounded);
        animator.SetBool("isSprinting", isMoving && sprinting && isGrounded);
        animator.SetBool("isJumping", !isGrounded);
        animator.SetBool("isIdle", !isMoving && isGrounded);
    }

    void FixedUpdate()
    {
        MovePlayer();
        
    }

    void MovePlayer()
    {
        //isGrounded = CheckIfGrounded();
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.5f, groundMask);
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float speed = sprinting ? sprintSpeed : walkSpeed;
        Vector3 move = transform.right * input.x + transform.forward * input.y;
        controller.Move(move * speed * Time.deltaTime);

        if (jumping)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    //bool CheckIfGrounded()
    //{
        //RaycastHit hit;
        //if (Physics.Raycast(groundCheck.position, Vector3.down, out hit, groundDistance, groundMask))
        //{
        //    float angle = Vector3.Angle(hit.normal, Vector3.up);
        //    return angle <= slopeLimit;
        //}
        //return false;
       // return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    //}
}
