using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stamina Drain Effect", menuName = "Cards/Card Effect/Stamina", order = 2)]
public class StaminaConsumtion : CardEffect
{
    [SerializeField]
    private int _staminaConsumtion = 10;

    public int staminaConsumtion { get { return _staminaConsumtion; } }

    public override void ApplyEffectToTarget(ICardAffectable cardAffectable)
    {
        cardAffectable.ApplyEffectToSelf(this);
    }


}
