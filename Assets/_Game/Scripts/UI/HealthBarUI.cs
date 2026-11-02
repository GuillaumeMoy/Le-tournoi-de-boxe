using UnityEngine;
using UnityEngine.UI;

// Shows one of the healthbar images depending on how much health is left.
// frames[0] = full, last frame = empty.
public class HealthBarUI : MonoBehaviour
{
    public Image image;
    public Sprite[] frames;

    public void SetHealth(int current, int max)
    {
        if (image == null || frames == null || frames.Length == 0 || max <= 0)
            return;

        int last = frames.Length - 1;
        current = Mathf.Clamp(current, 0, max);

        // Alive uses frames 0..last-1 (rounded up so any damage shows); empty frame only at 0 hp
        int index = current == 0 ? last : ((max - current) * (last - 1) + max - 1) / max;

        image.sprite = frames[index];
    }
}
