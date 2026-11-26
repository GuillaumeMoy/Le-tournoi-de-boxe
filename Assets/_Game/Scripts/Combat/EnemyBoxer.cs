using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Who the player is fighting this round: the 5 opponents (name, sprites, strength).
/// Also turns the enemy to always face the player and picks its pose (normal / punch / block).
/// Put this on the enemy next to EnemyHealth, EnemyAttack and EnemyMovement.
/// The RoundManager calls Setup(round) at the start of every round.
/// </summary>
public class EnemyBoxer : MonoBehaviour
{
    [System.Serializable]
    public class Opponent
    {
        public string name = "Boxer";
        public string barName = "BOXER";   // short name written on the health bar
        [Header("Sprites (all drawn facing RIGHT)")]
        public Sprite normal;
        public Sprite block;
        public Sprite punch1;   // arm fully out
        public Sprite punch2;   // arm half out
        [Header("Strength")]
        public int health = 100;
        public int damage = 10;
        public float timeBetweenPunches = 2f;
        public float approachSpeed = 3f;
        [Tooltip("Story rule: a player who bought steroids can never hurt this boxer")]
        public bool steroidsCantBeatMe;
    }

    public Opponent[] opponents;

    [Header("Found automatically if left empty")]
    public Transform player;
    public SpriteRenderer spriteRenderer;

    [Tooltip("The name on the enemy's health bar")]
    public TMP_Text nameText;

    [Header("Story message (no shortcuts in life)")]
    public TMP_Text messageText;
    [TextArea]
    public string shortcutMessage = "YOUR PUNCHES DO NOTHING!\nSteroids are a shortcut... and there are no shortcuts in life.";
    [TextArea]
    public string shortcutLoseMessage = "THERE ARE NO SHORTCUTS IN LIFE.\nHARD WORK BEATS DRUGS.";

    public Opponent Current { get; private set; }

    private EnemyHealth health;
    private EnemyAttack attack;
    private EnemyMovement movement;
    private float punchPoseTimer;
    private float blockPoseTimer;
    private Coroutine messageRoutine;

    void Awake()
    {
        health = GetComponent<EnemyHealth>();
        attack = GetComponent<EnemyAttack>();
        movement = GetComponent<EnemyMovement>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (player == null)
        {
            PlayerHealth p = FindFirstObjectByType<PlayerHealth>();
            if (p != null)
                player = p.transform;
        }

        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        if (attack != null)
            attack.Punched += OnPunched;
    }

    void OnDisable()
    {
        if (attack != null)
            attack.Punched -= OnPunched;
    }

    // round 1 = index 0
    public void Setup(int index)
    {
        if (opponents == null || opponents.Length == 0)
            return;

        Current = opponents[Mathf.Clamp(index, 0, opponents.Length - 1)];
        punchPoseTimer = 0f;
        blockPoseTimer = 0f;

        if (spriteRenderer != null)
            spriteRenderer.sprite = Current.normal;

        if (nameText != null)
            nameText.text = Current.barName;

        if (health != null)
            health.SetMaxHealth(Current.health);

        if (attack != null)
        {
            attack.damage = Current.damage;
            attack.timeBetweenPunches = Current.timeBetweenPunches;
        }

        if (movement != null)
            movement.approachSpeed = Current.approachSpeed;

        // A fresh round: the steroid lesson only shows on the game over screen if it happened this fight
        GameOverUI.NextSubtitle = null;
    }

    void OnPunched()
    {
        punchPoseTimer = 0.3f;
    }

    // Called by the player's punch. True = the punch does nothing (steroids vs the final boss).
    public bool ShortcutBlocked()
    {
        bool blocked = Current != null && Current.steroidsCantBeatMe
            && PlayerPowerups.Instance != null && PlayerPowerups.Instance.hasSteroids;

        if (blocked)
        {
            blockPoseTimer = 0.35f;
            GameOverUI.NextSubtitle = shortcutLoseMessage;
            ShowMessage(shortcutMessage);
        }
        return blocked;
    }

    void ShowMessage(string text)
    {
        if (messageText == null)
            return;
        if (messageRoutine != null)
            StopCoroutine(messageRoutine);
        messageRoutine = StartCoroutine(MessageRoutine(text));
    }

    IEnumerator MessageRoutine(string text)
    {
        messageText.text = text;
        messageText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        messageText.gameObject.SetActive(false);
        messageRoutine = null;
    }

    void LateUpdate()
    {
        if (spriteRenderer == null || Current == null)
            return;

        // Always face the player (sprites are drawn facing right)
        if (player != null)
            spriteRenderer.flipX = player.position.x < transform.position.x;

        punchPoseTimer -= Time.deltaTime;
        blockPoseTimer -= Time.deltaTime;

        Sprite pose = Current.normal;
        if (blockPoseTimer > 0f && Current.block != null)
            pose = Current.block;
        else if (attack != null && attack.IsWindingUp && Current.punch2 != null)
            pose = Current.punch2;              // pulling the arm back
        else if (punchPoseTimer > 0f && Current.punch1 != null)
            pose = Current.punch1;              // the punch lands

        spriteRenderer.sprite = pose;
    }
}
