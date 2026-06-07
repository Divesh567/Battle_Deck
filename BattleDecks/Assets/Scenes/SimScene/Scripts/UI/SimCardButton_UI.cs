using System;
using BattleDecks.Data;
using BattleDecks.Sim;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// One card in the player's hand.
///
/// Prefab hierarchy (each label is optional — missing labels are skipped):
///   CardButton (Button + SimCardButton_UI)
///     ├── NameLabel      → cardLabel
///     ├── CostLabel      → nrg_costLabel
///     ├── ValueLabel     → mainValueLabel
///     ├── ChanceLabel    → chanceLabel
///     └── TypeLabel      → infoLabel
/// </summary>
public class SimCardButton_UI : MonoBehaviour
{
    [Header("Labels")]
    public TMP_Text cardLabel;      // card name
    public TMP_Text nrg_costLabel;  // energy cost
    public TMP_Text mainValueLabel; // primary effect value
    public TMP_Text chanceLabel;    // hit chance (hidden when 100%)
    public TMP_Text infoLabel;      // card type · damage type

    public Button button;

    private CardData _card;
    private Action<CardData> _onClick;

    public void Init(CardData card, SimEntityState playerState, Action<CardData> onClick)
    {
        _card    = card;
        _onClick = onClick;

        bool canAfford = playerState.Energy >= card.energyCost;

        SetLabel(cardLabel,     card.cardName);
        SetLabel(nrg_costLabel, $"{card.energyCost} NRG");
        SetLabel(infoLabel,     BuildTypeInfo(card));
        SetMainValue(card);
        SetChance(card);

        button.interactable = canAfford;

        Color tint = canAfford ? Color.black : new Color(0.5f, 0.5f, 0.5f);
        TintAll(tint);

        button.onClick.AddListener(OnClick);
    }

    private void SetMainValue(CardData card)
    {
        if (mainValueLabel == null) return;
        var fx = card.onPlayEffects is { Length: > 0 } ? card.onPlayEffects[0] : null;
        mainValueLabel.text = fx != null ? fx.baseValue.ToString() : "";
    }

    private void SetChance(CardData card)
    {
        if (chanceLabel == null) return;
        var fx = card.onPlayEffects is { Length: > 0 } ? card.onPlayEffects[0] : null;
        if (fx == null || fx.missChance <= 0f)
        {
            chanceLabel.text = "";
            return;
        }
        int hit = 100 - Mathf.RoundToInt(fx.missChance * 100f);
        chanceLabel.text = $"{hit}% hit";
    }

    private static string BuildTypeInfo(CardData card)
    {
        return card.damageType != DamageType.Physical
            ? $"{card.cardType} · {card.damageType}"
            : card.cardType.ToString();
    }

    private void TintAll(Color color)
    {
        SetTint(cardLabel,      color);
        SetTint(nrg_costLabel,  color);
        SetTint(mainValueLabel, color);
        SetTint(chanceLabel,    color);
        SetTint(infoLabel,      color);
    }

    private static void SetLabel(TMP_Text label, string text)
    {
        if (label != null) label.text = text;
    }

    private static void SetTint(TMP_Text label, Color color)
    {
        if (label != null) label.color = color;
    }

    private void OnClick() => _onClick?.Invoke(_card);
}
