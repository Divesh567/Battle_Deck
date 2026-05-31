using System;
using System.Collections.Generic;
using BattleDecks.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BattleDecks.Sim
{
    /// <summary>
    /// Pure C# combat engine — no MonoBehaviour, no Unity UI.
    /// Drives the turn loop and resolves CardEffectData against SimEntityStates.
    /// The UI layer calls into this and reads back log lines + state.
    /// </summary>
    public class SimCombatEngine
    {
        // ── State ─────────────────────────────────────────────────────
        public SimEntityState Player  { get; private set; }
        public SimEntityState Enemy   { get; private set; }
        public int            TurnNumber { get; private set; } = 1;
        public bool           IsPlayerTurn { get; private set; } = true;
        public bool           IsCombatOver => Player.IsDead || Enemy.IsDead;

        // ── Events ────────────────────────────────────────────────────
        public event Action<string> OnLogLine;      // UI subscribes to display log
        public event Action         OnStateChanged; // UI subscribes to refresh displays

        // ─────────────────────────────────────────────────────────────
        public SimCombatEngine(SimEntityState player, SimEntityState enemy)
        {
            Player = player;
            Enemy  = enemy;
        }

        // ── Turn management ───────────────────────────────────────────
        public void StartCombat()
        {
            Log("═══ COMBAT START ═══");
            Log($"  {Player.Source.entityName} vs {Enemy.Source.entityName}");
            Log($"  Initiative: {Player.Source.entityName} {Player.Initiative} | {Enemy.Source.entityName} {Enemy.Initiative}");

            // higher initiative goes first
            IsPlayerTurn = Player.Initiative >= Enemy.Initiative;
            BeginTurn();
        }

        public void BeginTurn()
        {
            var active = IsPlayerTurn ? Player : Enemy;
            Log($"─── Turn {TurnNumber}: {active.Source.entityName} ───");

            int drawCount = active.Source is PlayerData pd ? pd.handSize : 3;
            active.OnTurnStart(drawCount);

            // ✅ NEW STEP
            ProcessStartOfTurnStatuses(active);

            // ❗ STOP if combat ended from status
            if (IsCombatOver) return;

            if (active.HasStatus("Stun"))
            {
                Log($"  {active.Source.entityName} is stunned and loses their turn!");
                EndTurn();
                return;
            }

            if (!IsPlayerTurn)
                RunEnemyTurn();

            OnStateChanged?.Invoke();
        }
        
        private void ProcessStartOfTurnStatuses(SimEntityState entity)
        {
            if (entity.Statuses == null || entity.Statuses.Count == 0) return;

            // Copy keys to avoid modifying dictionary during iteration
            var statusKeys = new List<string>(entity.Statuses.Keys);

            foreach (var status in statusKeys)
            {
                int value = entity.GetStatus(status);

                switch (status)
                {
                    case "Bleeding":
                        ApplyBleeding(entity, value);
                        break;

                    // Future:
                    // case "Poison": ApplyPoison(...)
                    // case "Burn": ApplyBurn(...)
                }
                
            }

            // 👇 CRITICAL: death check AFTER all ticks
            if (entity.IsDead)
            {
                Log($"  {entity.Source.entityName} dies from status effects!");
                FinishCombat();
            }
        }
        
        private void ApplyBleeding(SimEntityState target, int amount)
        {
            target.CurrentHP -= amount;

            Log($"    {target.Source.entityName} takes {amount} bleeding damage " +
                $"[HP: {target.CurrentHP}/{target.MaxHP}]");
        }

        /// <summary>Called by UI when player clicks "End Turn".</summary>
        public void PlayerEndTurn()
        {
            if (!IsPlayerTurn || IsCombatOver) return;
            EndTurn();
        }

        private void EndTurn()
        {
            var active = IsPlayerTurn ? Player : Enemy;
            active.OnTurnEnd();

            if (IsCombatOver) { FinishCombat(); return; }

            IsPlayerTurn = !IsPlayerTurn;
            TurnNumber++;
            BeginTurn();
        }

        // ── Card play ─────────────────────────────────────────────────
        /// <summary>Called by UI when player clicks a card in hand.</summary>
        public bool PlayerPlayCard(CardData card)
        {
            if (!IsPlayerTurn || IsCombatOver) return false;
            if (!Player.PlayCard(card))
            {
                Log($"  Cannot play {card.cardName} (not enough energy or not in hand)");
                return false;
            }

            Log($"  ▶ {Player.Source.entityName} plays [{card.cardName}]");
            ResolveEffects(card.onPlayEffects, Player, Enemy);

            OnStateChanged?.Invoke();
            if (IsCombatOver) FinishCombat();
            return true;
        }

        // ── Effect resolution ─────────────────────────────────────────
        public void ResolveEffects(CardEffectData[] effects, SimEntityState caster, SimEntityState opponent)
        {
            if (effects == null) return;
            foreach (var fx in effects)
                ResolveEffect(fx, caster, opponent);
        }

        private void ResolveEffect(CardEffectData fx, SimEntityState caster, SimEntityState opponent)
        {
            if (fx == null) return;

            SimEntityState target = ResolveTarget(fx.target, caster, opponent);
            int value = ComputeValue(fx, caster);
            
            Debug.Log("Effect value is" + value);
            switch (fx.effectType)
            {
                // ── Damage ────────────────────────────────────────────
                case EffectType.DealDamage:
                case EffectType.DealDamageAllEnemies:
                case EffectType.DealDamageRandomEnemy:
                    ApplyDamage(fx, caster, target, value);
                    break;

                // ── Healing ───────────────────────────────────────────
                case EffectType.Heal:
                case EffectType.HealAllAllies:
                    int healed = Mathf.Min(value, target.MaxHP - target.CurrentHP);
                    target.CurrentHP += healed;
                    Log($"    {target.Source.entityName} heals {healed} HP  [{target.CurrentHP}/{target.MaxHP}]");
                    break;

                // ── Defense ───────────────────────────────────────────
                case EffectType.GainArmor:
                    target.Armor += value;
                    Log($"    {target.Source.entityName} gains {value} armor  [ARM: {target.Armor}]");
                    break;

                // ── Statuses ──────────────────────────────────────────
                case EffectType.ApplyBleeding:      ApplyStatus(target, "Bleeding", fx.baseValue); break;
                /*case EffectType.ApplyPoison:    ApplyStatus(target, "Poison",    fx.statusStacks > 0 ? fx.statusStacks : value); break;
                case EffectType.ApplyFreeze:    ApplyStatus(target, "Freeze",    fx.statusDuration); break;
                case EffectType.ApplyStun:      ApplyStatus(target, "Stun",      fx.statusDuration); break;
                case EffectType.ApplyVulnerable:ApplyStatus(target, "Vulnerable",fx.statusDuration); break;
                case EffectType.ApplyWeak:      ApplyStatus(target, "Weak",      fx.statusDuration); break;
                case EffectType.ApplyStrength:  ApplyStatus(target, "Strength",  fx.statusStacks > 0 ? fx.statusStacks : value); break;*/

                // ── Card manipulation ─────────────────────────────────
                case EffectType.DrawCards:
                    target.DrawCards(value);
                    Log($"    {target.Source.entityName} draws {value} card(s)  [Hand: {target.Hand.Count}]");
                    break;

                case EffectType.GainEnergy:
                    target.Energy += value;
                    Log($"    {target.Source.entityName} gains {value} energy  [NRG: {target.Energy}]");
                    break;

                // ── Conditional ───────────────────────────────────────
                case EffectType.Conditional:
                    bool conditionMet = EvaluateCondition(fx, caster, target);
                    Log($"    Condition [{fx.conditionDescription}]: {(conditionMet ? "MET" : "not met")}");
                    if (conditionMet)
                        ResolveEffects(fx.thenEffects, caster, opponent);
                    else
                        ResolveEffects(fx.elseEffects, caster, opponent);
                    break;
            }
        }

        private void ApplyDamage(CardEffectData fx, SimEntityState caster, SimEntityState target, int baseValue)
        {
            if (target == null) return;

            float amount = baseValue;
            Log($"[DMG DEBUG] BaseValue: {baseValue}");

            // weakness
            if (caster.HasStatus("Weak"))
            {
                amount *= 0.75f;
                Log($"[DMG DEBUG] After Weak (0.75x): {amount}");
            }

            // vulnerable
            if (target.HasStatus("Vulnerable"))
            {
                amount *= 1.5f;
                Log($"[DMG DEBUG] After Vulnerable (1.5x): {amount}");
            }

            /*// resistance
            float resistance = GetResistance(target, fx.damageType);
            amount *= resistance;
            Log($"[DMG DEBUG] After Resistance ({resistance}x): {amount}");*/

            // crit
            bool isCrit = fx.canCrit && Random.value < (caster.Source.core.baseCritChance / 100f);
            if (isCrit)
            {
                amount *= 2f;
                Log($"[DMG DEBUG] After Crit (2x): {amount}");
            }

            // rounding
            int rounded = Mathf.RoundToInt(amount);
            Log($"[DMG DEBUG] Rounded: {rounded}");

            int finalDmg = Mathf.Max(1, rounded);
            Log($"[DMG DEBUG] After Clamp (min 1): {finalDmg}");

            // armor
            if (!fx.pierceArmor && target.Armor > 0)
            {
                int absorbed = Mathf.Min(target.Armor, finalDmg);
                target.Armor -= absorbed;
                finalDmg     -= absorbed;

                Log($"[DMG DEBUG] Armor Absorbed: {absorbed}, Remaining Damage: {finalDmg}");
            }

            target.CurrentHP -= finalDmg;

            string critTag = isCrit ? " (CRIT!)" : "";
           // string resTag  = resistance != 1f ? $" (x{resistance:0.0} res)" : "";

            Log($"    {target.Source.entityName} takes {finalDmg} {fx.damageType} damage{critTag} [HP: {target.CurrentHP}/{target.MaxHP}]");
        }

        private void ApplyStatus(SimEntityState target, string status, int amount)
        {
            target.ApplyStatus(status, amount);
            Log($"    {target.Source.entityName} gains {amount}x {status}  [now: {target.GetStatus(status)}]");
        }

        // ── Enemy AI ──────────────────────────────────────────────────
        private int _enemyIntentIndex = 0;

        private void RunEnemyTurn()
        {
            if (Enemy.Source is not EnemyData ed || ed.intentPattern == null || ed.intentPattern.Length == 0)
            {
                Log($"  {Enemy.Source.entityName} does nothing.");
                return;
            }

            if (ed.shuffleIntents)
                _enemyIntentIndex = UnityEngine.Random.Range(0, ed.intentPattern.Length);

            var intent = ed.intentPattern[_enemyIntentIndex % ed.intentPattern.Length];
            Log($"  ► {Enemy.Source.entityName} uses [{intent.intentName}]");
            ResolveEffects(intent.effects, Enemy, Player);

            _enemyIntentIndex++;
            EndTurn();
        }

        // ── Helpers ───────────────────────────────────────────────────
        private int ComputeValue(CardEffectData fx, SimEntityState caster)
        {
            float val = fx.baseValue;
            float scaling = fx.scalingMultiplier;

            /*val += fx.scalingSource switch
            {
                ScalingSource.BaseAttack    => caster.Attack * scaling,
                ScalingSource.MagicPower    => caster.MagicPower * scaling,
                ScalingSource.MissingHealth => (caster.MaxHP - caster.CurrentHP) * scaling,
                ScalingSource.CardsInHand   => caster.Hand.Count * scaling,
                ScalingSource.ArmorAmount   => caster.Armor * scaling,
                _                           => 0,
            };*/

            // // strength bonus adds to all damage/heal effects
            // if (caster.HasStatus("Strength"))
            //     val += caster.GetStatus("Strength");

            return Mathf.Max(0, Mathf.RoundToInt(val));
        }

        private SimEntityState ResolveTarget(EffectTarget t, SimEntityState caster, SimEntityState opponent) => t switch
        {
            EffectTarget.Self          => caster,
            EffectTarget.SingleEnemy   => opponent,
            EffectTarget.AllEnemies    => opponent,
            EffectTarget.RandomEnemy   => opponent,
            EffectTarget.SingleAlly    => caster,
            EffectTarget.AllAllies     => caster,
            EffectTarget.Everyone      => opponent,    // simplified: hits opponent
            _                          => opponent,
        };

        private float GetResistance(SimEntityState target, DamageType dmgType) => dmgType switch
        {
            DamageType.Physical  => target.Source.resistances.physical,
            DamageType.Fire      => target.Source.resistances.fire,
            DamageType.Ice       => target.Source.resistances.ice,
            DamageType.Lightning => target.Source.resistances.lightning,
            DamageType.Poison    => target.Source.resistances.poison,
            DamageType.Arcane    => target.Source.resistances.arcane,
            DamageType.Holy      => target.Source.resistances.holy,
            DamageType.Shadow    => target.Source.resistances.shadow,
            _                    => 1f,
        };

        private bool EvaluateCondition(CardEffectData fx, SimEntityState caster, SimEntityState target)
        {
            // Evaluate based on condition description keywords — extend as needed
            string cond = fx.conditionDescription?.ToLower() ?? "";
            if (cond.Contains("burn"))       return target.HasStatus("Burn");
            if (cond.Contains("poison"))     return target.HasStatus("Poison");
            if (cond.Contains("stun"))       return target.HasStatus("Stun");
            if (cond.Contains("frozen"))     return target.HasStatus("Freeze");
            if (cond.Contains("vulnerable")) return target.HasStatus("Vulnerable");
            if (cond.Contains("full hp"))    return caster.CurrentHP == caster.MaxHP;
            if (cond.Contains("low hp"))     return caster.CurrentHP <= caster.MaxHP * 0.25f;
            return false;
        }

        private void FinishCombat()
        {
            Log("═══ COMBAT OVER ═══");
            if (Player.IsDead && Enemy.IsDead) Log("  Draw!");
            else if (Player.IsDead)            Log($"  {Enemy.Source.entityName} wins!");
            else                               Log($"  {Player.Source.entityName} wins!");
            OnStateChanged?.Invoke();
        }

        private void Log(string line) => OnLogLine?.Invoke(line);
    }
}
