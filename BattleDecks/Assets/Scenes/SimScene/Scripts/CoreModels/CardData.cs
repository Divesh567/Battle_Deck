using UnityEngine;

namespace BattleDecks.Data
{
    public enum CardType   { Attack, Skill, Power, Reaction }
    public enum CardRarity { Common, Uncommon, Rare, Signature }

    /// <summary>
    /// Abstract base for all cards. Contains the full shared contract.
    /// Derive specialised types (e.g. WeaponCardData, SpellCardData) only if
    /// those cards genuinely need unique data fields — avoid premature subclassing.
    /// </summary>
    public abstract class CardDataBase : ScriptableObject
    {
        [Header("Identity")]
        public string cardName;
        public Sprite artwork;
        [TextArea(2, 5)]
        public string cardText;                 // shown on card face, supports {value} tokens
        [TextArea(1, 3)]
        public string flavourText;

        [Header("Classification")]
        public CardType cardType;
        public CardRarity rarity;
        public DamageType damageType;           // None for non-damage cards
        public SkillSetData[] allowedClasses;   // empty = usable by all classes

        [Header("Cost")]
        [Min(0)] public int energyCost;
        /*public bool isEthereal;                 // exhausted at end of turn if unplayed
        public bool isInnate;      */             // always in opening hand

        [Header("Effects")]
        public CardEffectData[] onPlayEffects;  // fires when card is played

        /*[Header("Upgrade")]
        public CardDataBase upgradedVersion;  */  // reference to the + version of this card; null if not upgradeable
    }

    /// <summary>
    /// Standard concrete card asset. The vast majority of cards are this type.
    /// Create via: Assets > Create > BattleDecks > Card > Card Data
    /// </summary>
    [CreateAssetMenu(menuName = "BattleDecks/Card/Card Data", fileName = "NewCard")]
    public class CardData : CardDataBase
    {
        /*
        [Header("Reaction / Trap")]
        public bool isReaction;                 // can be played outside player turn
        public CardEffectData[] reactionTriggerEffects; // what triggers this card if isReaction
        */

        /*[Header("Weapon Requirement")]
        public WeaponCategory[] requiredWeaponCategories; // empty = no weapon requirement*/
    }
}
