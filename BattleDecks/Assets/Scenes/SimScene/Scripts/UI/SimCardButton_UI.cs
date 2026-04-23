using System;
using BattleDecks.Data;
using BattleDecks.Sim;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// One card in the player's hand.
/// Attach to a Button prefab with a TMP_Text child.
///
/// Prefab hierarchy:
///   CardButton (Button + SimCardButton_UI)
///     └── CardLabel (TMP_Text)  — assigned to cardLabel
/// </summary>
public class SimCardButton_UI : MonoBehaviour
{
    public TMP_Text cardLabel;
    public Button   button;

    private CardData _card;
    private Action<CardData> _onClick;

    public void Init(CardData card, SimEntityState playerState, Action<CardData> onClick)
    {
        _card    = card;
        _onClick = onClick;

        bool canAfford = playerState.Energy >= card.energyCost;

        // multi-line card face: name, cost, type, text
        cardLabel.text =
            $"<b>{card.cardName}</b>  [{card.energyCost}NRG]\n" +
            $"<size=85%>{card.cardType}  {(card.damageType != DamageType.Physical ? card.damageType.ToString() : "")}</size>\n" +
            $"<size=80%>{card.cardText}</size>";

        button.interactable = canAfford;

        // grey out text when unaffordable
        cardLabel.color = canAfford ? Color.white : new Color(0.5f, 0.5f, 0.5f);

        button.onClick.AddListener(OnClick);
    }

    private void OnClick() => _onClick?.Invoke(_card);
}
