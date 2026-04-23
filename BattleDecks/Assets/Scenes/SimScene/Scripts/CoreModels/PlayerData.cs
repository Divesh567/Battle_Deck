using UnityEngine;

namespace BattleDecks.Data
{
    /// <summary>
    /// Data asset for a playable character.
    /// Create via: Assets > Create > BattleDecks > Entity > Player Data
    /// </summary>
    [CreateAssetMenu(menuName = "BattleDecks/Entity/Player Data", fileName = "NewPlayerData")]
    public class PlayerData : EntityData
    {
        [Header("Class")]
        public SkillSetData skillSet;           // warrior class definition (Rogue, Wizard, etc.)

        [Header("Starting Deck")]
        public CardData[] startingDeck;         // cards the player begins the run with

        [Header("Progression")]
        public int startingGold;
        public int handSize;                    // cards drawn at turn start
        public int maxHandSize;
    }
}
