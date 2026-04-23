using System;
using UnityEngine;

namespace BattleDecks.Data
{
    /// <summary>
    /// Per-damage-type resistance values for an entity.
    /// Values are multipliers: 1.0 = normal, 0.5 = half damage, 0.0 = immune, 1.5 = vulnerable.
    /// Embedded directly into EntityData — no separate asset needed.
    /// </summary>
    [Serializable]
    public struct DamageResistances
    {
        [Range(0f, 2f)] public float physical;
        [Range(0f, 2f)] public float fire;
        [Range(0f, 2f)] public float ice;
        [Range(0f, 2f)] public float lightning;
        [Range(0f, 2f)] public float poison;
        [Range(0f, 2f)] public float arcane;
        [Range(0f, 2f)] public float holy;
        [Range(0f, 2f)] public float shadow;

        public static DamageResistances Default => new DamageResistances
        {
            physical  = 1f,
            fire      = 1f,
            ice       = 1f,
            lightning = 1f,
            poison    = 1f,
            arcane    = 1f,
            holy      = 1f,
            shadow    = 1f,
        };
    }
}
