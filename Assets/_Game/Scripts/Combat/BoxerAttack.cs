using UnityEngine;

public class BoxerAttack : MonoBehaviour
{
    private Animator animator;
    private bool isPunching = false;

    // Hold the right mouse button to block: hits hurt much less
    public bool IsBlocking { get; private set; }

    public Collider2D punchHitbox;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (punchHitbox != null)
            punchHitbox.enabled = false;
    }

    void Update()
    {
        IsBlocking = Input.GetMouseButton(1) && !isPunching;
        animator.SetBool("Block", IsBlocking);

        if (Input.GetMouseButtonDown(0) && !isPunching && !IsBlocking)
        {
            isPunching = true;
            animator.SetTrigger("Punch");
        }
    }

    // Controls get turned off during the countdown/shop: drop the guard
    void OnDisable()
    {
        IsBlocking = false;
        if (animator != null)
            animator.SetBool("Block", false);
    }

    public void EnablePunchHitbox()
    {
        if (punchHitbox != null)
        {
            punchHitbox.enabled = true;
            PunchHitbox fist = punchHitbox.GetComponent<PunchHitbox>();
            if (fist != null)
                fist.StartPunch();
        }
    }

    public void DisablePunchHitbox()
    {
        if (punchHitbox != null)
            punchHitbox.enabled = false;
    }

    public void PunchFinished()
    {
        isPunching = false;
    }
}

