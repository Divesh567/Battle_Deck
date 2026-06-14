using UnityEngine;

namespace BattleDecks.Data
{
    public enum WeaponCategory
    {
        Sword, Axe, Dagger, Staff, Bow,
        Wand, Shield, Fist, Spear, Crossbow
    }

    /// <summary>
    /// Defines a warrior archetype (Rogue, Wizard, Paladin, etc.)
    /// Contains class bonuses and weapon/skill proficiencies.
    /// Create via: Assets > Create > BattleDecks > SkillSet Data
    /// </summary>
    [CreateAssetMenu(menuName = "BattleDecks/SkillSet Data", fileName = "NewSkillSet")]
    public class SkillSetData : ScriptableObject
    {
        [Header("Identity")]
        public string className;                  // "Rogue", "Wizard", "Paladin" etc.
        public Sprite classIcon;
        [TextArea(2, 4)]
        public string classDescription;

        [Header("Class Stat Bonuses")]
        public CoreAttributes bonusAttributes;    // added on top of EntityData.core at runtime

        [Header("Weapon Proficiencies")]
        public WeaponProficiency[] weaponProficiencies;

        [Header("Skill Proficiencies")]
        public SkillProficiency[] skillProficiencies;

        [Header("Class Cards")]
        public CardData[] classCardPool;          // cards available only to this class
        public CardData[] signatureCards;         // guaranteed in starting deck for this class

        [Header("Triggers")]
        public TriggerData[] triggers;            // passive triggers that fire during combat
    }

    /// <summary>
    /// How skilled this class is with a weapon category.
    /// Proficiency level gates certain cards and adds flat damage bonuses.
    /// </summary>
    [System.Serializable]
    public struct WeaponProficiency
    {
        public WeaponCategory category;
        [Range(0, 5)] public int level;           // 0 = untrained, 5 = master
        public int flatDamageBonus;               // added to attacks with this weapon type
        [Range(0f, 1f)] public float critBonus;   // additive crit chance with this weapon type
    }

    /// <summary>
    /// Proficiency in a non-weapon skill domain (Stealth, Arcana, Healing, etc.)
    /// Used to gate skill cards and determine effect scaling.
    /// </summary>
    [System.Serializable]
    public struct SkillProficiency
    {
        public string skillName;                  // "Stealth", "Arcana", "Lockpicking" etc.
        [Range(0, 5)] public int level;
        [TextArea(1, 2)]
        public string description;
    }
}
