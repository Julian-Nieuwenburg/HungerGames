using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 4f;
    public float sprintSpeed = 14f;
    public float maxVelocityChange = 10f;
    [Space]
    public float jumpHeight = 5f;

    private Vector2 input;
    private Rigidbody rb;
    private Animator animator;

    private bool sprinting;
    private bool jumping;
    private bool grounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input.Normalize();

        sprinting = Input.GetButton("Sprint");
        if (grounded)
        {
            jumping = Input.GetButtonDown("Jump");
        }

        // Update animator parameters
        bool isMoving = input.magnitude > 0;
        animator.SetBool("isWalking", isMoving && !sprinting && !jumping);
        animator.SetBool("isSprinting", isMoving && sprinting && !jumping);
        animator.SetBool("isJumping", jumping);
        animator.SetBool("isIdle", !isMoving && !sprinting && !jumping && grounded);
        animator.SetBool("isFalling", !grounded && !jumping);
    }

    private void FixedUpdate()
    {
        if (grounded && jumping)
        {
            rb.AddForce(Vector3.up * Mathf.Sqrt(2 * jumpHeight * Physics.gravity.magnitude), ForceMode.VelocityChange);
            grounded = false;
            jumping = false;
        }

        Vector3 movement = CalculateMovement(sprinting ? sprintSpeed : walkSpeed);
        if (movement != Vector3.zero)
        {
            rb.AddForce(movement, ForceMode.VelocityChange);
        }
    }

    Vector3 CalculateMovement(float speed)
    {
        Vector3 targetVelocity = new Vector3(input.x, 0, input.y);
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity *= speed;

        Vector3 velocity = rb.linearVelocity;

        if (input.magnitude > 0.5f)
        {
            Vector3 velocityChange = targetVelocity - velocity;
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            return velocityChange;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                grounded = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        grounded = false;
    }
}
