using UnityEngine;


public class BehaviourSO: ScriptableObject
{
    [SerializeField] [Range(0,9)]
    public int Priority;

    public virtual void CheckTrigger(CardEffect effect, ICardAffectable controller) 
    {
    
    }
}