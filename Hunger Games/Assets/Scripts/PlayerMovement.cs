using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 4f;
    public float sprintSpeed = 14f;
    public float maxVelocityChange = 10f;
    [Space]
    public float jumpForce = 7f;
    public Transform groundCheck;
    public LayerMask groundMask;
    [Space]
    public Camera playerCamera;

    private Vector2 input;
    private Rigidbody rb;
    private Animator animator;

    private bool sprinting;
    private bool jumping;
    private bool grounded = false;

    private float defaultHeadPosition;

    // Toegevoegde variabelen voor het controleren van hellingen
    public float slopeLimit = 45f; // Maximaal toegestaan hellingshoek voor springen
    public float groundRayDistance = 1.1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        rb.freezeRotation = true;
    }

    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        sprinting = Input.GetButton("Sprint");
        jumping = Input.GetButtonDown("Jump") && grounded;

        bool isMoving = input.magnitude > 0;
        animator.SetBool("isWalking", isMoving && !sprinting && grounded);
        animator.SetBool("isSprinting", isMoving && sprinting && grounded);
        animator.SetBool("isJumping", !grounded);
        animator.SetBool("isIdle", !isMoving && grounded);
    }

    private void FixedUpdate()
    {
        // Gebruik een Raycast om de grond te controleren
        grounded = CheckIfGrounded();

        if (jumping && grounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z); // Reset de y-velocity voor een nette sprong
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        Vector3 movement = CalculateMovement(sprinting ? sprintSpeed : walkSpeed);
        if (movement != Vector3.zero)
        {
            rb.AddForce(movement, ForceMode.VelocityChange);
        }
    }

    bool CheckIfGrounded()
    {
        RaycastHit hit;
        // Gebruik een raycast die iets verder onder de speler kijkt (groundRayDistance) om te zien of er grond is
        if (Physics.Raycast(groundCheck.position, Vector3.down, out hit, groundRayDistance, groundMask))
        {
            // Controleer of de normaal van de grond niet steiler is dan de slopeLimit
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            return angle <= slopeLimit;
        }
        return false;
    }

    Vector3 CalculateMovement(float speed)
    {
        Vector3 targetVelocity = new Vector3(input.x, 0, input.y);
        targetVelocity = transform.TransformDirection(targetVelocity) * speed;

        Vector3 velocity = rb.linearVelocity;  // Gebruik linearVelocity hier
        Vector3 velocityChange = targetVelocity - velocity;

        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;

        return velocityChange;
    }
}
