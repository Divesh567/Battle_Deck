using UnityEngine;

namespace BattleDecks.Data
{
    public enum EffectType
    {
        // Damage
        DealDamage, DealDamageAllEnemies, DealDamageRandomEnemy,
        // Healing
        Heal, HealAllAllies,
        // Defense
        GainArmor, GainDodge,
        // Status application
        ApplyBurn, ApplyPoison, ApplyFreeze, ApplyStun,
        ApplyVulnerable, ApplyWeak, ApplyStrength,
        ApplyBleeding,
        // Card manipulation
        DrawCards, DiscardCards, AddCardToHand, ExhaustCard,
        // Energy
        GainEnergy, LoseEnergy,
        // Conditional trigger (fires only if predicate is true)
        Conditional,
        // Lock
        ApplyLock,
        // Reactive statuses (consumed on next incoming hit, not time-ticked)
        ApplyCounter,
        ApplyParry,
        // Status
        ApplyExploited,
        // Information
        RevealEnemyIntent,
    }

    public enum EffectTarget
    {
        Self,
        SingleEnemy,        // targeted by player
        AllEnemies,
        RandomEnemy,
        SingleAlly,
        AllAllies,
        Everyone,
    }

    public enum ScalingSource
    {
        None,
        BaseAttack,         // scales with entity's baseAttack stat
        MagicPower,         // scales with entity's baseMagicPower stat
        MissingHealth,      // stronger when closer to death
        CardsInHand,        // scales with current hand size
        ArmorAmount,        // scales with current armor
        TurnsElapsed,       // scales with how many turns have passed
    }

    /// <summary>
    /// One atomic effect unit. Cards hold arrays of these.
    /// Being a SO means you can author "Deal 5 Fire damage" once
    /// and reference it from dozens of cards without duplication.
    /// Create via: Assets > Create > BattleDecks > Effect > Card Effect
    /// </summary>
    [CreateAssetMenu(menuName = "BattleDecks/Effect/Card Effect", fileName = "NewCardEffect")]
    public class CardEffectData : ScriptableObject
    {
        [Header("Identity")]
        public string effectName;               // e.g. "Deal Fire Damage"
        [TextArea(1, 2)]
        public string description;              // shown in card tooltip, e.g. "Deal {value} fire damage"

        [Header("Effect")]
        public EffectType effectType;
        public EffectTarget target;
        public DamageType damageType;           // only relevant for damage effects

        [Header("Value")]
        [Min(0)] public int baseValue;          // base amount (damage, heal, armor, etc.)
        public ScalingSource scalingSource;
        public float scalingMultiplier;         // baseValue + (scalingStat * multiplier)

        [Header("Status Effects")]
        [Min(0)] public int statusDuration;         // turns; 0 = permanent
        [Range(0f, 1f)] public float applyChance;   // 0 = always applies; values > 0 are the probability the status lands

        [Header("Dodge")]
        [Range(0f, 1f)] public float dodgeChance;   // GainDodge effects only: probability each dodge attempt succeeds

        [Header("Conditional (EffectType.Conditional only)")]
        [TextArea(1, 2)]
        public string conditionDescription;     // human-readable, e.g. "If target is Burning"
        public CardEffectData[] thenEffects;    // fired if condition passes
        public CardEffectData[] elseEffects;    // fired if condition fails (optional)

        [Header("Flags")]
        public bool canCrit;
        public bool pierceArmor;
        public bool triggerOnHitEffects;        // does this hit proc weapon on-hit effects?

        [Header("Success Chance")]
        [Range(0f, 1f)] public float successChance; // 0 = always succeeds; >0 = probability the effect fires at all (e.g. Parry: 0.7)

        [Header("Miss")]
        [Range(0f, 1f)] public float missChance;    // 0 = never misses; only used on damage effects
        public CardEffectData[] onMissEffects;       // fires on the CASTER when this attack misses (e.g. self-expose)

        [Header("Lock")]
        public string lockName;                      // label for this lock (e.g. "Intimidated")
        public CardTag lockedTags;                   // which card tags are blocked while lock is active

        [Header("Card Unlock")]
        public CardData cardToUnlock;                // for AddCardToHand: the card to add to hand
    }
}
