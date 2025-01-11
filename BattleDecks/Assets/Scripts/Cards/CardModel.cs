using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Card Model", menuName = "Cards/Card", order = 1)]
public class CardModel : ScriptableObject
{
    public CardUIDetails cardUIDetails; 

    [SerializeField]
    private List<CardEffect> _cardEffects;

    public List<CardEffect> cardEffects { get { return _cardEffects; } }
}

[Serializable]
public class CardUIDetails
{
    [SerializeField]
    private string cardName;
    public string CardName { get { return cardName; } }

    public Sprite baseArt;
    public Sprite mainArt;
    public string cardInfo;
}

