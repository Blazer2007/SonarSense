using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Stealth / Crouch")]
    public bool isCrouching = false; // está agachado?
    public bool isStressed = false;
    public bool isFatigued = false;
    public bool isMoving = false;
    public float crouchSpeedMultiplier = 0.4f; // andar devagar
    public KeyCode crouchKey = KeyCode.LeftControl; // tecla para agachamento
    public Transform playerHead;          // a tua camera ou objeto "Head"
    public float headCrouchOffset = -0.5f; // quanto desce ao agachar
    public float headMoveSpeed = 8f;       // velocidade do movimento
    private Vector3 headOriginalLocalPos;
    private Vector3 headCrouchLocalPos;
    
    public float baseNoise = 1f;
    public float maxExtraNoise = 3f;
    public float noiseLevel; // lido pelos inimigos

    [Header("Stun System")]
    public bool canWalk = true;
    public bool canCrouch = true; // pode agachar-se?
    public bool isStunned = false;
    public float stunTimer = 0f;
    public float slowSpeedMultiplier = 0.3f;  // velocidade reduzida durante stun

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
        if (isStunned)
        {
            HandleSlowMovement(); // movimento lento durante stun
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f)
            {
                isStunned = false;
                canWalk = true;
                canCrouch = true;
            }
            // ainda processa input básico da câmara, mas sem movimento
            UpdateCameraHead();
            return; // SAI CEDO - ignora todo o resto
        }

        float dt = Time.deltaTime;
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");

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

        if (isGrounded && Input.GetButtonDown("Jump"))
            Jump();

        if (canCrouch)
        {
            isCrouching = Input.GetKey(crouchKey);
        }
        else
        {
            isCrouching = false; // força a levantar durante stun/recuperação
        }
        if (canWalk)
        {
            HandleMovement();
        }
        UpdateCameraHead();
        HandleFootsteps();
        HandleSounds();
    }
    void HandleSounds()
    {
        if (playerSounds != null && canplay)
            playerSounds.UpdatePlayingState(isMoving);
    }
    void HandleFootsteps()
    {
        if (isStunned || !isMoving)
        {
            if (footstepsSource != null && footstepsSource.isPlaying)
                footstepsSource.Stop();
            footstepTimer = 0f;
            return;
        }

        if (footstepsSource != null && !footstepsSource.isPlaying && canplay)
            footstepsSource.Play();

        footstepTimer += Time.deltaTime;
        if (footstepTimer >= footstepInterval)
        {
            if (footsteps != null && canplay)
                footsteps.PlayFootstep();
            footstepTimer = 0f;
        }
    }
    void HandleSlowMovement()
    {
        inputX = Input.GetAxis("Horizontal") * 0.2f;
        inputZ = Input.GetAxis("Vertical") * 0.2f;

        Vector3 camForward = cam.transform.forward;
        camForward.y = 0f; camForward.Normalize();
        Vector3 camRight = cam.transform.right;
        camRight.y = 0f; camRight.Normalize();

        Vector3 moveDir = camForward * inputZ + camRight * inputX;
        float panicSpeed = moveSpeed * slowSpeedMultiplier * 0.5f; // extra lento
        if (isCrouching) panicSpeed *= 0.7f;

        Vector3 move = moveDir * panicSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);

        isMoving = false; // sem footsteps durante stun
    }
    void HandleMovement()
    {
        Vector3 camForward = cam.transform.forward;
        camForward.y = 0f; camForward.Normalize();
        Vector3 camRight = cam.transform.right;
        camRight.y = 0f; camRight.Normalize();

        Vector3 moveDir = camForward * inputZ + camRight * inputX;
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        float currentMoveSpeed = moveSpeed;
        if (isFatigued)
        {
            currentMoveSpeed *= fatigueSpeedPenalty;
            if (currentMoveSpeed < 3f) currentMoveSpeed = 3f;
        }
        if (isCrouching) currentMoveSpeed *= crouchSpeedMultiplier;

        Vector3 move = moveDir * currentMoveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);

        isMoving = moveDir.sqrMagnitude > 0.001f && isGrounded && !isCrouching;
    }
    void UpdateCameraHead()
    {
        Vector3 targetPos = isCrouching ? headCrouchLocalPos : headOriginalLocalPos;
        playerHead.localPosition = Vector3.Lerp(playerHead.localPosition, targetPos, headMoveSpeed * Time.deltaTime);
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
