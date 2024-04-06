using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Get Stunned", menuName = "Behaviour/Get Stunned", order = 50)]

class GetStunnedBehaviour : BehaviourSO
{
    [SerializeField]
    private List<CardEffect> _triggers;

    public List<CardEffect> triggers => _triggers;

    public override void CheckTrigger(CardEffect effect, ICardAffectable controller)
    {
        EnemyController enemyController = new EnemyController();

        if (controller is EnemyController _enemyController)
        {
            enemyController = _enemyController;
        }


        if (IsTriggered(effect))
        {
            ApplyDamageEffect(effect, enemyController);
        }
    }

    private bool IsTriggered(CardEffect effect)
    {
        return triggers.Exists(x => x.GetType() == effect.GetType());
    }

    private void ApplyDamageEffect(CardEffect effect, EnemyController controller)
    {
        var stunEffect = effect as StunEffect;
        if (stunEffect != null)
        {
            controller.StunnedEvent?.Invoke();
        }
    }
}
