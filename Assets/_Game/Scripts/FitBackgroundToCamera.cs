using UnityEngine;

/// <summary>
/// Scales the background sprite so it always fills the camera view without stretching,
/// whatever the window size. The bottom (ring floor) stays at the bottom of the screen.
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class FitBackgroundToCamera : MonoBehaviour
{
    [Tooltip("How far to push the picture down (world units). Stops before a gap would show at the top.")]
    public float moveDown = 1.5f;

    void LateUpdate()
    {
        Camera cam = Camera.main;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        if (cam == null || sprite == null) return;

        float viewH = cam.orthographicSize * 2f;
        float viewW = viewH * cam.aspect;
        Vector3 size = sprite.bounds.size;
        float k = Mathf.Max(viewW / size.x, viewH / size.y);   // same scale both ways = no stretch

        transform.localScale = new Vector3(k, k, 1f);
        Vector3 c = cam.transform.position;
        float extraHeight = size.y * k - viewH;                     // picture height hidden above the screen
        float down = Mathf.Clamp(moveDown, 0f, Mathf.Max(extraHeight, 0f));
        float bottom = c.y - viewH / 2f - down;
        transform.position = new Vector3(c.x, bottom + size.y * k / 2f, 0f) - sprite.bounds.center * k;
    }
}
