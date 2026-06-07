using System;
using UnityEngine;

namespace BattleDecks.Data
{
    /// <summary>
    /// Per-status-type resistance for an entity.
    /// 0 = no resistance (status always lands if apply chance passes).
    /// 1 = fully immune (status never lands).
    /// Embedded directly into EntityData — no separate asset needed.
    /// </summary>
    [Serializable]
    public struct StatusResistances
    {
        [Range(0f, 1f)] public float stun;
        [Range(0f, 1f)] public float bleeding;
        [Range(0f, 1f)] public float vulnerable;
        // Add more fields here as new statuses are implemented:
        // [Range(0f, 1f)] public float poison;
        // [Range(0f, 1f)] public float freeze;
        // [Range(0f, 1f)] public float weak;

        public static StatusResistances Default => new StatusResistances
        {
            stun      = 0f,
            bleeding  = 0f,
            vulnerable = 0f,
        };
    }
}
