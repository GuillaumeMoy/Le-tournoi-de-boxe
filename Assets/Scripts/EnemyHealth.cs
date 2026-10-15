using UnityEngine;
using TMPro;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int health = 100;
    public TMP_Text healthText;

    public SpriteRenderer spriteRenderer;
    public Color hitColor = new Color(1f, 0.5f, 0.5f);
    public float flashDuration = 0.12f;

    private Color originalColor;
    private Coroutine flashCoroutine;

    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        UpdateHealthText();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        health = Mathf.Max(health, 0);

        UpdateHealthText();
        FlashRed();

        if (health <= 0)
            Destroy(gameObject);
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
    }
}