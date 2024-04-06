using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// View class of hand context contains all cards and hand ui elements
/// </summary>
public class HandView : MonoBehaviour
{
    public List<CardView> cardsInHand =  new List<CardView>();
    public RectTransform cardSlotsParent ;
    public List<RectTransform> cardSlots = new List<RectTransform>();
    public GameObject cardPrefabObject;


    public class CardPlayed : UnityEvent<CardModel> {}

    public CardPlayed CardPlayedEvent = new CardPlayed();

    public void SpawnCards(List<CardModel> selectedcards)
    { 
        for(int i = 0; i < selectedcards.Count; i++) // instantiate cards unitl hand limit is reached
        {
            cardSlots.Add(cardSlotsParent.GetChild(i).GetComponent<RectTransform>());


            var newCardObj = Instantiate(cardPrefabObject, cardSlots[i]); 
            var newCard = newCardObj.GetComponent<CardView>();


            cardsInHand.Add(newCard); // maintain a list of cards in hand
            newCard.CardModel = selectedcards[i]; // setup cards based on their model
        }

        InitCardEvents();
    }

    public void InitCardEvents()
    {
        foreach (var card in cardsInHand)
        {
            card.SetCardsOnGameStart();
            card.CardUsedEvent.AddListener(() => CardPlayedEvent.Invoke(card.CardModel));
        }
    }
}
