using UnityEngine;
using UnityEngine.UI;

public class ConfigurableEnemyController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float baseSpeed = 2f;
    [SerializeField] private Slider speedSlider;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float baseShootInterval = 1f;
    [SerializeField] private Slider shootSpeedSlider;
    [SerializeField] private float bulletSpeed = 10f;

    [Header("Enemy Health Settings")]
    [SerializeField] private int maxEnemyHealth = 100;
    [SerializeField] private Slider enemyHealthSlider;
    [SerializeField] private Image enemyHealthFill;
    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private int enemyDamagePerHit = 10;

    [Header("Player Health Settings")]
    [SerializeField] private int maxPlayerHealth = 100;
    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private int playerDamagePerHit = 10;

    [Header("References")]
    [SerializeField] private Transform playerTransform;

    // Private variables
    private float currentSpeed;
    private float currentShootInterval;
    private float shootTimer = 0f;
    private int currentEnemyHealth;
    private int currentPlayerHealth;
    private bool movingToB = true;
    private Vector3 currentTarget;

    void Start()
    {
        // Initialize movement targets
        if (pointA == null || pointB == null)
        {
            Debug.LogWarning("Movement points not assigned. Using default positions.");
            if (pointA == null)
            {
                GameObject pointAObj = new GameObject("PointA");
                pointAObj.transform.position = transform.position + Vector3.left * 3f;
                pointA = pointAObj.transform;
            }

            if (pointB == null)
            {
                GameObject pointBObj = new GameObject("PointB");
                pointBObj.transform.position = transform.position + Vector3.right * 3f;
                pointB = pointBObj.transform;
            }
        }

        currentTarget = pointB.position;

        // Initialize health
        currentEnemyHealth = maxEnemyHealth;
        currentPlayerHealth = maxPlayerHealth;

        // Initialize UI
        InitializeUI();

        // Set initial values
        UpdateSpeed();
        UpdateShootSpeed();
    }

    void Update()
    {
        // Movement
        HandleMovement();

        // Shooting
        HandleShooting();

        // Update UI
        UpdateHealthUI();
    }

    void HandleMovement()
    {
        // Calculate current speed based on slider
        currentSpeed = baseSpeed * (speedSlider != null ? speedSlider.value : 1f);

        // Move towards current target
        transform.position = Vector3.MoveTowards(
            transform.position,
            currentTarget,
            currentSpeed * Time.deltaTime
        );

        // Check if reached target and switch direction
        if (Vector3.Distance(transform.position, currentTarget) < 0.1f)
        {
            if (movingToB)
            {
                currentTarget = pointA.position;
                movingToB = false;
            }
            else
            {
                currentTarget = pointB.position;
                movingToB = true;
            }
        }
    }

    void HandleShooting()
    {
        // Calculate current shoot interval based on slider
        currentShootInterval = baseShootInterval / (shootSpeedSlider != null ? shootSpeedSlider.value : 1f);

        // Update shoot timer
        shootTimer += Time.deltaTime;

        if (shootTimer >= currentShootInterval && playerTransform != null)
        {
            ShootAtPlayer();
            shootTimer = 0f;
        }
    }

    void ShootAtPlayer()
    {
        if (bulletPrefab == null || bulletSpawnPoint == null || playerTransform == null)
        {
            Debug.LogWarning("Shooting components not properly assigned!");
            return;
        }

        // Instantiate bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);

        // Calculate direction to player
        Vector3 direction = (playerTransform.position - bulletSpawnPoint.position).normalized;

        // Get bullet script and configure it
        EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();
        if (bulletScript == null)
        {
            bulletScript = bullet.AddComponent<EnemyBullet>();
        }

        bulletScript.Initialize(direction * bulletSpeed, gameObject.tag);
    }

    public void TakeEnemyDamage(int damage)
    {
        currentEnemyHealth -= damage;
        currentEnemyHealth = Mathf.Max(0, currentEnemyHealth);

        if (currentEnemyHealth <= 0)
        {
            EnemyDefeated();
        }
    }

    public void TakePlayerDamage(int damage)
    {
        currentPlayerHealth -= damage;
        currentPlayerHealth = Mathf.Max(0, currentPlayerHealth);

        if (currentPlayerHealth <= 0)
        {
            PlayerDefeated();
        }
    }

    void EnemyDefeated()
    {
        Debug.Log("Enemy Defeated!");
        // Add any enemy defeat logic here (animation, sound, etc.)
        gameObject.SetActive(false);
    }

    void PlayerDefeated()
    {
        Debug.Log("Player Defeated!");
        // Add any player defeat logic here (game over screen, etc.)
    }

    void InitializeUI()
    {
        // Enemy health slider
        if (enemyHealthSlider != null)
        {
            enemyHealthSlider.minValue = 0;
            enemyHealthSlider.maxValue = maxEnemyHealth;
            enemyHealthSlider.value = currentEnemyHealth;
        }

        // Player health slider
        if (playerHealthSlider != null)
        {
            playerHealthSlider.minValue = 0;
            playerHealthSlider.maxValue = maxPlayerHealth;
            playerHealthSlider.value = currentPlayerHealth;
        }

        // Speed slider
        if (speedSlider != null)
        {
            speedSlider.minValue = 0.1f;
            speedSlider.maxValue = 3f;
            speedSlider.value = 1f;
            speedSlider.onValueChanged.AddListener(delegate { UpdateSpeed(); });
        }

        // Shoot speed slider
        if (shootSpeedSlider != null)
        {
            shootSpeedSlider.minValue = 0.1f;
            shootSpeedSlider.maxValue = 5f;
            shootSpeedSlider.value = 1f;
            shootSpeedSlider.onValueChanged.AddListener(delegate { UpdateShootSpeed(); });
        }
    }

    void UpdateHealthUI()
    {
        // Update enemy health slider
        if (enemyHealthSlider != null)
        {
            enemyHealthSlider.value = currentEnemyHealth;

            // Update health bar color
            if (enemyHealthFill != null)
            {
                float healthPercentage = (float)currentEnemyHealth / maxEnemyHealth;
                enemyHealthFill.color = Color.Lerp(lowHealthColor, fullHealthColor, healthPercentage);
            }
        }

        // Update player health slider
        if (playerHealthSlider != null)
        {
            playerHealthSlider.value = currentPlayerHealth;
        }
    }

    public void UpdateSpeed()
    {
        if (speedSlider != null)
        {
            currentSpeed = baseSpeed * speedSlider.value;
        }
    }

    public void UpdateShootSpeed()
    {
        if (shootSpeedSlider != null)
        {
            currentShootInterval = baseShootInterval / shootSpeedSlider.value;
        }
    }

    // Public methods to modify settings
    public void SetEnemyDamage(int damage)
    {
        enemyDamagePerHit = damage;
    }

    public void SetPlayerDamage(int damage)
    {
        playerDamagePerHit = damage;
    }

    public void SetBaseSpeed(float speed)
    {
        baseSpeed = speed;
        UpdateSpeed();
    }

    public void SetShootInterval(float interval)
    {
        baseShootInterval = interval;
        UpdateShootSpeed();
    }

    void OnTriggerEnter(Collider other)
    {
        HandleCollision(other.gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.gameObject);
    }

    void HandleCollision(GameObject other)
    {
        // Check if hit by bullet
        Bullet bullet = other.GetComponent<Bullet>();
        if (bullet != null)
        {
            // Check who shot the bullet
            if (bullet.shooterTag == "Player")
            {
                // Enemy takes damage from player's bullet
                TakeEnemyDamage(enemyDamagePerHit);
                Destroy(other.gameObject);
            }
            else if (bullet.shooterTag == "Enemy")
            {
                // Check if enemy bullet hit the enemy (friendly fire)
                if (other.CompareTag("EnemyBullet"))
                {
                    TakeEnemyDamage(enemyDamagePerHit);
                    Destroy(other.gameObject);
                }
                // Check if enemy bullet hit the player
                else if (other.CompareTag("Player"))
                {
                    TakePlayerDamage(playerDamagePerHit);
                    Destroy(other.gameObject);
                }
            }
        }
    }
}

