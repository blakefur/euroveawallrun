using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("Zipline")]
    public float ziplineSpeed = 10f;
    public float ziplinePlayerOffset = 0f;
    public float ziplineJumpForce = 7.5f;

    private Rigidbody rb;
    private CapsuleCollider col;

    private float inputX;
    private float inputZ;

    // Input queues
    private bool jumpHeld;
    private bool jumpHoldQueued;
    private bool jumpPressQueued;

    private bool grounded;
    private bool wasGrounded;
    private float coyoteTimer;

    // Zipline state
    private Zipline currentZipline;
    private bool ridingZipline;
    private float ziplineProgress;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.linearDamping = 0f;
    }

    private void Start()
    {
        remainingAirJumps = Mathf.Clamp(remainingAirJumps, 0, maxAirJumps);
    }

    private void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputZ = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (ridingZipline)
            {
                ReleaseZipline(Vector3.zero);
            }
            else if (currentZipline != null)
            {
                GrabZipline();
            }
        }

        if (ridingZipline)
        {
            if (Input.GetButtonDown("Jump"))
            {
                JumpOffZipline();
            }
        }
        else
        {
            jumpHeld = Input.GetButton("Jump");

            if (jumpHeld)
            {
                jumpHoldQueued = true;
            }

            if (Input.GetButtonDown("Jump"))
            {
                jumpPressQueued = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void FixedUpdate()
    {
        if (ridingZipline)
        {
            RideZipline();
            return;
        }

        wasGrounded = grounded;
        grounded = IsGrounded();

        if (grounded)
        {
            coyoteTimer = coyoteTime;
        }
        else
        {
            coyoteTimer -= Time.fixedDeltaTime;
        }

        if (resetAirJumpsOnImpact && grounded && !wasGrounded)
        {
            remainingAirJumps = maxAirJumps;
        }

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
            {
                horiz = Vector3.zero;
            }
            else
            {
                horiz += frictionAccel;
            }
        }

        rb.linearVelocity = new Vector3(horiz.x, v.y, horiz.z);

        if (jumpHoldQueued && (grounded || coyoteTimer > 0f))
        {
            DoJump();
            coyoteTimer = 0f;
        }
        else if (jumpPressQueued && !grounded && !wasGrounded && remainingAirJumps > 0)
        {
            DoJump();
            remainingAirJumps = Mathf.Clamp(remainingAirJumps - 1, 0, maxAirJumps);
        }

        jumpPressQueued = false;
        jumpHoldQueued = false;
    }

    private void GrabZipline()
    {
        if (currentZipline == null || !currentZipline.IsValid())
        {
            currentZipline = null;
            return;
        }

        ridingZipline = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;

        ziplineProgress = currentZipline.GetClosestProgress(transform.position);
        transform.position = currentZipline.GetPoint(ziplineProgress) + Vector3.up * ziplinePlayerOffset;
    }

    private void RideZipline()
    {
        if (currentZipline == null || !currentZipline.IsValid())
        {
            ReleaseZipline(Vector3.zero);
            return;
        }

        float length = currentZipline.GetLength();
        if (length <= 0.001f)
        {
            ReleaseZipline(Vector3.zero);
            return;
        }

        ziplineProgress += (ziplineSpeed / length) * Time.fixedDeltaTime;

        Vector3 direction = currentZipline.GetDirection();
        Vector3 point = currentZipline.GetPoint(ziplineProgress);
        transform.position = point + Vector3.up * ziplinePlayerOffset;
        rb.linearVelocity = direction * ziplineSpeed;

        if (ziplineProgress >= 1f)
        {
            ReleaseZipline(direction * ziplineSpeed);
        }
    }

    private void ReleaseZipline(Vector3 exitVelocity)
    {
        ridingZipline = false;
        currentZipline = null;
        rb.useGravity = true;
        rb.linearVelocity = exitVelocity;
    }

    private void JumpOffZipline()
    {
        if (currentZipline == null)
        {
            ReleaseZipline(Vector3.up * ziplineJumpForce);
            return;
        }

        Vector3 direction = currentZipline.GetDirection();
        Vector3 exitVelocity = direction * ziplineSpeed + Vector3.up * ziplineJumpForce;
        ReleaseZipline(exitVelocity);
    }

    private void OnTriggerEnter(Collider other)
    {
        Zipline zipline = other.GetComponentInParent<Zipline>();
        if (zipline != null && zipline.IsValid())
        {
            currentZipline = zipline;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Zipline zipline = other.GetComponentInParent<Zipline>();
        if (zipline != null && zipline == currentZipline && !ridingZipline)
        {
            currentZipline = null;
        }
    }

    private void DoJump()
    {
        Vector3 vel = rb.linearVelocity;
        if (vel.y < 0f)
        {
            vel.y = 0f;
        }

        rb.linearVelocity = new Vector3(vel.x, jumpForce, vel.z);
    }

    private bool IsGrounded()
    {
        float radius = col.radius * Mathf.Abs(transform.localScale.x);
        float height = Mathf.Max(col.height * Mathf.Abs(transform.localScale.y), radius * 2f);

        Vector3 centerWorld = transform.TransformPoint(col.center);
        Vector3 up = transform.up;
        float half = height * 0.5f - radius;

        Vector3 top = centerWorld + up * half;
        Vector3 bottom = centerWorld - up * half;

        return Physics.CapsuleCast(
            top,
            bottom,
            radius * 0.98f,
            -up,
            out _,
            groundCheckDistance,
            groundMask,
            QueryTriggerInteraction.Ignore
        );
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
