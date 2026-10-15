using UnityEngine;

public class BoxerAttack : MonoBehaviour
{
    private Animator animator;
    private bool isPunching = false;

    public Collider2D punchHitbox;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (punchHitbox != null)
            punchHitbox.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isPunching)
        {
            isPunching = true;
            animator.SetTrigger("Punch");
        }
    }

    public void EnablePunchHitbox()
    {
        if (punchHitbox != null)
            punchHitbox.enabled = true;
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

