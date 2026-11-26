using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// When the player is knocked out: stop the controls, play a little "falling over"
/// animation, then freeze the game. Put this on the Player object.
/// After that, the game can be restarted with the Restart() method (a button) or the R key.
/// </summary>
public class PlayerDeath : MonoBehaviour
{
    [Header("Who dies (found automatically if left empty)")]
    public PlayerHealth playerHealth;

    [Header("What falls over (found automatically if left empty: the BOXER child)")]
    public Transform visual;

    [Header("Fall animation")]
    [Tooltip("How far the character tips over. If it falls the wrong way, make this negative (-90).")]
    public float fallAngle = 90f;
    [Tooltip("How long the fall takes, in seconds")]
    public float fallTime = 0.6f;
    [Tooltip("How far the character sinks down while falling, so it lies on the floor")]
    public float sinkDistance = 0.15f;

    [Header("After the fall")]
    [Tooltip("Wait this long after the fall before freezing the game")]
    public float pauseDelay = 0.5f;
    [Tooltip("The 'KNOCKED OUT' screen to show")]
    public GameObject loseScreen;
    [Tooltip("Pressing this key after being knocked out also restarts the game")]
    public KeyCode restartKey = KeyCode.R;

    private bool dying;
    private bool frozen;

    void Awake()
    {
        if (playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();

        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (visual == null)
        {
            Transform boxer = transform.Find("BOXER");
            visual = boxer != null ? boxer : transform;
        }
    }

    void OnEnable()
    {
        if (playerHealth != null)
            playerHealth.Died += OnPlayerDied;
    }

    void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.Died -= OnPlayerDied;
    }

    void Update()
    {
        // Works even while the game is frozen
        if (frozen && Input.GetKeyDown(restartKey))
            Restart();
    }

    void OnPlayerDied()
    {
        if (dying)
            return;

        dying = true;

        // 1. Stop walking, jumping and punching
        PlayerPlatform movement = GetComponent<PlayerPlatform>();
        if (movement != null)
            movement.enabled = false;

        foreach (BoxerAttack attack in GetComponentsInChildren<BoxerAttack>())
        {
            attack.DisablePunchHitbox();   // no punch can land after being knocked out
            attack.enabled = false;
        }

        // 2. Stop the walking/punching animations so they don't fight the fall
        foreach (Animator animator in GetComponentsInChildren<Animator>())
            animator.enabled = false;

        // 3. Fall over, then freeze the game
        StartCoroutine(FallAndFreeze());
    }

    IEnumerator FallAndFreeze()
    {
        Quaternion startRotation = visual.localRotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0f, 0f, fallAngle);
        Vector3 startPosition = visual.localPosition;
        Vector3 endPosition = startPosition + Vector3.down * sinkDistance;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / Mathf.Max(fallTime, 0.01f);
            float eased = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t));

            visual.localRotation = Quaternion.Slerp(startRotation, endRotation, eased);
            visual.localPosition = Vector3.Lerp(startPosition, endPosition, eased);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(pauseDelay);

        // Prefer the game over panel (Try Again / Main Menu buttons); fall back to the old lose screen
        GameOverUI gameOver = FindFirstObjectByType<GameOverUI>();
        if (gameOver != null)
            gameOver.ShowGameOver();
        else if (loseScreen != null)
            loseScreen.SetActive(true);

        // Stops the whole game (enemy, timers, everything)
        Time.timeScale = 0f;
        frozen = true;
    }

    // Hook this to the "TRY AGAIN" button. Starts the whole game over from the beginning
    // (gold, items and health all reset).
    public void Restart()
    {
        Time.timeScale = 1f;   // un-freeze before loading
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
