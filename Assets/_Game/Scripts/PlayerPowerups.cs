using UnityEngine;

/// <summary>
/// Remembers which shop items the player owns and turns them into power.
/// Lives on one empty GameObject in the scene. Resets every time the game starts.
/// </summary>
public class PlayerPowerups : MonoBehaviour
{
    public static PlayerPowerups Instance { get; private set; }

    [Header("Punch damage")]
    public int baseDamage = 10;
    public int brassKnucklesBonus = 8;      // 10 -> 18
    public int creatineBonus = 3;           // small extra
    public float steroidsMultiplier = 3f;   // super strength: triple damage

    [Header("Helmet")]
    [Range(0f, 0.9f)]
    public float helmetDamageReduction = 0.3f;   // take 30% less damage

    [Header("Owned items (you can watch these tick while playing)")]
    public bool hasBrassKnuckles;
    public bool hasCreatine;
    public bool hasSteroids;
    public bool hasHelmet;

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // How hard the player's punch hits right now
    public int Damage
    {
        get
        {
            float damage = baseDamage;
            if (hasBrassKnuckles) damage += brassKnucklesBonus;
            if (hasCreatine) damage += creatineBonus;
            if (hasSteroids) damage *= steroidsMultiplier;
            return Mathf.RoundToInt(damage);
        }
    }

    // Used when something hits the player: the helmet softens the blow
    public int ReduceIncomingDamage(int damage)
    {
        if (!hasHelmet) return damage;
        int reduced = Mathf.RoundToInt(damage * (1f - helmetDamageReduction));
        return Mathf.Max(reduced, 1);   // a hit always does at least 1
    }

    // The shop calls this after a purchase goes through
    public void Give(string itemID)
    {
        switch (itemID)
        {
            case "brass_knuckles": hasBrassKnuckles = true; break;
            case "creatine":       hasCreatine = true;      break;
            case "steroids":       hasSteroids = true;      break;
            case "boxing_helmet":  hasHelmet = true;        break;
        }
    }
}
