using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Stealth / Crouch")]
    public bool isCrouching = false; // pode agachar-se?
    public float crouchSpeedMultiplier = 0.4f; // andar devagar
    public KeyCode crouchKey = KeyCode.LeftControl; // tecla para agachamento
    public Transform playerHead;          // a tua camera ou objeto "Head"
    public float headCrouchOffset = -0.5f; // quanto desce ao agachar
    public float headMoveSpeed = 8f;       // velocidade do movimento
    private Vector3 headOriginalLocalPos;
    private Vector3 headCrouchLocalPos;

    [Header("Movement")]
    public float moveSpeed = 5f; // velocidade de movimento
    public float maxSpeed = 5f; // velocidade máxima
    public float fatigueSpeedPenalty = 0.9f; // penalidade de velocidade por fadiga
    public float jumpForce = 5f; // força de salto
    public float groundCheckRadius = 0.5f; // raio de verificação do chão
    public Transform groundCheck; // chão
    public LayerMask groundLayer; // camada do chão
    public float inputX; // entrada horizontal
    public float inputZ; // entrada vertical
    public bool canWalk = true;
    public bool isFatigued = false;
    public bool isMoving = false;
    public bool isStressed = false;

    [Header("Camera")]
    public Camera cam; // camara do jogador

    [Header("Footsteps")]
    public AudioSource footstepsSource;     // AudioSource com o clip longo dos passos (Loop ON, PlayOnAwake OFF)
    public PlayerSounds playerSounds;   // script de eco ligado ao mesmo objeto / mesma fonte
    public PlayerFootsteps footsteps;      // script que avisa a IA
    public float footstepInterval = 0.35f; // intervalo para eventos de eco para a IA

    float horizontalInput;
    bool isGrounded;
    Rigidbody rb;
    float footstepTimer = 0f;
    public bool canplay = true;
    private PlayerHealth playerHealth;

    void Awake()
    {
        canplay = true;
        rb = GetComponent<Rigidbody>();
        rb.rotation = transform.rotation;

        if (footstepsSource == null)
            footstepsSource = GetComponent<AudioSource>();

        if (playerSounds == null)
            playerSounds = GetComponent<PlayerSounds>();

        if(playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

            // guarda a posição original da cabeça/câmara em espaço local
            headOriginalLocalPos = playerHead.localPosition;
            headCrouchLocalPos = headOriginalLocalPos + new Vector3(0f, headCrouchOffset, 0f);

    }

    void Update()
    {
        Vector3 targetPos = isCrouching ? headCrouchLocalPos : headOriginalLocalPos;

        // move suavemente a cabeça/câmara
        playerHead.localPosition = Vector3.Lerp(
            playerHead.localPosition,
            targetPos,
            headMoveSpeed * Time.deltaTime
        );

        float dt = Time.deltaTime;
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");

        Vector3 camForward = cam.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cam.transform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 moveDir = camForward * inputZ + camRight * inputX;
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        if (isFatigued) 
        {
            moveSpeed = moveSpeed * Mathf.Pow(fatigueSpeedPenalty, dt * 5);
            if(moveSpeed < 3) 
            {
                moveSpeed = 3f;
            }
            if (isCrouching) 
            {
                moveSpeed = 3.5f;
            }
        }
        else moveSpeed = 10f;


        float currentSpeed = isCrouching ? moveSpeed * (crouchSpeedMultiplier = isFatigued ? 1 : crouchSpeedMultiplier)  : moveSpeed;
        
        Vector3 move = moveDir * currentSpeed * dt;
        transform.Translate(move, Space.World);
        
        isMoving = moveDir.sqrMagnitude > 0.001f && isGrounded && !isCrouching;

        // SOM DE PASSOS (fonte principal, loop)
        if (isMoving)
        {
            if (footstepsSource != null && !footstepsSource.isPlaying && canplay)
                footstepsSource.Play();

            footstepTimer += dt;
            if (footstepTimer >= footstepInterval)
            {
                if (footsteps != null && canplay)
                    footsteps.PlayFootstep();

                footstepTimer = 0f;
            }
        }
        else
        {
            footstepTimer = 0f;
            if (footstepsSource != null && footstepsSource.isPlaying)
                footstepsSource.Stop();
        }

        // Eco (andar->parar)
        if (playerSounds != null && canplay)
        {
            //Se parar de andar, avisar o script de eco para iniciar o eco
            playerSounds.UpdatePlayingState(isMoving);
        }

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
