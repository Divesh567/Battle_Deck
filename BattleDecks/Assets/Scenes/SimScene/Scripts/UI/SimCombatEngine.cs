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
            var other  = IsPlayerTurn ? Enemy : Player;
            Log($"─── Turn {TurnNumber}: {active.Source.entityName} ───");

            int drawCount = active.Source is PlayerData pd ? pd.handSize : 3;
            active.OnTurnStart(drawCount);

            ProcessStartOfTurnStatuses(active);

            // ❗ STOP if combat ended from status
            if (IsCombatOver) return;

            EvaluateTriggers(active, other, TriggerCondition.OnTurnStart);

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
            if (!Player.IsCardPlayable(card))
            {
                Log($"  Cannot play {card.cardName} — it is locked!");
                return false;
            }

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

            // success chance: 0 always succeeds; >0 is the probability the effect fires
            if (fx.successChance > 0f && Random.value > fx.successChance)
            {
                Log($"    {caster.Source.entityName}'s {fx.effectName} failed!");
                return;
            }

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

                case EffectType.GainDodge:
                    ApplyDodge(target, value, fx.dodgeChance);
                    break;

                // ── Statuses ──────────────────────────────────────────
                case EffectType.ApplyBleeding:
                    TryApplyStatus(caster, target, "Bleeding",   fx.baseValue,      fx.applyChance, target.Source.statusResistances.bleeding);   break;
                case EffectType.ApplyStun:
                    TryApplyStatus(caster, target, "Stun",       fx.statusDuration, fx.applyChance, target.Source.statusResistances.stun);        break;
                case EffectType.ApplyVulnerable:
                    TryApplyStatus(caster, target, "Vulnerable", fx.statusDuration, fx.applyChance, target.Source.statusResistances.vulnerable);  break;
                case EffectType.ApplyExploited:
                    TryApplyStatus(caster, target, "Exploited",  fx.statusDuration, fx.applyChance, 0f);                                         break;
                case EffectType.ApplyLock:
                    ApplyLockEffect(target, fx.lockName, fx.lockedTags, fx.statusDuration);
                    break;
                case EffectType.ApplyCounter:
                    target.ApplyStatus("Counter", fx.baseValue);
                    Log($"    {target.Source.entityName} readies a counter [{fx.baseValue} dmg]");
                    break;
                case EffectType.ApplyParry:
                    target.ApplyStatus("Parry", fx.baseValue);
                    Log($"    {target.Source.entityName} readies a parry [{fx.baseValue} reduction]");
                    break;
                case EffectType.AddCardToHand:
                    if (fx.cardToUnlock != null)
                    {
                        target.Hand.Add(fx.cardToUnlock);
                        Log($"    [{fx.cardToUnlock.cardName}] added to {target.Source.entityName}'s hand!");
                    }
                    break;

                case EffectType.RevealEnemyIntent:
                    caster.ApplyStatus("Scouted", 1);
                    Log($"    {caster.Source.entityName} reads the enemy's next move!");
                    break;
                /*case EffectType.ApplyPoison:   TryApplyStatus(caster, target, "Poison",   fx.baseValue,      fx.applyChance, 0f); break;
                case EffectType.ApplyFreeze:     TryApplyStatus(caster, target, "Freeze",   fx.statusDuration, fx.applyChance, 0f); break;
                case EffectType.ApplyWeak:       TryApplyStatus(caster, target, "Weak",     fx.statusDuration, fx.applyChance, 0f); break;
                case EffectType.ApplyStrength:   TryApplyStatus(caster, target, "Strength", fx.baseValue,      fx.applyChance, 0f); break;*/

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

            // miss
            if (fx.missChance > 0f && Random.value < fx.missChance)
            {
                Log($"    {caster.Source.entityName}'s attack misses!");
                if (fx.onMissEffects != null && fx.onMissEffects.Length > 0)
                    ResolveEffects(fx.onMissEffects, caster, target);
                return;
            }

            // dodge — one stack consumed per incoming attack regardless of success
            if (target.HasStatus("Dodge"))
            {
                bool dodged = Random.value < target.DodgeChance;
                target.Statuses["Dodge"]--;
                if (target.Statuses["Dodge"] <= 0)
                {
                    target.Statuses.Remove("Dodge");
                    target.DodgeChance = 0f;
                }
                if (dodged)
                {
                    Log($"    {target.Source.entityName} dodges the attack!  [Dodge stacks left: {target.GetStatus("Dodge")}]");
                    return;
                }
                Log($"    {target.Source.entityName}'s dodge fails!  [Dodge stacks left: {target.GetStatus("Dodge")}]");
            }

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

            // parry — deflect before armor
            if (target.HasStatus("Parry"))
            {
                int block = target.GetStatus("Parry");
                finalDmg = Mathf.Max(0, finalDmg - block);
                target.Statuses.Remove("Parry");
                Log($"    {target.Source.entityName} parries! [{block} damage blocked]");
                if (finalDmg == 0) return;
            }

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
            Log($"    {target.Source.entityName} takes {finalDmg} {fx.damageType} damage{critTag} [HP: {target.CurrentHP}/{target.MaxHP}]");

            // counter — retaliate after absorbing the hit
            if (finalDmg > 0 && target.HasStatus("Counter"))
            {
                int counterDmg = target.GetStatus("Counter");
                target.Statuses.Remove("Counter");
                caster.CurrentHP -= counterDmg;
                Log($"    {target.Source.entityName} counters for {counterDmg}!  [{caster.Source.entityName} HP: {caster.CurrentHP}/{caster.MaxHP}]");
            }

            // low-health trigger check
            if (!target.IsDead)
                EvaluateTriggers(target, caster, TriggerCondition.OnLowHealth);
        }

        private void ApplyDodge(SimEntityState target, int stacks, float chance)
        {
            target.ApplyStatus("Dodge", stacks);
            target.DodgeChance = chance;
            Log($"    {target.Source.entityName} prepares to dodge  " +
                $"[{target.GetStatus("Dodge")} stack(s), {Mathf.RoundToInt(chance * 100)}% chance each]");
        }

        private void TryApplyStatus(SimEntityState caster, SimEntityState target, string status, int amount, float applyChance, float resistance)
        {
            // roll 1: base apply chance (applyChance == 0 means always applies)
            float effectiveChance = applyChance > 0f ? applyChance : 1f;
            if (Random.value > effectiveChance)
            {
                Log($"    {status} failed to apply to {target.Source.entityName}.");
                return;
            }

            // roll 2: target's resistance to this status
            if (resistance > 0f && Random.value < resistance)
            {
                Log($"    {target.Source.entityName} resisted {status}!");
                return;
            }

            ApplyStatus(target, status, amount);

            // fire OnEnemyHasStatus triggers for whoever applied the status
            if (caster != null && caster != target)
                EvaluateTriggers(caster, target, TriggerCondition.OnEnemyHasStatus, status);
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
                EndTurn();
                return;
            }

            if (ed.shuffleIntents)
                _enemyIntentIndex = UnityEngine.Random.Range(0, ed.intentPattern.Length);

            int total = ed.intentPattern.Length;
            int acted = 0;

            while (!IsCombatOver)
            {
                var intent = ed.intentPattern[_enemyIntentIndex % total];
                int cost = intent.energyCost;

                // can't afford next action — stop for this turn
                if (cost > 0 && Enemy.Energy < cost) break;

                Log($"  ► {Enemy.Source.entityName} uses [{intent.intentName}]");
                Enemy.Energy -= cost;
                ResolveEffects(intent.effects, Enemy, Player);
                _enemyIntentIndex++;
                acted++;

                // free (cost=0) actions execute once then stop
                if (cost == 0) break;
            }

            if (acted == 0)
                Log($"  {Enemy.Source.entityName} has no energy to act.");

            EndTurn();
        }

        /// <summary>Returns the ordered list of intents the enemy will execute on their next turn, given their current energy pool.</summary>
        public List<EnemyIntent> GetEnemyNextIntents()
        {
            var result = new List<EnemyIntent>();
            if (Enemy.Source is not EnemyData ed || ed.intentPattern == null || ed.intentPattern.Length == 0)
                return result;

            int energy = Enemy.Source.core.startingEnergy;
            int total  = ed.intentPattern.Length;
            int idx    = _enemyIntentIndex;

            for (int i = 0; i < total; i++)
            {
                var intent = ed.intentPattern[idx % total];
                int cost   = intent.energyCost;

                if (cost > 0 && energy < cost) break;

                result.Add(intent);
                energy -= cost;
                idx++;

                if (cost == 0) break;
            }
            return result;
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

        private void ApplyLockEffect(SimEntityState target, string lockName, CardTag lockedTags, int duration)
        {
            if (string.IsNullOrEmpty(lockName)) return;
            int dur = duration > 0 ? duration : 1;
            target.ActiveLocks[lockName] = lockedTags;
            target.ApplyStatus(lockName, dur);
            Log($"    {target.Source.entityName} is {lockName}! ({lockedTags} cards locked for {dur} turns)");
        }

        private void EvaluateTriggers(SimEntityState actor, SimEntityState opponent, TriggerCondition condition, string statusParam = null)
        {
            if (actor.SkillSet?.triggers == null) return;

            foreach (var trigger in actor.SkillSet.triggers)
            {
                if (trigger == null) continue;
                if (trigger.oneShot && actor.FiredOneShotTriggers.Contains(trigger)) continue;
                if (trigger.condition != condition) continue;

                bool fires = condition switch
                {
                    TriggerCondition.OnTurnStart      => true,
                    TriggerCondition.OnLowHealth      => actor.CurrentHP <= actor.MaxHP * trigger.hpThreshold,
                    TriggerCondition.OnEnemyHasStatus => string.Equals(trigger.statusToCheck, statusParam, StringComparison.OrdinalIgnoreCase),
                    _                                 => false,
                };

                if (!fires) continue;

                Log($"  ✦ {trigger.name} triggered!");

                if (trigger.effects?.Length > 0)
                    ResolveEffects(trigger.effects, actor, opponent);

                if (trigger.cardToUnlock != null)
                {
                    actor.Hand.Add(trigger.cardToUnlock);
                    Log($"    [{trigger.cardToUnlock.cardName}] unlocked and added to hand!");
                }

                if (trigger.oneShot) actor.FiredOneShotTriggers.Add(trigger);
            }
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
