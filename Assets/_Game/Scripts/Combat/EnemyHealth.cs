using UnityEngine;
using TMPro;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int health = 100;
    public TMP_Text healthText;
    public HealthBarUI healthBar;
    private int maxHealth;

    public SpriteRenderer spriteRenderer;
    public Color hitColor = new Color(1f, 0.5f, 0.5f);
    public float flashDuration = 0.12f;

    private Color originalColor;
    private Coroutine flashCoroutine;

    // Awake (not Start) so the real colour is known even if ResetHealth runs before the first frame;
    // otherwise originalColor stays (0,0,0,0) and the enemy turns invisible.
    void Awake()
    {
        maxHealth = health;

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    void Start()
    {
        UpdateHealthText();
    }

    // RoundManager listens to this to open the shop
    public System.Action Died;

    public void TakeDamage(int damage)
    {
        if (health <= 0)
            return;

        health -= damage;
        health = Mathf.Max(health, 0);

        UpdateHealthText();
        FlashRed();

        if (health <= 0)
        {
            // Hide instead of Destroy so it can come back next round
            gameObject.SetActive(false);
            Died?.Invoke();
        }
    }

    // Brings the enemy back with full health for the next round
    public void ResetHealth()
    {
        health = maxHealth;
        flashCoroutine = null;
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor; // flash may have been cut off when it died

        gameObject.SetActive(true);
        UpdateHealthText();
    }

    void FlashRed()
    {
        if (spriteRenderer == null)
            return;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(RedFlash());
    }

    IEnumerator RedFlash()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(flashDuration);

        spriteRenderer.color = originalColor;
        flashCoroutine = null;
    }

    void UpdateHealthText()
    {
        if (healthText != null)
            healthText.text = "Health: " + health;

        if (healthBar != null)
            healthBar.SetHealth(health, maxHealth);
    }
}