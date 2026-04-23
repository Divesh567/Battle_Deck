using UnityEngine;

namespace BattleDecks.Data
{
    public enum DamageType
    {
        Physical, Fire, Ice, Lightning,
        Poison, Arcane, Holy, Shadow
    }

    public enum WeaponSlot { MainHand, OffHand, TwoHanded, Ranged }

    /// <summary>
    /// Abstract base for all weapons. Contains shared stats.
    /// Derive specialised types (MeleeWeaponData, RangedWeaponData, etc.) from this
    /// if those types need unique fields — otherwise use WeaponData directly.
    /// </summary>
    public abstract class WeaponDataBase : ScriptableObject
    {
        [Header("Identity")]
        public string weaponName;
        public Sprite icon;
        [TextArea(1, 3)]
        public string flavourText;

        [Header("Classification")]
        public WeaponCategory category;
        public WeaponSlot slot;
        public DamageType damageType;
        public int requiredProficiencyLevel;      // minimum WeaponProficiency.level to equip

        [Header("Base Stats")]
        [Min(0)] public int baseDamageMin;
        [Min(0)] public int baseDamageMax;
        [Min(0)] public int baseAttackBonus;      // flat bonus stacked on top of entity baseAttack
        [Range(0f, 1f)] public float critMultiplier; // default 1.5 = 150% damage on crit

        [Header("Cards")]
        public CardData[] grantedCards;           // cards added to hand/deck when weapon is equipped
    }

    /// <summary>
    /// Standard concrete weapon asset. Covers the majority of weapons.
    /// Create via: Assets > Create > BattleDecks > Weapon > Weapon Data
    /// </summary>
    [CreateAssetMenu(menuName = "BattleDecks/Weapon/Weapon Data", fileName = "NewWeapon")]
    public class WeaponData : WeaponDataBase
    {
        [Header("Special")]
        public CardEffectData[] onHitEffects;     // effects that trigger on each attack hit
        public int durability;                    // 0 = unbreakable
    }
}
