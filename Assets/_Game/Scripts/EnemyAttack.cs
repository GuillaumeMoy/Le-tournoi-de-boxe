using System.Collections;
using UnityEngine;

/// <summary>
/// Makes the enemy punch the player when the player is close.
/// The enemy flashes yellow for a moment first, so the player has a chance to step away.
/// It only attacks while a real fight is on (not during the countdown or the shop).
/// </summary>
public class EnemyAttack : MonoBehaviour
{
    [Header("Who to hit (found automatically if left empty)")]
    public PlayerHealth player;

    [Header("Attack")]
    public int damage = 10;                 // damage before the helmet
    public float attackRange = 2.5f;        // how close the player must be
    public float timeBetweenPunches = 2f;   // seconds between punches
    public float windUpTime = 0.4f;         // warning time before the punch lands

    [Header("Walking")]
    public float walkSpeed = 2f;            // how fast it walks towards the player

    [Header("Warning flash (found automatically if left empty)")]
    public SpriteRenderer spriteRenderer;
    public Color windUpColor = Color.yellow;

    private float timer;
    private bool windingUp;
    private Color originalColor = Color.white;
    private Vector3 startPosition;

    void Awake()
    {
        startPosition = transform.position;

        if (player == null)
            player = FindFirstObjectByType<PlayerHealth>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    // Runs each time the enemy appears or this script is switched on,
    // so every round starts with a fresh punch timer.
    void OnEnable()
    {
        transform.position = startPosition;   // back to its corner every round
        timer = timeBetweenPunches;
        windingUp = false;
    }

    void OnDisable()
    {
        // If the enemy was knocked out mid-warning, put its colour back
        if (windingUp && spriteRenderer != null)
            spriteRenderer.color = originalColor;

        windingUp = false;
    }

    void Update()
    {
        // No attacking during the 3-2-1 countdown or while the shop is open
        if (!RoundManager.FightIsOn)
            return;

        if (player == null || player.health <= 0 || windingUp)
            return;

        if (!PlayerInRange())
        {
            // Too far to punch: walk towards the player (left/right only, stays on the floor)
            float x = Mathf.MoveTowards(transform.position.x, player.transform.position.x, walkSpeed * Time.deltaTime);
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
            return;
        }

        timer -= Time.deltaTime;

        if (timer <= 0f)
            StartCoroutine(Punch());
    }

    IEnumerator Punch()
    {
        windingUp = true;

        if (spriteRenderer != null)
            spriteRenderer.color = windUpColor;

        yield return new WaitForSeconds(windUpTime);

        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;

        // Only lands if the player is still close (they can dodge by stepping back)
        if (player != null && player.health > 0 && PlayerInRange() && RoundManager.FightIsOn)
        {
            Debug.Log("Enemy punched the player");
            player.TakeDamage(damage);
        }

        timer = timeBetweenPunches;
        windingUp = false;
    }

    bool PlayerInRange()
    {
        return Vector2.Distance(transform.position, player.transform.position) <= attackRange;
    }

    // Draws the attack range as a yellow circle in the Scene view when the enemy is selected
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
