using System.Collections.Generic;
using UnityEngine;

public class PunchHitbox : MonoBehaviour
{
    private bool landed;   // one hit per punch

    // Called by BoxerAttack the moment the fist is out. Checks for the enemy right away,
    // so the punch still lands when the game runs slowly (the hitbox may be on for less than a physics step).
    public void StartPunch()
    {
        landed = false;
        Collider2D box = GetComponent<Collider2D>();
        if (box == null)
            return;

        Physics2D.SyncTransforms();
        var hits = new List<Collider2D>();
        box.Overlap(new ContactFilter2D { useTriggers = true }, hits);
        foreach (Collider2D other in hits)
            Hit(other);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Hit(other);
    }

    void Hit(Collider2D other)
    {
        if (landed || !other.CompareTag("Enemy"))
            return;

        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy == null)
            return;

        landed = true;

        // Story rule: steroids can't beat the final boss (no shortcuts in life)
        EnemyBoxer boxer = other.GetComponent<EnemyBoxer>();
        if (boxer != null && boxer.ShortcutBlocked())
            return;

        // Punch strength comes from the Powerups object (knuckles, creatine, steroids).
        // If it's missing, fall back to the old plain 10.
        int damage = PlayerPowerups.Instance != null
            ? PlayerPowerups.Instance.Damage
            : 10;

        // Enemy has its guard up: most of the punch is blocked
        if (boxer != null && boxer.IsGuarding)
            damage = Mathf.Max(1, Mathf.RoundToInt(damage * EnemyBoxer.GuardDamageMultiplier));

        Debug.Log("Punch hit: " + other.name + " for " + damage);
        enemy.TakeDamage(damage);
    }
}
