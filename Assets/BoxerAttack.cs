using UnityEngine;

public class BoxerAttack : MonoBehaviour
{
    private Animator animator;
    private bool isPunching = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isPunching)
        {
            isPunching = true;
            animator.SetTrigger("Punch");
        }
    }

    // Call this when the punch animation finishes
    public void PunchFinished()
    {
        isPunching = false;
    }
}