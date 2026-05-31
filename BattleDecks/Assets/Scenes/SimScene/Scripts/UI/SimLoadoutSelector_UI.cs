using System;
using System.Collections.Generic;
using BattleDecks.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Loadout selection screen.
///
/// Hierarchy expected:
///   SimLoadoutSelector_UI (this)
///     ├── EntityDropdown        (TMP_Dropdown)
///     ├── SkillSetDropdown      (TMP_Dropdown)  — filtered by entity's allowed classes
///     ├── WeaponDropdown        (TMP_Dropdown)  — filtered by skillset proficiencies
///     ├── EnemyDropdown         (TMP_Dropdown)
///     ├── LoadoutInfoPanel
///     │     ├── EntityInfoText  (TMP_Text)
///     │     ├── SkillInfoText   (TMP_Text)
///     │     └── WeaponInfoText  (TMP_Text)
///     └── StartBattleButton     (Button)
/// </summary>
public class SimLoadoutSelector_UI : MonoBehaviour
{
    [Header("Dropdowns")]
    public TMP_Dropdown entityDropdown;
    public TMP_Dropdown skillSetDropdown;
    public TMP_Dropdown weaponDropdown;
    public TMP_Dropdown enemyDropdown;

    [Header("Info Labels")]
    public TMP_Text entityInfoText;
    public TMP_Text skillInfoText;
    public TMP_Text weaponInfoText;
    public TMP_Text enemyInfoText;

    [Header("Actions")]
    public Button startBattleButton;

    // ── Private state ─────────────────────────────────────────────────
    private EntityData[]   _entities;
    private SkillSetData[] _allSkillSets;
    private WeaponData[]   _weapons;
    private EnemyData[]    _enemies;

    private SkillSetData[] _filteredSkillSets;
    private WeaponData[]   _filteredWeapons;

    private Action<EntityData, SkillSetData, WeaponData, EnemyData> _onConfirm;

    // ─────────────────────────────────────────────────────────────────
    public void Init(EntityData[] entities, SkillSetData[] skillSets,
                     WeaponData[] weapons,  EnemyData[]   enemies,
                     Action<EntityData, SkillSetData, WeaponData, EnemyData> onConfirm)
    {
        _entities     = entities;
        _allSkillSets = skillSets;
        _weapons      = weapons;
        _enemies      = enemies;
        _onConfirm    = onConfirm;

        entityDropdown.onValueChanged.AddListener(OnEntityChanged);
        skillSetDropdown.onValueChanged.AddListener(OnSkillSetChanged);
        weaponDropdown.onValueChanged.AddListener(OnWeaponChanged);
        enemyDropdown.onValueChanged.AddListener(OnEnemyChanged);
        startBattleButton.onClick.AddListener(OnStartBattle);

        PopulateEntityDropdown();
        PopulateEnemyDropdown();
    }

    // ── Population ────────────────────────────────────────────────────
    private void PopulateEntityDropdown()
    {
        entityDropdown.ClearOptions();
        var options = new List<string>();
        foreach (var e in _entities) options.Add(e.entityName);
        entityDropdown.AddOptions(options);
        OnEntityChanged(0);
    }

    private void PopulateSkillSetDropdown(EntityData entity)
    {
        skillSetDropdown.ClearOptions();

        // if entity is a PlayerData check for allowed classes on each skillset
        var available = new List<SkillSetData>();
        foreach (var ss in _allSkillSets)
        {
            // SkillSetData has no restriction fields — all skillsets available for now
            // extend here when you add per-class entity restrictions
            available.Add(ss);
        }

        _filteredSkillSets = available.ToArray();
        var options = new List<string>();
        foreach (var ss in _filteredSkillSets)
            options.Add(ss.className);

        skillSetDropdown.AddOptions(options);
        OnSkillSetChanged(0);
    }

    private void PopulateWeaponDropdown(SkillSetData skillSet)
    {
        weaponDropdown.ClearOptions();

        var available = new List<WeaponData>();
        foreach (var w in _weapons)
        {
            // filter by proficiency: only show weapons the class has at least level 1 in
            bool hasProficiency = true;
            if (skillSet?.weaponProficiencies != null)
            {
                foreach (var prof in skillSet.weaponProficiencies)
                {
                    if (prof.category == w.category && prof.level >= w.requiredProficiencyLevel)
                    {
                        hasProficiency = true;
                        break;
                    }
                }
            }
            // if no proficiencies defined, show all weapons
            if (skillSet?.weaponProficiencies == null ||
                skillSet.weaponProficiencies.Length == 0 ||
                hasProficiency)
                available.Add(w);
        }

        _filteredWeapons = available.ToArray();
        var options = new List<string>();
        foreach (var w in _filteredWeapons) options.Add(w.weaponName);
        weaponDropdown.AddOptions(options);
        OnWeaponChanged(0);
    }

