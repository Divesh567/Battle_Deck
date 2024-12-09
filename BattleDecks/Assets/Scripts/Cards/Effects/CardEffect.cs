using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Base Effect", menuName = "Cards/Card Effect/Base", order = 2)]

public class CardEffect : ScriptableObject, ICardEffect
{
    public bool UseOnSelf;
    public virtual void ApplyEffectToTarget(ICardAffectable cardAffectable)
    {
        //Apply Base Effect to enemy, if any
    }
}
