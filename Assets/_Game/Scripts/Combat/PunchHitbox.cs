using UnityEngine;

public class PunchHitbox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();

            if (enemy != null)
            {
                // Punch strength comes from the Powerups object (knuckles, creatine, steroids).
                // If it's missing, fall back to the old plain 10.
                int damage = PlayerPowerups.Instance != null
                    ? PlayerPowerups.Instance.Damage
                    : 10;

                Debug.Log("Punch hit: " + other.name + " for " + damage);
                enemy.TakeDamage(damage);
            }
        }
    }
}
