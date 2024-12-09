using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Take Damage", menuName = "Behaviour/Enemy/Take Damage", order = 50)]
public class TakeDamageBehaviour : BehaviourSO
{
    [SerializeField]
    private List<CardEffect> _triggers;

    public List<CardEffect> triggers => _triggers;

    public DodgeBehaviour dodgeBehaviour;

    public override void CheckTrigger(CardEffect effect, ICardAffectable controller)
    {
        if (!IsTriggered(effect)) return;

        EnemyController enemyController = new EnemyController();

        if (controller is EnemyController _enemyController)
        {
            enemyController = _enemyController;
        }

        if (dodgeBehaviour.CheckCanDodge())
        {
            enemyController.AttackDodgedEvent?.Invoke();
            return;
        }

        ApplyDamageEffect(effect, enemyController);
      
    }



    private bool IsTriggered(CardEffect effect)
    {
        return triggers.Exists(x => x.GetType() == effect.GetType());
    }

    private void ApplyDamageEffect(CardEffect effect, EnemyController controller)
    {
        var damageEffect = effect as DamageEffect;
        if (damageEffect != null)
        {
            
            controller.DamageTakenEvent?.Invoke(damageEffect.damageAmount);
        }
    }
}
