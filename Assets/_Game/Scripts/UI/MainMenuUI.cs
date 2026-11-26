using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Main menu: a loading intro (bar fills while the fight scene really loads in the background),
/// then the title screen with PLAY / QUIT. PLAY switches to the already-loaded fight.
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Header("Scene to play")]
    public string gameScene = "SampleScene";
    [Tooltip("The loading intro lasts at least this long, even if loading is faster")]
    public float minLoadingTime = 2.5f;

    [Header("Loading intro")]
    public CanvasGroup loadingGroup;
    public Image barFill;              // Image Type = Filled, Horizontal
    public TMP_Text loadingText;

    [Header("Title screen")]
    public CanvasGroup menuGroup;
    public RectTransform title;
    public TMP_Text pressText;         // blinking hint
    public Button playButton;
    public Button quitButton;

    private AsyncOperation load;

    void Start()
    {
        Time.timeScale = 1f;   // coming back from a paused game over screen

        if (playButton != null) playButton.onClick.AddListener(Play);
        if (quitButton != null) quitButton.onClick.AddListener(Quit);

        StartCoroutine(Intro());
    }

    IEnumerator Intro()
    {
        Show(menuGroup, 0f);
        Show(loadingGroup, 1f);

        // Start loading the fight now, but don't switch to it until PLAY is pressed
        load = SceneManager.LoadSceneAsync(gameScene);
        if (load != null)
            load.allowSceneActivation = false;

        string[] dots = { "CHARGEMENT", "CHARGEMENT.", "CHARGEMENT..", "CHARGEMENT..." };
        float t = 0f;
        while (true)
        {
            t += Time.deltaTime;
            float real = load != null ? Mathf.Clamp01(load.progress / 0.9f) : 1f;   // Unity stops at 0.9 until activation
            float shown = Mathf.Min(real, t / minLoadingTime);
            if (barFill != null) barFill.fillAmount = shown;
            if (loadingText != null) loadingText.text = dots[(int)(t * 4f) % 4] + "  " + Mathf.RoundToInt(shown * 100f) + "%";
            if (shown >= 1f)
                break;
            yield return null;
        }
        yield return new WaitForSeconds(0.25f);

        // Fade the loading screen out, title screen in
        for (float f = 0f; f < 1f; f += Time.deltaTime / 0.5f)
        {
            Show(loadingGroup, 1f - f);
            Show(menuGroup, f);
            yield return null;
        }
        Show(loadingGroup, 0f);
        Show(menuGroup, 1f);
        if (loadingGroup != null) loadingGroup.blocksRaycasts = false;

        // Title slams in
        if (title != null)
        {
            for (float f = 0f; f < 1f; f += Time.deltaTime / 0.35f)
            {
                title.localScale = Vector3.one * Mathf.LerpUnclamped(2f, 1f, EaseOutBack(f));
                yield return null;
            }
            title.localScale = Vector3.one;
        }
    }

    void Update()
    {
        // Blinking hint, retro arcade style
        if (pressText != null)
            pressText.alpha = Mathf.PingPong(Time.time * 1.5f, 1f) > 0.35f ? 1f : 0.15f;
    }

    public void Play()
    {
        if (playButton != null) playButton.interactable = false;
        if (load != null)
            load.allowSceneActivation = true;     // the fight is already loaded: switch now
        else
            SceneManager.LoadScene(gameScene);
    }

    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    static void Show(CanvasGroup g, float alpha)
    {
        if (g == null) return;
        g.alpha = alpha;
        g.interactable = alpha > 0.99f;
        g.blocksRaycasts = alpha > 0.01f;
    }

    static float EaseOutBack(float k)
    {
        const float c1 = 1.70158f, c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(k - 1f, 3f) + c1 * Mathf.Pow(k - 1f, 2f);
    }
}
