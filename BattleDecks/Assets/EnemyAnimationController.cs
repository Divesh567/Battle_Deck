using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{

    public const string IdleAnim = "Idle";
    public const string ReadyAnim = "GetReady";
    public const string AttackAnim = "Attack";
    public const string GetHitAnim = "GetHit";
    public const string DieAnim = "Die";


    [SerializeField]
    private Animator animator;

    // Trigger Get Hit Animation
    public void PlayGetHit()
    {
        animator.SetTrigger(GetHitAnim);
    }

    // Trigger Attack Animation
    public void PlayAttack()
    {
        animator.SetTrigger(AttackAnim);
    }

    public void PlayDeath()
    {
        animator.SetTrigger(DieAnim);
    }

    // Set Ready State
    public void SetReady()
    {
        animator.SetTrigger(ReadyAnim);
    }

    public void SetIdle()
    {
        animator.SetTrigger(IdleAnim);
    }

   
}