    private void PopulateEnemyDropdown()
    {
        enemyDropdown.ClearOptions();
        var options = new List<string>();
        foreach (var e in _enemies)
            options.Add($"[{e.tier}] {e.entityName}");
        enemyDropdown.AddOptions(options);
        OnEnemyChanged(0);
    }

    // ── Change handlers ───────────────────────────────────────────────
    private void OnEntityChanged(int index)
    {
        if (_entities == null || index >= _entities.Length) return;
        var entity = _entities[index];

        var core = entity.core;
        entityInfoText.text = entity.entityName;
            /*
            $"<b>{entity.entityName}</b>\n" +
            $"HP: {core.maxHealth}  ATK: {core.baseAttack}  MGK: {core.baseMagicPower}\n" +
            $"ARM: {core.maxArmor}  NRG: {core.startingEnergy}  INIT: {core.initiative}";
            */

        PopulateSkillSetDropdown(entity);
        RefreshStartButton();
    }

    private void OnSkillSetChanged(int index)
    {
        if (_filteredSkillSets == null || index >= _filteredSkillSets.Length) return;
        var ss = _filteredSkillSets[index];

        var b = ss.bonusAttributes;
        skillInfoText.text = ss.className;
            /*$"<b>{ss.className}</b>\n" +
            $"{ss.classDescription}\n" +
            $"Bonuses — HP:+{b.maxHealth} ATK:+{b.baseAttack} MGK:+{b.baseMagicPower}\n" +
            $"Weapon skills: {WeaponProfSummary(ss)}";*/

        PopulateWeaponDropdown(ss);
        RefreshStartButton();
    }

    private void OnWeaponChanged(int index)
    {
        if (_filteredWeapons == null || index >= _filteredWeapons.Length)
        {
            weaponInfoText.text = "No weapon";
            return;
        }
        var w = _filteredWeapons[index];
        weaponInfoText.text = w.weaponName;
            /*$"<b>{w.weaponName}</b>  [{w.category}]\n" +
            $"Dmg: {w.baseDamageMin}–{w.baseDamageMax}  ATK+{w.baseAttackBonus}\n" +
            $"Grants {w.grantedCards?.Length ?? 0} card(s)" +
            (w.grantedCards?.Length > 0 ? $": {CardNames(w.grantedCards)}" : "");*/

        RefreshStartButton();
    }

    private void OnEnemyChanged(int index)
    {
        if (_enemies == null || index >= _enemies.Length) return;
        var e = _enemies[index];
        var core = e.core;
        enemyInfoText.text = e.entityName;
            /*$"<b>{e.entityName}</b>  [{e.tier}]\n" +
            $"{e.description}\n" +
            $"HP: {core.maxHealth}  ATK: {core.baseAttack}  INIT: {core.initiative}\n" +
            $"Intents: {e.intentPattern?.Length ?? 0}";*/

        RefreshStartButton();
    }

    // ── Start ─────────────────────────────────────────────────────────
    private void OnStartBattle()
    {
        startBattleButton.gameObject.SetActive(false);
        
        var entity   = _entities[entityDropdown.value];
        var skillSet = _filteredSkillSets.Length > 0
                       ? _filteredSkillSets[skillSetDropdown.value] : null;
        var weapon   = _filteredWeapons.Length > 0
                       ? _filteredWeapons[weaponDropdown.value] : null;
        var enemy    = _enemies[enemyDropdown.value];

        _onConfirm?.Invoke(entity, skillSet, weapon, enemy);
    }

    // ── Helpers ───────────────────────────────────────────────────────
    private void RefreshStartButton()
    {
        startBattleButton.interactable =
            _entities?.Length > 0 && _enemies?.Length > 0;
    }

    private string WeaponProfSummary(SkillSetData ss)
    {
        if (ss.weaponProficiencies == null || ss.weaponProficiencies.Length == 0)
            return "none";
        var parts = new List<string>();
        foreach (var p in ss.weaponProficiencies)
            parts.Add($"{p.category}({p.level})");
        return string.Join(", ", parts);
    }

    private string CardNames(CardData[] cards)
    {
        var names = new List<string>();
        foreach (var c in cards) names.Add(c.cardName);
        return string.Join(", ", names);
    }
}
