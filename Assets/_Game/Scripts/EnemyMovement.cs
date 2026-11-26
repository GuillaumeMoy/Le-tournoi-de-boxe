using UnityEngine;

/// <summary>
/// Boxer-style enemy movement: walks toward the player, stands and punches
/// (EnemyAttack does the punching), then backs off for a moment and comes in again.
/// Only moves left and right. Stays inside two limits so it can't leave the ring.
/// </summary>
public class EnemyMovement : MonoBehaviour
{
    [Header("Who to fight (found automatically if left empty)")]
    public PlayerHealth player;

    [Header("Speed")]
    public float approachSpeed = 3f;
    public float retreatSpeed = 4f;

    [Header("Distances")]
    [Tooltip("The enemy walks in until it is this close, then stands and punches")]
    public float fightDistance = 2f;
    [Tooltip("The enemy never gets closer than this, so it can't walk into the player")]
    public float minDistance = 1.6f;
    [Tooltip("How long the enemy backs off after each punch (seconds)")]
    public float retreatTime = 1f;

    [Tooltip("Chance per second that the enemy suddenly backs off (set per opponent by EnemyBoxer)")]
    [Range(0f, 1f)] public float randomRetreatChance = 0f;

    [Header("Ring limits (measured from where the enemy starts)")]
    public float moveLeft = 6f;
    public float moveRight = 1f;

    private Rigidbody2D rb;
    private EnemyAttack attack;
    private Vector2 startPosition;
    private float leftLimit;
    private float rightLimit;
    private float retreatTimer;

    void Awake()
    {
        if (player == null)
            player = FindFirstObjectByType<PlayerHealth>();

        attack = GetComponent<EnemyAttack>();

        // The enemy needs a Rigidbody2D to move smoothly. Add one if missing,
        // and make it "Kinematic" so gravity and pushing don't affect it.
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        startPosition = transform.position;
        leftLimit = startPosition.x - moveLeft;
        rightLimit = startPosition.x + moveRight;
    }

    void OnEnable()
    {
        // Every round the enemy starts back at its starting spot
        transform.position = startPosition;
        if (rb != null)
            rb.position = startPosition;

        retreatTimer = 0f;

        if (attack != null)
            attack.Punched += StartRetreat;
    }

    void OnDisable()
    {
        if (attack != null)
            attack.Punched -= StartRetreat;
    }

    void StartRetreat()
    {
        retreatTimer = retreatTime;
    }

    void FixedUpdate()
    {
        // Stand still during the countdown and the shop
        if (!RoundManager.FightIsOn || player == null || player.health <= 0)
            return;

        // Stand still while winding up a punch
        if (attack != null && attack.IsWindingUp)
            return;

        float x = rb.position.x;
        float playerX = player.transform.position.x;
        float distance = Mathf.Abs(playerX - x);
        float towardPlayer = Mathf.Sign(playerX - x);

        // Now and then, step back out of range (makes the enemy harder to pin down)
        if (retreatTimer <= 0f && distance < fightDistance * 1.3f && Random.value < randomRetreatChance * Time.fixedDeltaTime)
            retreatTimer = Random.Range(0.5f, 1.1f);

        if (retreatTimer > 0f)
        {
            // Back off (after a punch, or the random step back above)
            retreatTimer -= Time.fixedDeltaTime;
            x -= towardPlayer * retreatSpeed * Time.fixedDeltaTime;
        }
        else if (distance > fightDistance)
        {
            // Walk in toward the player
            x += towardPlayer * approachSpeed * Time.fixedDeltaTime;
        }
        else if (distance < minDistance)
        {
            // Player walked into us: give a little room
            x -= towardPlayer * approachSpeed * Time.fixedDeltaTime;
        }

        x = Mathf.Clamp(x, leftLimit, rightLimit);
        rb.MovePosition(new Vector2(x, rb.position.y));
    }

    // Draws the ring limits as red lines in the Scene view when the enemy is selected
    void OnDrawGizmosSelected()
    {
        float start = Application.isPlaying ? startPosition.x : transform.position.x;
        float y = transform.position.y;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(start - moveLeft, y - 3f, 0f), new Vector3(start - moveLeft, y + 3f, 0f));
        Gizmos.DrawLine(new Vector3(start + moveRight, y - 3f, 0f), new Vector3(start + moveRight, y + 3f, 0f));
    }
}
