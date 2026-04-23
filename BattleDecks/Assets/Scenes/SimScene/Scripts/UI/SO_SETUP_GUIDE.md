# Battle Decks — Starter SO Setup Guide

A concrete set of assets to create in Unity right now.
Each entry maps directly to a CreateAssetMenu path.

---

## ENEMIES  (Assets > Create > BattleDecks > Entity > Enemy Data)

### 1. Goblin Scrapper  [Minion]
- maxHealth: 20  baseAttack: 4  initiative: 8  startingEnergy: 1
- Resistances: all 1.0 (no special resistances)
- Intent pattern (loops):
    1. Scratch  — DealDamage / Physical / baseValue:4 / target:SingleEnemy
    2. Scratch  — same
    3. Cower    — GainArmor / baseValue:4 / target:Self
- Description: "A fast but fragile grunt. Low damage but acts first."

### 2. Stone Golem  [Elite]
- maxHealth: 60  baseAttack: 8  initiative: 2  maxArmor: 6
- Resistances: physical 0.5, fire 1.0, lightning 1.5
- Intent pattern:
    1. Slam     — DealDamage / Physical / baseValue:10 / target:SingleEnemy / canCrit:true
    2. Fortify  — GainArmor / baseValue:8 / target:Self
    3. Slam
    4. Boulder Toss — DealDamage / Physical / baseValue:14 / pierceArmor:true / target:SingleEnemy
- Description: "Slow but hits hard. Resistant to physical. Weak to lightning."

### 3. Plague Rat  [Minion]
- maxHealth: 14  baseAttack: 2  initiative: 10
- Intent pattern:
    1. Nibble   — DealDamage / Physical / baseValue:2
    2. Infect   — ApplyPoison / statusStacks:2 / target:SingleEnemy
    3. Nibble
    4. Infect
- Description: "Weak hits but stacks Poison fast. Kill it before it stacks."

### 4. Bandit Captain  [Elite]
- maxHealth: 45  baseAttack: 7  initiative: 6  baseCritChance: 20
- Intent pattern:
    1. Slash      — DealDamage / Physical / baseValue:8 / canCrit:true
    2. Blind Dust — ApplyVulnerable / statusDuration:2 / target:SingleEnemy
    3. Slash
    4. Cheap Shot — DealDamage / Physical / baseValue:5 + ApplyWeak / statusDuration:2
- Description: "Applies Vulnerable then punishes it. Has a 20% crit chance."

### 5. Lich Acolyte  [Elite]
- maxHealth: 35  baseAttack: 0  baseMagicPower: 9  initiative: 5
- Resistances: physical 1.5 (weak to physical), arcane 0.0 (immune), holy 0.5
- Intent pattern:
    1. Arcane Bolt — DealDamage / Arcane / baseValue:0 / scalingSource:MagicPower / scalingMultiplier:1.0
    2. Drain Life   — DealDamage / Shadow / baseValue:5 + Heal / baseValue:5 / target:Self
    3. Curse        — ApplyVulnerable / statusDuration:2 + ApplyWeak / statusDuration:2
- Description: "Pure magic attacker. Physical hits hit harder on it."

---

## WEAPONS  (Assets > Create > BattleDecks > Weapon > Weapon Data)

### 1. Rusted Dagger  [Dagger / MainHand]
- damageMin:2  damageMax:5  baseAttackBonus:1  requiredProficiency:0
- grantedCards: Quick Stab, Backstab
- onHitEffects: none
- flavourText: "Chipped but still sharp enough."

### 2. Wooden Club  [Axe / MainHand]  (upgrade path from Tree Branch)
- damageMin:4  damageMax:7  baseAttackBonus:2  requiredProficiency:0
- grantedCards: Club Smash, Dizzy Blow
- flavourText: "Heavier than it looks."

### 3. Iron Sword  [Sword / MainHand]
- damageMin:5  damageMax:9  baseAttackBonus:3  requiredProficiency:1
- grantedCards: Slash, Parry, Pommel Strike
- critMultiplier: 1.75
- flavourText: "Standard issue. Reliable."

### 4. Apprentice Staff  [Staff / TwoHanded]
- damageMin:2  damageMax:4  baseAttackBonus:0  requiredProficiency:1
- grantedCards: Magic Missile, Arcane Shield, Mana Surge
- onHitEffects: ApplyBurn / statusStacks:1 (every hit applies 1 Burn)
- flavourText: "Etched with runes that still faintly glow."

### 5. Hunter's Shortbow  [Bow / Ranged]
- damageMin:4  damageMax:8  baseAttackBonus:2  requiredProficiency:1
- grantedCards: Aimed Shot, Poison Arrow, Evade
- critMultiplier: 2.0  baseCritChance bonus: 10%
- flavourText: "Draw, aim, release."

---

## CARDS  (Assets > Create > BattleDecks > Card > Card Data)

### Attack Cards

**Quick Stab**  [Attack / Common / 1 NRG]
- onPlayEffects: DealDamage / Physical / baseValue:4 / scalingSource:BaseAttack / scalingMultiplier:0.5
- cardText: "Deal 4 + (ATK×0.5) physical damage."

**Backstab**  [Attack / Uncommon / 1 NRG]
- onPlayEffects:
    Conditional ["if target is Vulnerable"] →
        then: DealDamage / Physical / baseValue:10 / scalingSource:BaseAttack / scalingMultiplier:1.0 / canCrit:true
        else: DealDamage / Physical / baseValue:4
