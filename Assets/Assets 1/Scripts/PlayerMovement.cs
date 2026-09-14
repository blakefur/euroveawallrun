using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float groundAcceleration = 20f;
    public float airAcceleration = 6f;

    [Header("Friction (horizontal only)")]
    public float groundFriction = 12f;

    [Header("Jump")]
    public float jumpForce = 7.5f;
    public float coyoteTime = 0.1f;

    [Header("Grounding")]
    public LayerMask groundMask = ~0;
    public float groundCheckDistance = 0.15f;

    [Header("Air Jumps")]
    public int maxAirJumps = 5;
    public int remainingAirJumps = 2;
    public bool resetAirJumpsOnImpact = true;

    Rigidbody rb;
    CapsuleCollider col;

    float inputX, inputZ;

    // Input queues
    bool jumpHeld;           // raw held state
    bool jumpHoldQueued;     // request for ground/coyote jump while held
    bool jumpPressQueued;    // request for air-jump on press

    bool grounded;
    bool wasGrounded;
    float coyoteTimer;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.linearDamping = 0f;
    }

    void Start()
    {
        remainingAirJumps = Mathf.Clamp(remainingAirJumps, 0, maxAirJumps);



      
    }

    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputZ = Input.GetAxisRaw("Vertical");

        // Queue ground/coyote jump while held
        jumpHeld = Input.GetButton("Jump");
        if (jumpHeld) jumpHoldQueued = true;

        // Queue air-jump only on press
        if (Input.GetButtonDown("Jump")) jumpPressQueued = true;

        if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void FixedUpdate()
    {
        wasGrounded = grounded;
        grounded = IsGrounded();

        if (grounded) coyoteTimer = coyoteTime;
        else coyoteTimer -= Time.fixedDeltaTime;

        if (resetAirJumpsOnImpact && grounded && !wasGrounded)
        {
            remainingAirJumps = maxAirJumps;
      
        }

        // Movement
        Vector3 moveDir = (transform.right * inputX + transform.forward * inputZ).normalized;
        Vector3 targetVel = moveDir * moveSpeed;

        Vector3 v = rb.linearVelocity;
        Vector3 horiz = new Vector3(v.x, 0f, v.z);

        float maxDelta = (grounded ? groundAcceleration : airAcceleration) * Time.fixedDeltaTime;
        Vector3 needed = targetVel - horiz;
        Vector3 delta = Vector3.ClampMagnitude(needed, maxDelta);
        horiz += delta;

        if (grounded && targetVel.sqrMagnitude < 0.01f && horiz.sqrMagnitude > 0f)
        {
            Vector3 frictionAccel = -horiz.normalized * groundFriction * Time.fixedDeltaTime;
            if (frictionAccel.magnitude > horiz.magnitude)
                horiz = Vector3.zero;
            else
                horiz += frictionAccel;
        }

        rb.linearVelocity = new Vector3(horiz.x, v.y, horiz.z);

        // Jumping
        bool didJump = false;

        // Ground/coyote jump while held (never consumes air jumps)
        if (jumpHoldQueued && (grounded || coyoteTimer > 0f))
        {
            DoJump();
            didJump = true;
            coyoteTimer = 0f; // consume coyote this frame
        }
        // Air jump only on press, only if we started this tick in the air
        else if (jumpPressQueued && !grounded && !wasGrounded && remainingAirJumps > 0)
        {
            DoJump();
            remainingAirJumps = Mathf.Clamp(remainingAirJumps - 1, 0, maxAirJumps);
           
            didJump = true;
        }

        // Consume queues for this physics step
        jumpPressQueued = false;
        jumpHoldQueued = false; // will be re-queued by Update() next frame if still held
    }

    void DoJump()
    {
        Vector3 vel = rb.linearVelocity;
        if (vel.y < 0f) vel.y = 0f;
        rb.linearVelocity = new Vector3(vel.x, jumpForce, vel.z);
    }

    bool IsGrounded()
    {
        float radius = col.radius * Mathf.Abs(transform.localScale.x);
        float height = Mathf.Max(col.height * Mathf.Abs(transform.localScale.y), radius * 2f);

        Vector3 centerWorld = transform.TransformPoint(col.center);
        Vector3 up = transform.up;
        float half = height * 0.5f - radius;

        Vector3 top = centerWorld + up * half;
        Vector3 bottom = centerWorld - up * half;

        return Physics.CapsuleCast(top, bottom, radius * 0.98f, -up, out _, groundCheckDistance, groundMask, QueryTriggerInteraction.Ignore);
    }


    public void SetAirJumps(int i)
    {
        remainingAirJumps = Mathf.Clamp(i, 0, maxAirJumps);
 
    }

    public void AddAirJumps(int i)
    {
        remainingAirJumps = Mathf.Clamp(remainingAirJumps + i, 0, maxAirJumps);
     
    }
}
