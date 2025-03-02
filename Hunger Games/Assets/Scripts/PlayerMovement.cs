using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpForce = 5f;
    public float groundCheckDistance = 0.3f;
    public LayerMask groundMask;

    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = true; // Zorg ervoor dat zwaartekracht ingeschakeld is
    }

    void FixedUpdate()
    {
        // Controleer of de speler op de grond staat met een raycast
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask);

        // Input ophalen
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        // Bereken snelheid en richting
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        float speed = isSprinting ? sprintSpeed : walkSpeed;

        // Haal de huidige linearVelocity op (niet velocity)
        Vector3 linearVelocity = rb.linearVelocity;

        // Pas de X en Z snelheid aan voor beweging
        linearVelocity.x = move.x * speed;
        linearVelocity.z = move.z * speed;

        // Als de speler op de grond is, kan hij springen
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            linearVelocity.y = jumpForce; // Voeg kracht toe aan Y als we op de grond staan
        }

        // Zet de nieuwe linearVelocity van de Rigidbody
        rb.linearVelocity = linearVelocity; // Pas de linearVelocity direct aan
    }
}
