using UnityEngine;

namespace BattleDecks.Data
{
    /// <summary>
    /// Abstract base for all entities (players and enemies).
    /// Never instantiated directly — derive PlayerData or EnemyData from this.
    /// SOs that share this base can be referenced polymorphically via EntityData.
    /// </summary>
    public abstract class EntityData : ScriptableObject
    {
        [Header("Identity")]
        public string entityName;
        public Sprite portrait;

        [Header("Core Attributes")]
        public CoreAttributes core;

        [Header("Resistances")]
        public DamageResistances resistances;
    }
}
