using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Behaviour List", menuName = "Behaviour/New Behaviour List", order = 0)]
public class Behaviour: ScriptableObject
{
    public List<BehaviourSO> behaviours;


    public void CheckBehaviours(CardEffect effect, ICardAffectable controller)
    {
        behaviours.ForEach(x => x.CheckTrigger(effect, controller));
    }
}