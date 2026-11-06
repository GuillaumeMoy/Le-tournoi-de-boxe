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
        StartCoroutine(BattleWon()); // banner first, then the shop
    }

    // Big "BATTLE WON!" in the middle: slams in, shakes, flashes white, pulses, zooms out. Then opens the shop.
    IEnumerator BattleWon()
    {
        TMP_Text t = countdownText;   // reuse the big centered countdown text
        if (t != null)
        {
            RectTransform rt = t.rectTransform;
            Vector2 home = rt.anchoredPosition;
            Color gold = t.color;

            t.text = "BATTLE WON!";
            t.textWrappingMode = TextWrappingModes.NoWrap;   // one line, even though the box is narrow
            t.outlineWidth = 0.25f;
            t.outlineColor = Color.black;
            t.gameObject.SetActive(true);

            // Slam in: from huge and see-through down to normal size, with a little bounce
            for (float e = 0f; e < 0.35f; e += Time.deltaTime)
            {
                float k = e / 0.35f;
                rt.localScale = Vector3.one * Mathf.LerpUnclamped(4f, 1f, EaseOutBack(k));
                t.alpha = k;
                yield return null;
            }

            // Impact: white flash and screen-shake style wobble that fades out
            for (float e = 0f; e < 0.3f; e += Time.deltaTime)
            {
                float k = 1f - e / 0.3f;
                t.color = Color.Lerp(gold, Color.white, k);
                rt.anchoredPosition = home + Random.insideUnitCircle * 25f * k;
                rt.localRotation = Quaternion.Euler(0f, 0f, Random.Range(-5f, 5f) * k);
                yield return null;
            }
            t.color = gold;
            rt.anchoredPosition = home;

            // Hold: gentle pulse and sway
            for (float e = 0f; e < 1.3f; e += Time.deltaTime)
            {
                rt.localScale = Vector3.one * (1f + 0.06f * Mathf.Sin(e * 10f));
                rt.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(e * 3f) * 3f);
                yield return null;
            }

            // Zoom out and fade
            for (float e = 0f; e < 0.25f; e += Time.deltaTime)
            {
                float k = e / 0.25f;
                rt.localScale = Vector3.one * Mathf.Lerp(1f, 1.6f, k);
                t.alpha = 1f - k;
                yield return null;
            }

            // Put the text back the way the countdown expects it
            t.gameObject.SetActive(false);
            rt.localScale = Vector3.one;
            rt.localRotation = Quaternion.identity;
            t.alpha = 1f;
        }

        SetFightUI(false);
        if (shopScreen != null)
            shopScreen.SetActive(true);
    }

    static float EaseOutBack(float k)
    {
        const float c1 = 1.70158f, c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(k - 1f, 3f) + c1 * Mathf.Pow(k - 1f, 2f);
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