// Bullet base class
public class Bullet : MonoBehaviour
{
    public string shooterTag;
    public float speed = 10f;
    public float lifetime = 3f;
    protected Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.useGravity = false;
        Destroy(gameObject, lifetime);
    }

    public virtual void Initialize(Vector3 velocity, string shooter)
    {
        shooterTag = shooter;
        if (rb != null)
        {
            rb.linearVelocity = velocity;
        }
    }
}

// Enemy bullet class
public class EnemyBullet : Bullet
{
    void OnTriggerEnter(Collider other)
    {
        // Don't destroy if hitting another enemy bullet or the shooter
        if (other.CompareTag("EnemyBullet") || other.CompareTag(shooterTag))
            return;

        // Check if hit player
        if (other.CompareTag("Player"))
        {
            // Player takes damage (handled by ConfigurableEnemyController)
            Destroy(gameObject);
        }

        // Destroy on hitting anything else
        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}

// Player bullet class (you need to attach this to player's bullets)
public class PlayerBullet : Bullet
{
    void OnTriggerEnter(Collider other)
    {
        // Don't destroy if hitting another player bullet or the shooter
        if (other.CompareTag("PlayerBullet") || other.CompareTag(shooterTag))
            return;

        // Check if hit enemy
        if (other.CompareTag("Enemy"))
        {
            // Enemy takes damage (handled by ConfigurableEnemyController)
            Destroy(gameObject);
        }

        // Destroy on hitting anything else
        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}