using UnityEngine;

public class PunchHitbox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Punch hit: " + other.name);

            EnemyHealth enemy = other.GetComponent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(10);
            }
        }
    }
}