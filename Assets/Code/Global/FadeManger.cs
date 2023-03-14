using System.Collections;
using UnityEngine;

namespace Global
{
    public class FadeManger : MonoBehaviour
    {
        [SerializeField]
        private Animator animator;
        public IEnumerator FadeRoutine(bool fadeStatus)
        {
            if (!animator)
                yield break;

            animator.SetBool("FadeStatus", fadeStatus);

            // Wait till proper AnimationState is loaded
            if (fadeStatus)
                yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("FadeIn"));
            else
                yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("FadeOut"));

            // Wait till the end of the animation
            yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);
        }

    }
}
