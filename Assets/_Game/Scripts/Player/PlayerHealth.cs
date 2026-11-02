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
        health = Mathf.Max(health - damage, 0);
        UpdateBar();

        if (health <= 0)
            Debug.Log("Player knocked out!");
    }

    void UpdateBar()
    {
        if (healthBar != null)
            healthBar.SetHealth(health, maxHealth);
    }
}
