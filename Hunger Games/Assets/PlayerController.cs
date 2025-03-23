using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerInput playerInput;
    PlayerInput.MainActions input;

    CharacterController controller;
    Actor actor; // Reference to the Actor component

    [Header("Controller")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 10f; // Sprint speed
    public float gravity = -9.8f;
    public float jumpHeight = 1.2f;

    Vector3 _PlayerVelocity;

    bool isGrounded;
    bool isSprinting;

    [Header("Camera")]
    public Camera cam;
    public float sensitivity = 100f; // Adjusted sensitivity value

    [Header("Storm")]
    public float damagePerSecond = 15f; // Damage per second when outside the storm

    float xRotation = 0f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        actor = GetComponent<Actor>(); // Get the Actor component

        playerInput = new PlayerInput();
        input = new PlayerInput.MainActions(playerInput);
        AssignInputs();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        // Repeat Inputs
        if (input.Attack.IsPressed())
        { Attack(); }

        SetAnimations();

        // Check if the player is outside the storm
        CheckStormDamage();
    }

    void FixedUpdate()
    {
        Vector2 movementInput = input.Movement.ReadValue<Vector2>();
        MoveInput(movementInput);
    }

    void LateUpdate()
    {
        Vector2 lookInput = input.Look.ReadValue<Vector2>();
        LookInput(lookInput);
    }

    void MoveInput(Vector2 input)
    {
        Vector3 moveDirection = new Vector3(input.x, 0, input.y);
        moveDirection = transform.TransformDirection(moveDirection);

        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;

        if (moveDirection.magnitude > 0.1f) // Only move if there is significant input
        {
            controller.Move(moveDirection * currentSpeed * Time.deltaTime);
        }

        _PlayerVelocity.y += gravity * Time.deltaTime;
        if (isGrounded && _PlayerVelocity.y < 0)
            _PlayerVelocity.y = -2f;
        controller.Move(_PlayerVelocity * Time.deltaTime);
    }

    void LookInput(Vector2 input)
    {
        float mouseX = input.x * sensitivity / 15;
        float mouseY = -input.y * sensitivity / 15; // Inverted mouseY

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void OnEnable()
    { input.Enable(); }

    void OnDisable()
    { input.Disable(); }

    void Jump()
    {
        // Adds force to the player rigidbody to jump
        if (isGrounded)
        {
            _PlayerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void Sprint(InputAction.CallbackContext context)
    {
        isSprinting = context.ReadValueAsButton();
    }

    void AssignInputs()
    {
        input.Jump.performed += ctx => Jump();
        input.Attack.started += ctx => Attack();
        input.Sprint.performed += Sprint;
        input.Sprint.canceled += Sprint;
    }

    void CheckStormDamage()
    {
        if (StormMechanism.Instance == null) return;

        float distanceFromCenter = Vector3.Distance(transform.position, StormMechanism.Instance.transform.position);
        if (distanceFromCenter > StormMechanism.Instance.CurrentRadius)
        {
            actor.TakeDamage((int)(damagePerSecond * Time.deltaTime));
        }
    }

    // ---------- //
    // ANIMATIONS //
    // ---------- //

    public const string IDLE = "Idle";
    public const string WALK = "Walk";
    public const string ATTACK1 = "Attack 1";
    public const string ATTACK2 = "Attack 2";

    string currentAnimationState;

    public void ChangeAnimationState(string newState)
    {
        // STOP THE SAME ANIMATION FROM INTERRUPTING WITH ITSELF //
        if (currentAnimationState == newState) return;

        // PLAY THE ANIMATION //
        currentAnimationState = newState;

    }

    void SetAnimations()
    {
        // If player is not attacking
        if (!attacking)
        {
            if (_PlayerVelocity.x == 0 && _PlayerVelocity.z == 0)
            { ChangeAnimationState(IDLE); }
            else
            { ChangeAnimationState(WALK); }
        }
    }

    // ------------------- //
    // ATTACKING BEHAVIOUR //
    // ------------------- //

    [Header("Attacking")]
    public float attackDistance = 3f;
    public float attackDelay = 0.4f;
    public float attackSpeed = 1f;
    public int attackDamage = 1;
    public LayerMask attackLayer;

    public GameObject hitEffect;

    bool attacking = false;
    bool readyToAttack = true;
    int attackCount;

    public void Attack()
    {
        if (!readyToAttack || attacking) return;

        readyToAttack = false;
        attacking = true;

        Invoke(nameof(ResetAttack), attackSpeed);
        Invoke(nameof(AttackRaycast), attackDelay);

        if (attackCount == 0)
        {
            ChangeAnimationState(ATTACK1);
            attackCount++;
        }
        else
        {
            ChangeAnimationState(ATTACK2);
            attackCount = 0;
        }
    }

    void ResetAttack()
    {
        attacking = false;
        readyToAttack = true;
    }

    void AttackRaycast()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, attackDistance, attackLayer))
        {
            HitTarget(hit.point);

            if (hit.transform.TryGetComponent<Actor>(out Actor T))
            { T.TakeDamage(attackDamage); }
        }
    }

    void HitTarget(Vector3 pos)
    {
        GameObject GO = Instantiate(hitEffect, pos, Quaternion.identity);
        Destroy(GO, 20);
    }
}
