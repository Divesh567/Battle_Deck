using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Stamina Consumed", menuName = "Behaviour/Player/Stamina Consumed", order = 50)]
public class StaminaConsumedBehaviour : BehaviourSO
{
    [SerializeField]
    private List<CardEffect> _triggers;

    public List<CardEffect> triggers => _triggers;


    public override void CheckTrigger(CardEffect effect, ICardAffectable controller)
    {
        PlayerController playerController = new PlayerController();

        if (controller is PlayerController _playerController)
        {
            playerController = _playerController;
        }

        if (!IsTriggered(effect)) return;


        ConsumeStamina(effect, playerController);

    }

    private bool IsTriggered(CardEffect effect)
    {
        return triggers.Exists(x => x.GetType() == effect.GetType());
    }

    private void ConsumeStamina(CardEffect effect, PlayerController controller)
    {
        var staminaConsumed = effect as StaminaConsumtion;
        if (staminaConsumed != null)
        {
            controller.StaminaReducedEvent?.Invoke(staminaConsumed.staminaConsumtion);
        }
    }
}
