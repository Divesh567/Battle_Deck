using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card Model", menuName = "Cards/Card", order = 1)]
public class CardModel : ScriptableObject
{
    [SerializeField]
    private string _cardName;
    public string cardName { get { return _cardName; } }

    [SerializeField]
    private int _cardIndex;
    public int cardIndex { get { return _cardIndex; } }



    [SerializeField]
    private List<CardEffect> _cardEffects;

    public List<CardEffect> cardEffects { get { return _cardEffects; } }
}