- cardText: "Deal 4 damage. If target is Vulnerable, deal 10+ATK instead and can crit."

**Club Smash**  [Attack / Common / 1 NRG]  (your Smash card)
- onPlayEffects: DealDamage / Physical / baseValue:6 / scalingSource:BaseAttack / scalingMultiplier:0.75
- cardText: "Deal 6 + (ATK×0.75) physical damage."

**Dizzy Blow**  [Attack / Common / 2 NRG]  (your Blind Smash card)
- onPlayEffects:
    DealDamage / Physical / baseValue:5 / scalingSource:BaseAttack / scalingMultiplier:0.5
    ApplyVulnerable / statusDuration:2 / target:SingleEnemy
- cardText: "Deal 5+ATK damage. Apply Vulnerable 2 to target."

**Slash**  [Attack / Common / 1 NRG]
- onPlayEffects: DealDamage / Physical / baseValue:5 / canCrit:true / triggerOnHitEffects:true
- cardText: "Deal 5 physical damage. Can crit. Triggers on-hit effects."

**Poison Arrow**  [Attack / Uncommon / 2 NRG]
- onPlayEffects:
    DealDamage / Physical / baseValue:4
    ApplyPoison / statusStacks:3 / target:SingleEnemy
- cardText: "Deal 4 damage. Apply 3 Poison."

**Magic Missile**  [Attack / Common / 1 NRG]
- onPlayEffects: DealDamage / Arcane / baseValue:0 / scalingSource:MagicPower / scalingMultiplier:1.2
- cardText: "Deal (MAGIC×1.2) arcane damage."

**Arcane Surge**  [Attack / Rare / 3 NRG]
- onPlayEffects:
    DealDamage / Arcane / baseValue:0 / scalingSource:MagicPower / scalingMultiplier:2.5 / canCrit:true
    DrawCards / baseValue:1
- cardText: "Deal (MAGIC×2.5) arcane damage, can crit. Draw 1."

### Skill Cards

**Dodge**  [Skill / Common / 1 NRG]  (your existing card)
- onPlayEffects: GainArmor / baseValue:0 / scalingSource:BaseAttack / scalingMultiplier:1.0
- cardText: "Gain armor equal to your ATK."

**Parry**  [Skill / Common / 1 NRG]
- isReaction: true
- onPlayEffects: GainArmor / baseValue:5
- cardText: "Reaction: Gain 5 armor."

**Arcane Shield**  [Skill / Uncommon / 2 NRG]
- onPlayEffects: GainArmor / baseValue:0 / scalingSource:MagicPower / scalingMultiplier:1.5
- cardText: "Gain armor equal to MAGIC×1.5."

**Second Wind**  [Skill / Uncommon / 2 NRG]
- onPlayEffects:
    Heal / baseValue:0 / scalingSource:MissingHealth / scalingMultiplier:0.25
    DrawCards / baseValue:1
- cardText: "Heal 25% of missing HP. Draw 1."

**Preparation**  [Skill / Common / 0 NRG]
- onPlayEffects: DrawCards / baseValue:2
- cardText: "Draw 2 cards."

**Adrenaline**  [Skill / Rare / 0 NRG]
- onPlayEffects:
    GainEnergy / baseValue:2
    DrawCards / baseValue:2
- cardText: "Gain 2 energy. Draw 2 cards."

### Power Cards  (passive — effect fires once on play, lasts the fight)

**Burning Rage**  [Power / Uncommon / 1 NRG]
- onPlayEffects: ApplyStrength / statusStacks:2 / target:Self
- cardText: "Gain 2 Strength. (All damage +2 permanently.)"

**Toxicology**  [Power / Rare / 2 NRG]
- onPlayEffects: ApplyStrength / statusStacks:0 (custom — add a PoisonAmp status)
- cardText: "Your Poison effects apply double stacks."
  — note: implement as a custom conditional or interceptor later

---

## SKILLSETS  (Assets > Create > BattleDecks > SkillSet Data)

### Commoner  (your existing one)
- bonusAttributes: all 0 (no bonuses — baseline)
- weaponProficiencies: Axe(1), Dagger(1)  — can use basic weapons
- signatureCards: Dodge
- classCardPool: Club Smash, Second Wind, Preparation
- classDescription: "No formal training. Scrappy and adaptable."

### Rogue
- bonusAttributes: baseAttack:+2, initiative:+3
- weaponProficiencies: Dagger(3), Bow(2), Sword(1)
- signatureCards: Quick Stab, Backstab
- classCardPool: Poison Arrow, Adrenaline, Preparation
- classDescription: "Fast and evasive. Punishes Vulnerable hard."

### Wizard
- bonusAttributes: baseMagicPower:+4, maxHealth:-10, initiative:+1
- weaponProficiencies: Staff(3), Wand(2)
- signatureCards: Magic Missile, Arcane Shield
- classCardPool: Arcane Surge, Burning Rage, Preparation
- classDescription: "Fragile but devastating magic damage."

### Warrior
- bonusAttributes: baseAttack:+3, maxHealth:+10, maxArmor:+2, initiative:-1
- weaponProficiencies: Sword(3), Axe(2), Shield(2), Spear(1)
- signatureCards: Slash, Parry
- classCardPool: Second Wind, Burning Rage, Pommel Strike
- classDescription: "Tanky melee fighter. High armor, high attack."
