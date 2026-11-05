using System.Collections;
using UnityEngine;
using TMPro;

// Fight -> enemy dies -> shop shows -> Next Round -> 3 2 1 -> fight again (forever for now)
public class RoundManager : MonoBehaviour
{
    // True only while a fight is really happening (not during the 3-2-1 countdown or the shop).
    // The enemy's punch script checks this so it doesn't attack at the wrong time.
    public static bool FightIsOn { get; private set; } = true;

    public EnemyHealth enemy;
    public GameObject shopScreen;     // the shop canvas (Shop button, money, menu, Next Round)
    public onbuttondo shopMenu;       // opens/closes the shop menu
    public GameObject[] hideDuringShop; // e.g. the health bars
    public int round = 1;

    [Header("Next round")]
    public Transform player;
    public Behaviour[] playerControls;  // turned off during the countdown (movement, punching)
    public TMP_Text countdownText;

    [Header("Rewards")]
    public ShopManager shop;
    public int goldPerWin = 50;
    public PlayerHealth playerHealth;

    private Vector3 playerStartPosition;
    private Coroutine countdown;

    void Awake()
    {
        if (enemy != null)
            enemy.Died += OnEnemyDied;
    }

    void Start()
    {
        if (player != null)
            playerStartPosition = player.position;

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        if (shopScreen != null)
            shopScreen.SetActive(false);

        // First round gets a countdown too
        countdown = StartCoroutine(Countdown());
    }

    void OnEnemyDied()
    {
        SetPlayerControls(false); // no walking/punching while shopping
        if (shop != null)
            shop.AddGold(goldPerWin);
        SetFightUI(false);
        if (shopScreen != null)
            shopScreen.SetActive(true);
    }

    void SetFightUI(bool visible)
    {
        if (hideDuringShop == null)
            return;
        foreach (var go in hideDuringShop)
            if (go != null)
                go.SetActive(visible);
    }

    // Hooked to the Next Round button
    public void NextRound()
    {
        if (countdown != null)
            return; // already counting down

        if (shopMenu != null)
            shopMenu.CloseMenu();

        if (shopScreen != null)
            shopScreen.SetActive(false);
        SetFightUI(true);

        round++;
        Debug.Log("Round " + round);

        if (enemy != null)
            enemy.ResetHealth();

        if (playerHealth != null)
            playerHealth.ResetHealth();

        ResetPlayer();
        countdown = StartCoroutine(Countdown());
    }

    void ResetPlayer()
    {
        if (player == null)
            return;

        player.position = playerStartPosition;
        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.position = playerStartPosition;
            rb.linearVelocity = Vector2.zero;
        }
    }

    IEnumerator Countdown()
    {
        SetPlayerControls(false);

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
            for (int i = 3; i >= 1; i--)
            {
                countdownText.text = i.ToString();
                yield return new WaitForSeconds(1f);
            }
            countdownText.text = "FIGHT!";
        }

        SetPlayerControls(true);
        yield return new WaitForSeconds(0.7f);

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);
        countdown = null;
    }

    void SetPlayerControls(bool on)
    {
        FightIsOn = on;   // lets the enemy know whether it may attack

        if (playerControls == null)
            return;
        foreach (var b in playerControls)
            if (b != null)
                b.enabled = on;
    }
}
