using UnityEngine;

/// <summary>
/// Scales the background sprite so it always fills the camera view without stretching,
/// whatever the window size. The bottom (ring floor) stays at the bottom of the screen.
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class FitBackgroundToCamera : MonoBehaviour
{
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
        float bottom = c.y - viewH / 2f;
        transform.position = new Vector3(c.x, bottom + size.y * k / 2f, 0f) - sprite.bounds.center * k;
    }
}
