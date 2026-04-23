using System;
using UnityEngine;

namespace BattleDecks.Data
{
    /// <summary>
    /// Base combat attributes shared by every entity.
    /// Serializable struct so it embeds cleanly into any SO
    /// without needing its own asset file.
    /// </summary>
    [Serializable]
    public struct CoreAttributes
    {
        [Header("Vitals")]
        [Min(1)] public int maxHealth;
        [Min(0)] public int maxArmor;         // starting block/shield pool
        [Min(0)] public int startingEnergy;   // energy available each turn

        [Header("Offense")]
        [Min(0)] public int baseAttack;       // flat bonus added to all attack effects
        [Min(0)] public int baseMagicPower;   // flat bonus added to all spell effects
        [Min(0)] public int initiative;       // determines turn order

        [Header("Defense")]
        [Min(0)] public int baseDodgeChance;  // percentage 0-100
        [Min(0)] public int baseCritChance;   // percentage 0-100
    }
}
