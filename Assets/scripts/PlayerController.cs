using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float maxSpeed;
    public float jumpForce;
    public float groundCheckRadius = 0.5f;
    public float horizontalInput;
    private bool isGrounded;
    public Transform groundCheck;
    public LayerMask groundLayer;
    private Rigidbody rb;
    public Camera cam;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.rotation = transform.rotation;
    }
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        float dt = Time.deltaTime;
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // direções da câmara no plano XZ
        Vector3 camForward = cam.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cam.transform.right;
        camRight.y = 0f;
        camRight.Normalize();

        // direção desejada de movimento em relação à câmara
        Vector3 moveDir = camForward * v + camRight * h;
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        // mover com Rigidbody (recomendado) OU Translate
        Vector3 move = moveDir * moveSpeed * dt;
        transform.Translate(move, Space.World);

        if (isGrounded && Input.GetButtonDown("Jump")) Jump();

    }

    private void FixedUpdate()
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
