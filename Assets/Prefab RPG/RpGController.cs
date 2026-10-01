using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RpGController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float gridSize = 1f;

    [Header("Collision Settings")]
    public float collisionCheckDistance = 0.4f;
    public string[] solidObjectTags = { "Solid", "Wall", "Tree", "Building" }; // Tags for solid objects

    [Header("References")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    // Private variables
    private Vector2 input;
    private bool isMoving = false;
    private Vector3 targetPosition;

    // Direction tracking
    private Vector2 lastDirection = Vector2.down;

    // Physics components
    private Rigidbody2D rb;
    private Collider2D col;

    // Animation parameters
    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");

    void Start()
    {
        // Get required components
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        // Set up Rigidbody2D for kinematic movement
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }

    void Update()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            // Prevent diagonal movement
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                input.y = 0;
            else if (Mathf.Abs(input.y) > Mathf.Abs(input.x))
                input.x = 0;

            if (input != Vector2.zero)
            {
                // Check if movement is possible before starting
                if (CanMove(input))
                {
                    StartCoroutine(Move(input));
                }
                else
                {
                    // Still update facing direction even if can't move
                    lastDirection = input;
                    UpdateAnimator(Vector2.zero, lastDirection);
                }
            }
            else
            {
                UpdateAnimator(Vector2.zero, lastDirection);
            }
        }
    }

    bool CanMove(Vector2 direction)
    {
        // Method 1: Raycast checking for specific tags
        if (CheckRaycastForSolidTags(direction))
            return false;

        // Method 2: Overlap circle checking for specific tags
        if (CheckOverlapForSolidTags(direction))
            return false;

        return true;
    }

    bool CheckRaycastForSolidTags(Vector2 direction)
    {
        // Cast a ray in the movement direction
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            direction,
            collisionCheckDistance
        );

        // Visualize the ray
        Debug.DrawRay(transform.position, direction * collisionCheckDistance, Color.red, 0.1f);

        // Check if we hit something with a solid tag
        if (hit.collider != null && !hit.collider.isTrigger && hit.collider.gameObject != gameObject)
        {
            foreach (string tag in solidObjectTags)
            {
                if (hit.collider.CompareTag(tag))
                {
                    return true; // Found a solid object
                }
            }
        }

        return false;
    }

    bool CheckOverlapForSolidTags(Vector2 direction)
    {
        // Calculate the position to check for collisions
        Vector2 checkPosition = (Vector2)transform.position + direction * (collisionCheckDistance * 0.5f);

        // Check for any colliders at that position
        Collider2D[] hits = Physics2D.OverlapCircleAll(checkPosition, collisionCheckDistance * 0.3f);

        // Visualize the check area
        Debug.DrawLine(transform.position, checkPosition, Color.blue, 0.1f);

        foreach (Collider2D hit in hits)
        {
            if (hit != null && !hit.isTrigger && hit.gameObject != gameObject)
            {
                foreach (string tag in solidObjectTags)
                {
                    if (hit.CompareTag(tag))
                    {
                        return true; // Found a solid object
                    }
                }
            }
        }

        return false;
    }

    IEnumerator Move(Vector2 direction)
    {
        isMoving = true;
        lastDirection = direction;
        UpdateAnimator(direction, direction);

        Vector3 startPosition = transform.position;
        targetPosition = startPosition + new Vector3(direction.x * gridSize, direction.y * gridSize, 0);

        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;

            // Use transform.position for movement
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null;
        }

        // Ensure exact position (snap to grid)
        transform.position = new Vector3(
            Mathf.Round(transform.position.x / gridSize) * gridSize,
            Mathf.Round(transform.position.y / gridSize) * gridSize,
            transform.position.z
        );

        isMoving = false;
    }

    void UpdateAnimator(Vector2 movement, Vector2 direction)
    {
        if (animator != null)
        {
            animator.SetBool(IsMoving, movement != Vector2.zero);

            if (direction != Vector2.zero)
            {
                animator.SetFloat(MoveX, direction.x);
                animator.SetFloat(MoveY, direction.y);
            }
        }
    }

    // Alternative: Physics-based collision without layers
    void OnTriggerEnter2D(Collider2D other)
    {
        // If we're moving and hit a solid object, stop movement
        if (isMoving && !other.isTrigger)
        {
            foreach (string tag in solidObjectTags)
            {
                if (other.CompareTag(tag))
                {
                    StopAllCoroutines();
                    isMoving = false;
                    SnapToGrid();
                    UpdateAnimator(Vector2.zero, lastDirection);
                    break;
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Backup collision detection
        if (isMoving && !collision.collider.isTrigger)
        {
            foreach (string tag in solidObjectTags)
            {
                if (collision.collider.CompareTag(tag))
                {
                    StopAllCoroutines();
                    isMoving = false;
                    SnapToGrid();
                    UpdateAnimator(Vector2.zero, lastDirection);
                    break;
                }
            }
        }
    }

    void SnapToGrid()
    {
        transform.position = new Vector3(
            Mathf.Round(transform.position.x / gridSize) * gridSize,
            Mathf.Round(transform.position.y / gridSize) * gridSize,
            transform.position.z
        );
    }

    // Visual debugging
    void OnDrawGizmos()
    {
        // Draw movement target
        if (Application.isPlaying && isMoving)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(targetPosition, Vector3.one * 0.3f);
            Gizmos.DrawLine(transform.position, targetPosition);
        }

        // Draw collision check area
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, collisionCheckDistance * 0.3f);
    }

    public Vector2 GetFacingDirection()
    {
        return lastDirection;
    }

    public bool IsCharacterMoving()
    {
        return isMoving;
    }
}
