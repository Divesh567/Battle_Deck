# Battle Deck — Architecture Review

Current weaknesses in the combat system architecture and directions for future improvement.
Written after implementing: Actions, Reflex cards, Triggers, Passives, Locks, Enemy energy, Success chance, Scouting.

---

## 1. `CardEffectData` is a God Object

**Problem:** Every effect type shares one class, so it accumulates fields for all possible uses — `lockName`, `lockedTags`, `successChance`, `missChance`, `cardToUnlock`, `conditionDescription`, `thenEffects`, `elseEffects`, `applyChance`. Most fields are irrelevant for any given effect. A "Deal Damage" effect carries a `lockName` field that will never be touched. As more effect types are added the class and the Inspector become unmanageable.

**Direction:** Polymorphic effect subclasses — `DealDamageEffect`, `ApplyStatusEffect`, `ApplyLockEffect`, each extending a base `CardEffectData`. Only relevant fields show in the Inspector per type.

---

## 2. `EffectType` Enum + Giant Switch Is a Closed Door

**Problem:** Every new mechanic needs: a new enum value + a new case in a 27-case switch in `ResolveEffect`. The system cannot be extended without modifying the engine's core code. This will compound as the game grows.

**Direction:** Command pattern. Each effect type is a self-contained object with an `Execute(caster, target, engine)` method. New effects are new classes, not new enum cases.

---

## 3. `SimCombatEngine` Is a Monolith

**Problem:** The engine handles turn management, damage calculation, miss/dodge/parry/counter logic, armor, status application, enemy AI, trigger evaluation, lock management, healing, and energy — all in one class (~540 lines and growing). Touching one system risks breaking another.

**Direction:** Dedicated subsystems — `DamageResolver`, `StatusManager`, `TriggerEvaluator`, `EnemyAI`. The engine becomes a thin orchestrator that delegates to these, rather than implementing everything itself.

---

## 4. Status System Is Stringly-Typed

**Problem:** Statuses are a `Dictionary<string, int>` with keys like `"Bleeding"`, `"Parry"`, `"Counter"`, `"Scouted"`, `"Intimidated"`. Any typo is a silent bug. The string `"Counter"` appears across `SimEntityState`, `SimCombatEngine`, and effect SO assets — they all have to stay in sync manually. There is no central registry of valid status names.

**Direction:** A `StatusType` enum or const string class as a single source of truth. Ideally a `StatusData` ScriptableObject — authored once, referenced by GUID, same as cards.

---

## 5. `TickStatuses` Has a Hardcoded Exception List

**Problem:** End-of-turn status ticking uses a string check to skip reactive statuses:
```csharp
if (k == "Dodge" || k == "Counter" || k == "Parry") continue;
```
Every new reactive status requires remembering this line and adding to it. There is nothing in the status data itself that declares its tick behaviour. Fragile implicit contract.

**Direction:** A `StatusBehavior` field on each status — `TickDown`, `ConsumeOnHit`, `Permanent`. The tick loop reads the behaviour flag rather than checking names.

---

## 6. Enemy AI Has No Situational Awareness

**Problem:** The enemy always executes intents in a fixed cycle with no awareness of context. The Bandit will try to Intimidate a player who is already Intimidated, will use Gut Stab at 5 HP, will never adapt. `shuffleIntents` is random but not strategic.

**Direction:** Intents with preconditions — each intent defines when it is eligible (`only if target not Intimidated`, `only if own HP > 30%`). The AI picks the first affordable, eligible intent rather than blindly cycling. This doesn't require a full behaviour tree — just a condition array on each intent, same pattern as card locks.

---

## 7. No Card Instances — Hand Is `List<CardData>`

**Problem:** Cards in hand are direct ScriptableObject references. Two copies of Quick Slash are identical references with no way to distinguish them. There is no "instance" concept, so you cannot mark "this specific copy costs 0 this turn", track individual card history, or handle mid-combat unlocked cards differently from starting deck cards.

**Direction:** A `CardInstance` wrapper around `CardData` with a unique instance ID and per-instance modifier slots. This unblocks card upgrades, temporary cost reduction, and per-instance state tracking.

---

## 8. UI Reaches Directly into Engine Internals

**Problem:** `SimBattlePanel_UI` calls `_engine.GetEnemyNextIntents()`, reads `_engine.Player.HasStatus("Scouted")` with a magic string, accesses `_engine.Player.Hand` directly. The view is tightly coupled to the engine's internal structure. Engine refactors break the UI.

**Direction:** A `CombatViewModel` (or state snapshot struct) that the engine publishes on each `OnStateChanged` event. The UI reads only from the ViewModel. Engine internals can be restructured freely as long as the ViewModel contract holds.

---

## 9. `TriggerCondition` Is Closed to Extension

**Problem:** Same pattern as `EffectType` — adding a new trigger condition (`OnKill`, `OnCardDrawn`, `OnEnemyActed`) requires modifying the enum, the switch in `EvaluateTriggers`, and the SO Inspector layout. Designers cannot add new trigger conditions without a code change.

**Direction:** Trigger conditions as self-contained predicate objects, same solution as effects — a base class with an `Evaluate(actor, opponent, context)` method.

---

## 10. No Structured Event Bus

**Problem:** `OnLogLine` is a string event used for both player-facing narrative text and internal debug output (`[DMG DEBUG] BaseValue: 10`). There is no structured `OnDamageDealt(source, target, amount)` or `OnStatusApplied(status, target)`. This means audio cues, animation hooks, achievements, or analytics all have to parse log strings or dig into engine internals.

**Direction:** A typed event bus — `OnDamageDealt`, `OnCardPlayed`, `OnCombatantDied`, etc. The text log becomes a subscriber to the event bus, not the bus itself.

---

## Priority Order

Issues that will cause real pain soonest, before a proper engine rewrite is worth doing:

| Priority | Issue | Why It Hurts First |
|----------|-------|--------------------|
| 🔴 High | #4 — Stringly-typed statuses | Silent bugs. Easy to add a new status with a typo and have no idea. |
| 🔴 High | #1 — CardEffectData god object | Inspector confusion grows with every new effect type. |
| 🟡 Medium | #5 — TickStatuses exceptions | Will catch people out the first time a new reactive status is added. |
| 🟡 Medium | #6 — Enemy AI no situational awareness | Fights feel mechanical and exploitable quickly. |
| 🟡 Medium | #2 — EffectType switch | Manageable now, painful after ~40 effect types. |
| 🟢 Later | #3 — Engine monolith | Worth splitting when adding a second combat scene or multiplayer. |
| 🟢 Later | #7 — No card instances | Needed when card upgrades or per-copy modifiers are designed. |
| 🟢 Later | #8 — UI coupling | Matters when the UI is rebuilt or a second UI (e.g. mobile) is added. |
| 🟢 Later | #9 — TriggerCondition closed | Fine until trigger types exceed ~6-8. |
| 🟢 Later | #10 — No event bus | Essential before audio/animation/analytics, not before. |
