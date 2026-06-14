using System;
using System.Collections.Generic;
using BattleDecks.Data;
using BattleDecks.Sim;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The main battle screen — all text-based, no sprites needed.
///
/// Hierarchy expected:
///   SimBattlePanel_UI (this)
///     ├── PlayerStatText        (TMP_Text)
///     ├── EnemyStatText         (TMP_Text)
///     ├── TurnLabel             (TMP_Text)
///     ├── HandPanel
///     │     └── [spawned SimCardButton_UI prefabs go here]
///     ├── HandCardPrefab        (assign prefab reference)
///     ├── LogScrollRect
///     │     └── Viewport > Content > LogText  (TMP_Text)
///     ├── EndTurnButton         (Button)
///     └── ReturnButton          (Button)   — back to loadout
/// </summary>
public class SimBattlePanel_UI : MonoBehaviour
{
    [Header("Stat displays")]
    public TMP_Text playerStatText;
    public TMP_Text enemyStatText;
    public TMP_Text turnLabel;

    [Header("Hand")]
    public Transform     handPanel;        // layout group parent
    public GameObject    cardButtonPrefab; // prefab with SimCardButton_UI component

    [Header("Log")]
    public TMP_Text logText;
    public ScrollRect logScrollRect;

    [Header("Buttons")]
    public Button endTurnButton;
    public Button returnButton;

    // ── Private ───────────────────────────────────────────────────────
    private SimCombatEngine _engine;
    private Action          _onReturn;
    private readonly List<string> _logLines = new();
    private readonly List<GameObject> _cardButtons = new();

    // ─────────────────────────────────────────────────────────────────
    public void Init(SimCombatEngine engine, Action onReturn)
    {
        _engine   = engine;
        _onReturn = onReturn;
        _logLines.Clear();

        _engine.OnLogLine     += AppendLog;
        _engine.OnStateChanged += RefreshAll;

        endTurnButton.onClick.AddListener(OnEndTurn);
        returnButton.onClick.AddListener(OnReturn);

        RefreshAll();
    }

    private void OnDestroy()
    {
        if (_engine == null) return;
        _engine.OnLogLine      -= AppendLog;
        _engine.OnStateChanged -= RefreshAll;
    }

    // ── Refresh ───────────────────────────────────────────────────────
    private void RefreshAll()
    {
        RefreshStats();
        RefreshHand();
        RefreshButtons();
    }

    private void RefreshStats()
    {
        var p = _engine.Player;
        var e = _engine.Enemy;

        playerStatText.text =
            $"<b>{p.Source.entityName}</b>\n" +
            $"HP {p.CurrentHP}/{p.MaxHP}\n" +
            $"ARM {p.Armor}   NRG {p.Energy}/{p.Source.core.startingEnergy}\n" +
            $"Hand:{p.Hand.Count}  Draw:{p.DrawPile.Count}  Discard:{p.DiscardPile.Count}\n" +
            StatusSummary(p);

        turnLabel.text = _engine.IsCombatOver
            ? "Combat Over"
            : $"Turn {_engine.TurnNumber} — {(_engine.IsPlayerTurn ? "YOUR TURN" : "Enemy Turn")}";

        // show enemy next intent — hidden until player uses "Read the Fight"
        string nextIntent = "";
        if (!_engine.IsCombatOver)
        {
            if (_engine.Player.HasStatus("Scouted"))
            {
                var intents = _engine.GetEnemyNextIntents();
                if (intents.Count > 0)
                {
                    var names = new List<string>();
                    foreach (var intent in intents) names.Add(intent.intentName);
                    nextIntent = $"\nNext: [{string.Join(" → ", names)}]";
                }
                else
                {
                    nextIntent = "\nNext: [—]";
                }
            }
            else
            {
                nextIntent = "\nNext: [???]";
            }
        }

        enemyStatText.text =
            $"<b>{e.Source.entityName}</b>  [{(e.Source is EnemyData ed ? ed.tier.ToString() : "?")}]\n" +
            $"HP  {e.CurrentHP}/{e.MaxHP}\n" +
            $"ARM {e.Armor}{nextIntent}\n" +
            StatusSummary(e);
    }

    private void RefreshHand()
    {
        // clear old buttons
        foreach (var btn in _cardButtons)
            Destroy(btn);
        _cardButtons.Clear();

        if (!_engine.IsPlayerTurn || _engine.IsCombatOver) return;

        foreach (var card in _engine.Player.Hand)
        {
            var go  = Instantiate(cardButtonPrefab, handPanel);
            var btn = go.GetComponent<SimCardButton_UI>();
            btn.Init(card, _engine.Player, OnCardClicked);
            _cardButtons.Add(go);
        }
    }

    private void RefreshButtons()
    {
        bool playerCanAct = _engine.IsPlayerTurn && !_engine.IsCombatOver;
        endTurnButton.interactable = playerCanAct;
    }

    // ── Handlers ──────────────────────────────────────────────────────
    private void OnCardClicked(CardData card)
    {
        _engine.PlayerPlayCard(card);
    }

    private void OnEndTurn()
    {
        _engine.PlayerEndTurn();
    }

    private void OnReturn()
    {
        _engine.OnLogLine      -= AppendLog;
        _engine.OnStateChanged -= RefreshAll;
        _onReturn?.Invoke();
    }

    // ── Log ───────────────────────────────────────────────────────────
    private void AppendLog(string line)
    {
        _logLines.Add(line);
        // keep last 120 lines to avoid the text growing unbounded
        if (_logLines.Count > 120) _logLines.RemoveAt(0);
        logText.text = string.Join("\n", _logLines);

        // scroll to bottom next frame
        Canvas.ForceUpdateCanvases();
        logScrollRect.verticalNormalizedPosition = 0f;
    }

    // ── Helpers ───────────────────────────────────────────────────────
    private static string StatusSummary(SimEntityState s)
    {
        if (s.Statuses.Count == 0) return "";
        var parts = new List<string>();
        foreach (var kv in s.Statuses)
            parts.Add($"{kv.Key}:{kv.Value}");
        return "  " + string.Join("  ", parts);
    }
}
