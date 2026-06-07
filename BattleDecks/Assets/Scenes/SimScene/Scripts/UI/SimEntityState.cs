using System.Collections.Generic;
using BattleDecks.Data;
using UnityEngine;

namespace BattleDecks.Sim
{
    /// <summary>
    /// Mutable runtime snapshot of one combatant.
    /// Never writes back to the SO — SOs stay read-only templates.
    /// </summary>
    public class SimEntityState
    {
        // ── Source data ──────────────────────────────────────────────
        public EntityData   Source        { get; private set; }
        public WeaponData   EquippedWeapon { get; private set; }
        public SkillSetData SkillSet       { get; private set; }

        // ── Vitals ───────────────────────────────────────────────────
        public int MaxHP    { get; private set; }
        public int CurrentHP { get; set; }
        public int Armor    { get; set; }   // resets each turn start
        public int Energy   { get; set; }

        // ── Stats (base + class bonus + weapon bonus) ─────────────────
        public int Attack      { get; private set; }
        public int MagicPower  { get; private set; }
        public int Initiative  { get; private set; }

        // ── Deck / Hand ───────────────────────────────────────────────
        public List<CardData> DrawPile  { get; private set; } = new();
        public List<CardData> Hand      { get; private set; } = new();
        public List<CardData> DiscardPile { get; private set; } = new();

        // ── Active buffs/statuses ─────────────────────────────────────
        public Dictionary<string, int> Statuses { get; private set; } = new(); // name → stacks/turns

        // ── Dodge ─────────────────────────────────────────────────────
        public float DodgeChance { get; set; }   // probability each dodge stack succeeds; set by ApplyDodge

        // ── Flags ─────────────────────────────────────────────────────
        public bool IsDead  => CurrentHP <= 0;
        public bool IsPlayer { get; private set; }

        // ─────────────────────────────────────────────────────────────
        public SimEntityState(EntityData source, SkillSetData skillSet, WeaponData weapon, bool isPlayer)
        {
            Source         = source;
            SkillSet       = skillSet;
            EquippedWeapon = weapon;
            IsPlayer       = isPlayer;

            // flatten stats: entity base + class bonus + weapon bonus
            var core  = source.core;
            var bonus = skillSet != null ? skillSet.bonusAttributes : default;
            int weaponAtk = weapon != null ? weapon.baseAttackBonus : 0;

            MaxHP       = core.maxHealth;
            CurrentHP   = MaxHP;
            Armor       = 0;
            Energy      = core.startingEnergy;
            Attack      = core.baseAttack + bonus.baseAttack + weaponAtk;
            MagicPower  = core.baseMagicPower + bonus.baseMagicPower;
            Initiative  = core.initiative + bonus.initiative;

            // build starting deck
            BuildDeck(source, skillSet, weapon);
        }

        private void BuildDeck(EntityData source, SkillSetData skillSet, WeaponData weapon)
        {
            DrawPile.Clear();
            Hand.Clear();
            DiscardPile.Clear();

            if (source is PlayerData pd)
            {
                if (pd.startingDeck != null)
                    foreach (var c in pd.startingDeck) DrawPile.Add(c);
            }

            if (skillSet?.signatureCards != null)
                foreach (var c in skillSet.signatureCards) DrawPile.Add(c);

            if (skillSet?.classCardPool != null)
                foreach (var c in skillSet.classCardPool) DrawPile.Add(c);

            if (weapon?.grantedCards != null)
                foreach (var c in weapon.grantedCards) DrawPile.Add(c);

            ShuffleDraw();
        }

        // ── Deck helpers ─────────────────────────────────────────────
        public void ShuffleDraw()
        {
            for (int i = DrawPile.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (DrawPile[i], DrawPile[j]) = (DrawPile[j], DrawPile[i]);
            }
        }

        public void DrawCards(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (DrawPile.Count == 0)
                {
                    if (DiscardPile.Count == 0) break;
                    DrawPile.AddRange(DiscardPile);
                    DiscardPile.Clear();
                    ShuffleDraw();
                }
                var card = DrawPile[0];
                DrawPile.RemoveAt(0);
                Hand.Add(card);
            }
        }

        public bool PlayCard(CardData card)
        {
            if (!Hand.Contains(card)) return false;
            if (Energy < card.energyCost) return false;
            Energy -= card.energyCost;
            Hand.Remove(card);
            DiscardPile.Add(card);
            return true;
        }

        public void DiscardHand()
        {
            DiscardPile.AddRange(Hand);
            Hand.Clear();
        }

        // ── Status helpers ────────────────────────────────────────────
        public void ApplyStatus(string status, int stacks)
        {
            if (Statuses.ContainsKey(status))
                Statuses[status] += stacks;
            else
                Statuses[status] = stacks;
        }

        public bool HasStatus(string status) =>
            Statuses.TryGetValue(status, out int v) && v > 0;

        public int GetStatus(string status) =>
            Statuses.TryGetValue(status, out int v) ? v : 0;

        public void TickStatuses()
        {
            var keys = new List<string>(Statuses.Keys);
            foreach (var k in keys)
            {
                if (k == "Dodge") continue;   // dodge is consumed by attacks, not time
                Statuses[k]--;
                if (Statuses[k] <= 0) Statuses.Remove(k);
            }
        }

        // ── Turn reset ────────────────────────────────────────────────
        public void OnTurnStart(int handSize)
        {
            Armor  = Source.core.maxArmor;      // reset armor each turn
            Energy = Source.core.startingEnergy;
            DrawCards(handSize);
        }

        public void OnTurnEnd()
        {
            DiscardHand();
            TickStatuses();
        }

        public override string ToString() =>
            $"{Source.entityName}  HP:{CurrentHP}/{MaxHP}  ARM:{Armor}  NRG:{Energy}";
    }
}
