using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int health;
    public HealthBarUI healthBar;

    // Other scripts (like PlayerDeath) listen to this to know the player was knocked out
    public System.Action Died;

    private bool dead;

    void Start()
    {
        health = maxHealth;
        dead = false;
        UpdateBar();
    }

    // Call this from the enemy's punch later
    public void TakeDamage(int damage)
    {
        // Already knocked out: ignore any more hits
        if (dead)
            return;

        // The helmet (from the shop) makes every hit hurt less
        if (PlayerPowerups.Instance != null)
            damage = PlayerPowerups.Instance.ReduceIncomingDamage(damage);

        Debug.Log("Player took " + damage + " damage");

        health = Mathf.Max(health - damage, 0);
        UpdateBar();

        if (health <= 0)
        {
            dead = true;
            Debug.Log("Player knocked out!");
            Died?.Invoke();
        }
    }

    // Test buttons: while the game is running, right-click the "Player Health"
    // title in the Inspector and choose one of these.
    [ContextMenu("Test: take 10 damage")]
    void TestTakeDamage()
    {
        TakeDamage(10);
    }

    [ContextMenu("Test: knock out player")]
    void TestKnockOut()
    {
        TakeDamage(9999);
    }

    public void ResetHealth()
    {
        health = maxHealth;
        dead = false;
        UpdateBar();
    }

    void UpdateBar()
    {
        if (healthBar != null)
            healthBar.SetHealth(health, maxHealth);
    }
}
