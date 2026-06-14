using UnityEngine;

namespace BattleDecks.Data
{
    public enum TriggerCondition
    {
        OnTurnStart,
        OnLowHealth,
        OnEnemyHasStatus,
    }

    [CreateAssetMenu(menuName = "BattleDecks/Trigger Data", fileName = "NewTrigger")]
    public class TriggerData : ScriptableObject
    {
        [Header("Condition")]
        public TriggerCondition condition;
        [Range(0f, 1f)] public float hpThreshold;   // used by OnLowHealth (e.g. 0.25 = 25%)
        public string statusToCheck;                  // used by OnEnemyHasStatus

        [Header("Effects")]
        public CardEffectData[] effects;              // resolved on self when triggered
        public CardData cardToUnlock;                 // added to hand when triggered

        [Header("Behaviour")]
        public bool oneShot;                          // fires only once per combat
    }
}
