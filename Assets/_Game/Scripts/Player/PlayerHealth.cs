using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int health;
    public HealthBarUI healthBar;

    void Start()
    {
        health = maxHealth;
        UpdateBar();
    }

    // Call this from the enemy's punch later
    public void TakeDamage(int damage)
    {
        // The helmet (from the shop) makes every hit hurt less
        if (PlayerPowerups.Instance != null)
            damage = PlayerPowerups.Instance.ReduceIncomingDamage(damage);

        Debug.Log("Player took " + damage + " damage");

        health = Mathf.Max(health - damage, 0);
        UpdateBar();

        if (health <= 0)
            Debug.Log("Player knocked out!");
    }

    // Test button: while the game is running, right-click the "Player Health"
    // title in the Inspector and choose this to pretend the player got hit for 10.
    [ContextMenu("Test: take 10 damage")]
    void TestTakeDamage()
    {
        TakeDamage(10);
    }

    public void ResetHealth()
    {
        health = maxHealth;
        UpdateBar();
    }

    void UpdateBar()
    {
        if (healthBar != null)
            healthBar.SetHealth(health, maxHealth);
    }
}
