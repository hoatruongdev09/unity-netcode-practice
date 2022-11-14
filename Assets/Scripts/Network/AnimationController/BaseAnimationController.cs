using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class BaseAnimationController : NetworkBehaviour
{
    public NetworkAnimator NetworkAnimator => networkAnimator;
    public Animator Animator => animator;

    [SerializeField] private NetworkAnimator networkAnimator;
    [SerializeField] private Animator animator;

    public virtual void PlayTriggerAnimation(string animation, float speed = 1)
    {
        if (NetworkAnimator)
        {
            NetworkAnimator.SetTrigger(animation);
            NetworkAnimator.Animator.SetFloat($"{animation}_speed", speed);
        }
    }

    public virtual void PlayBoolAnimation(string animation, float speed = 1)
    {
        if (NetworkAnimator)
        {
            NetworkAnimator.Animator.SetBool(animation, true);
            NetworkAnimator.Animator.SetFloat($"{animation}_speed", speed);
        }
        if (Animator)
        {
            Animator.SetBool(animation, true);
            Animator.SetFloat($"{animation}_speed", speed);
        }

    }
    public virtual void PauseBoolAnimation(string animation, float speed = 1)
    {
        if (NetworkAnimator)
        {
            NetworkAnimator.Animator.SetBool(animation, false);
            NetworkAnimator.Animator.SetFloat($"{animation}_speed", speed);
        }
        if (Animator)
        {
            Animator.SetBool(animation, false);
            Animator.SetFloat($"{animation}_speed", speed);
        }
    }
}