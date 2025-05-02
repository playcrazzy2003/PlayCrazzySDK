using System;
using UnityEngine;
using DG.Tweening;

public enum AnimType
{
    Idle,
    Move,
}
public enum AnimLayerState
{
    Base,
    UpperBody
}
[RequireComponent(typeof(Animator))]
public class AnimationController : MonoBehaviour
{
    [Header("Animator Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private AnimType startAnimation = AnimType.Idle;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (animator != null)
        {
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        }
    }

    private void Start()
    {
        PlayAnimation(startAnimation, 0f); // Optional: auto-play on start
    }

    /// <summary>
    /// Plays a crossfade animation with a specified transition time.
    /// </summary>
    /// <param name="animation">The animation to play.</param>
    /// <param name="transitionTime">The duration of the transition.</param>
    public void PlayAnimation(AnimType animation, float transitionTime = 0.1f)
    {
        if (animator == null) return;

        animator.Play(animation.ToString());
        startAnimation = animation;
    }

    public void SetFloat(string parameter, float value)
    {
        animator.SetFloat(parameter, value);
    }
    /// <summary>
    /// Returns the name of the current animation state in layer 0.
    /// </summary>
    /// <returns>Animation state name or error description.</returns>
    public string GetCurrentAnimationState()
    {
        if (animator == null) return "Animator Not Found";

        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);

        foreach (AnimType anim in Enum.GetValues(typeof(AnimType)))
        {
            if (Animator.StringToHash(anim.ToString()) == currentState.shortNameHash)
            {
                return anim.ToString();
            }
        }

        return "Unknown State";
    }

    /// <summary>
    /// Smoothly adjusts the weight of a specific animation layer.
    /// </summary>
    /// <param name="layer">The animation layer to modify.</param>
    /// <param name="targetWeight">Target weight (0 to 1).</param>
    /// <param name="duration">Duration of the transition.</param>
    public void SetLayerWeight(AnimLayerState layer, float targetWeight, float duration)
    {
        if (animator == null) return;

        float currentWeight = animator.GetLayerWeight((int)layer);

        DOTween.To(() => currentWeight, x =>
        {
            animator.SetLayerWeight((int)layer, x);
        }, targetWeight, duration)
        .SetEase(Ease.Linear);
    }
}
