using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Stealth / Crouch")]
    public bool isCrouching = false;
    public float crouchSpeedMultiplier = 0.4f; // andar devagar
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float maxSpeed = 5f;
    public float jumpForce = 5f;
    public float groundCheckRadius = 0.5f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Camera")]
    public Camera cam;

    [Header("Footsteps / Echo")]
    public AudioSource footstepsSource;     // AudioSource com o clip longo dos passos (Loop ON, PlayOnAwake OFF)
    public PlayerEchoSounds playerEcho;   // script de eco ligado ao mesmo objeto / mesma fonte
    public PlayerFootsteps footsteps;      // script que avisa a IA
    public float footstepInterval = 0.35f; // intervalo para eventos de eco para a IA

    float horizontalInput;
    bool isGrounded;
    Rigidbody rb;
    float footstepTimer = 0f;
    bool wasWalking = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.rotation = transform.rotation;

        if (footstepsSource == null)
            footstepsSource = GetComponent<AudioSource>();

        if (playerEcho == null)
            playerEcho = GetComponent<PlayerEchoSounds>();
    }

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        float dt = Time.deltaTime;
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 camForward = cam.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cam.transform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 moveDir = camForward * v + camRight * h;
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        float currentSpeed = isCrouching ? moveSpeed * crouchSpeedMultiplier : moveSpeed;
        Vector3 move = moveDir * currentSpeed * dt;
        transform.Translate(move, Space.World);

        bool isMoving = moveDir.sqrMagnitude > 0.001f && isGrounded && !isCrouching;

        // SOM DE PASSOS (fonte principal, loop)
        if (isMoving)
        {
            if (footstepsSource != null && !footstepsSource.isPlaying)
                footstepsSource.Play();

            footstepTimer += dt;
            if (footstepTimer >= footstepInterval)
            {
                if (footsteps != null)
                    footsteps.PlayFootstep();

                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = 0f;
        }

        // Eco (andar->parar)
        if (playerEcho != null)
        {
            //Se parar de andar, avisar o script de eco para iniciar o eco
            playerEcho.UpdatePlayingState(isMoving);
        }

        wasWalking = isMoving;

        if (isGrounded && Input.GetButtonDown("Jump"))
            Jump();

        isCrouching = Input.GetKey(crouchKey);
    }

    void FixedUpdate()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);

        Vector3 velocity = rb.linearVelocity;
        float targetVelX = horizontalInput * moveSpeed;
        float newVelX = Mathf.MoveTowards(velocity.x, targetVelX, 50f * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector3(Mathf.Clamp(newVelX, -maxSpeed, maxSpeed), velocity.y, 0f);
    }

    void Jump()
    {
        Vector3 vel = rb.linearVelocity;
        vel.y = 0f;
        rb.linearVelocity = vel;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}
