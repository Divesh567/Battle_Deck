using UnityEngine;

namespace BattleDecks.Data
{
    public enum EnemyTier { Minion, Elite, Boss }

    /// <summary>
    /// Data asset for an enemy.
    /// Create via: Assets > Create > BattleDecks > Entity > Enemy Data
    /// </summary>
    [CreateAssetMenu(menuName = "BattleDecks/Entity/Enemy Data", fileName = "NewEnemyData")]
    public class EnemyData : EntityData
    {
        [Header("Classification")]
        public EnemyTier tier;
        [TextArea(2, 4)]
        public string description;

        [Header("AI")]
        public EnemyIntent[] intentPattern;     // ordered list of intents; loops when exhausted
        public bool shuffleIntents;             // if true, pick randomly rather than in order

        [Header("Loot")]
        public int goldMin;
        public int goldMax;
        public CardData[] possibleCardDrops;    // pool to draw from on death
        public int cardDropCount;
    }

    /// <summary>
    /// A single "move" the enemy telegraphs and then executes.
    /// Serializable inline — no separate asset needed.
    /// </summary>
    [System.Serializable]
    public struct EnemyIntent
    {
        public string intentName;               // display name, e.g. "Strike", "Defend", "Buff"
        public Sprite intentIcon;               // shown in UI during enemy telegraph
        public CardEffectData[] effects;        // effects that fire when this intent executes
    }
}
