using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDownManager : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    public IEnumerator CountDownRoutine()
    {
        animator.SetTrigger("countdown");

        // Wait till proper AnimationState is loaded
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Countdown"));

        // Wait till the end of the animation
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Countdown"));
    }
}
