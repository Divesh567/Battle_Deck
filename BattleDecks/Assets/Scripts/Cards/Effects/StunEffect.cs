using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stun Effect", menuName = "Cards/Card Effect/Stun", order = 2)]
public class StunEffect : CardEffect
{
    [SerializeField][Range(0,100)]
    private int _chanceToStun = 10;

    public int chanceToStun { get { return _chanceToStun; } }
    public override void ApplyEffectToTarget(ICardAffectable target)
    {
        target.ApplyEffectToSelf(this);
    }

}
