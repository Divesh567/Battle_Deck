using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Take Damage", menuName = "Behaviour/Player/Take Damage", order = 50)]
public class PlayerTakeDamageBehaviour : BehaviourSO
{
    [SerializeField]
    private List<CardEffect> _triggers;

    public List<CardEffect> triggers => _triggers;

    public DodgeBehaviour dodgeBehaviour;

    public override void CheckTrigger(CardEffect effect, ICardAffectable controller)
    {
        if (!IsTriggered(effect)) return;

        PlayerController playerController = new PlayerController();

        if (controller is PlayerController _playerController)
        {
            playerController = _playerController;
        }

/*        if (dodgeBehaviour.CheckCanDodge())
        {
            playerController.AttackDodgedEvent?.Invoke();
            return;
        }*/


        ApplyDamageEffect(effect, playerController);

    }



    private bool IsTriggered(CardEffect effect)
    {
        return triggers.Exists(x => x.GetType() == effect.GetType());
    }

    private void ApplyDamageEffect(CardEffect effect, PlayerController controller)
    {
        var damageEffect = effect as DamageEffect;
        if (damageEffect != null)
        {
            controller.DamageTakenEvent?.Invoke(damageEffect.damageAmount);
        }
    }
}
