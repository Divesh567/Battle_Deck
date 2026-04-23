using BattleDecks.Data;
using BattleDecks.Sim;
using UnityEngine;

/// <summary>
/// Root entry point for the sim scene.
/// Holds all SO references and owns the flow between loadout → battle panels.
/// Attach to a single root GameObject in the sim scene.
/// </summary>
public class SimUIManager : MonoBehaviour
{
    [Header("SO Libraries — drag all assets here")]
    public EntityData[]   allEntities;
    public SkillSetData[] allSkillSets;
    public WeaponData[]   allWeapons;
    public EnemyData[]    allEnemies;

    [Header("UI Panels")]
    public SimLoadoutSelector_UI loadoutPanel;
    public SimBattlePanel_UI     battlePanel;

    // ── Active session ────────────────────────────────────────────────
    public SimCombatEngine Engine { get; private set; }

    private void Start()
    {
        battlePanel.gameObject.SetActive(false);
        loadoutPanel.gameObject.SetActive(true);
        loadoutPanel.Init(allEntities, allSkillSets, allWeapons, allEnemies, OnLoadoutConfirmed);
    }

    private void OnLoadoutConfirmed(EntityData entity, SkillSetData skillSet,
                                    WeaponData weapon, EnemyData enemy)
    {
        var playerState = new SimEntityState(entity, skillSet, weapon, isPlayer: true);
        // enemies use their own base SkillSet if available — null is fine
        var enemyState  = new SimEntityState(enemy,  null,     null,   isPlayer: false);

        Engine = new SimCombatEngine(playerState, enemyState);

        loadoutPanel.gameObject.SetActive(false);
        battlePanel.gameObject.SetActive(true);
        battlePanel.Init(Engine, OnReturnToLoadout);

        Engine.StartCombat();
    }

    private void OnReturnToLoadout()
    {
        battlePanel.gameObject.SetActive(false);
        loadoutPanel.gameObject.SetActive(true);
    }
}
