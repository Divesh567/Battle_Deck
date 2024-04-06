using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Damage Effect", menuName = "Cards/Card Effect/Damage", order = 2)]
public class DamageEffect : CardEffect
{
    [SerializeField]
    private int _damageAmount = 10;

    public int damageAmount { get { return _damageAmount; } }
    public override void ApplyEffectToTarget(ICardAffectable target)
    {
        target.ApplyCardToSelf(this);
    }

}
