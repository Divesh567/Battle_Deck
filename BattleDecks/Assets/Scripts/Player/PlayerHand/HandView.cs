using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// View class of hand context contains all cards and hand ui elements
/// </summary>
public class HandView : MonoBehaviour
{
    #region Events
    public class CardPlayed : UnityEvent { }
    public CardPlayed CardPlayedEvent = new CardPlayed();

    public class CardSelected : UnityEvent<CardModel> { }
    public CardSelected CardSelectedEvent = new CardSelected();

    public class CardUnSelected : UnityEvent<CardModel> { }
    public CardUnSelected CardUnSelectedEvent = new CardUnSelected();

    public class TurnEndClickedEvent : UnityEvent { }
    public TurnEndClickedEvent OnTurnEndClickedEvent = new TurnEndClickedEvent();



    #endregion


    #region ANIMATIONS
     

    #endregion

    [Header("UI ELEMENTS")]
    public List<CardView> cardsInHand = new List<CardView>();
    public RectTransform cardSlotsParent;
    public List<RectTransform> cardSlots = new List<RectTransform>();
    public GameObject cardPrefabObject;
    [SerializeField]
    private Button endTurnButton;



    private CardView selectedCard = new CardView();

    private void Start()
    {
        endTurnButton.onClick.AddListener(OnTurnEnded);
    }

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
    }

    public void InitCardEvents()
    {
        foreach (var card in cardsInHand)
        {
            card.CardUnSelectedEvent.RemoveAllListeners();
            card.CardSelectedEvent.RemoveAllListeners();
         
            card.CardSelectedEvent.AddListener(() => OnCardSelected(card));
            card.CardUnSelectedEvent.AddListener(() => OnCardUnselected(card));
        }

        EnableHand();
    }


    private void OnCardSelected(CardView selected)
    {

        if (selectedCard != null)
        {
            Debug.Log("Unselected card" + selectedCard.cardModel.cardName);
            selectedCard.CardUnSelectedEvent.Invoke();
        }

        if (selectedCard == selected)
        {
            Debug.Log("Unselected card" + selectedCard.cardModel.cardName);
            selectedCard = null;
            return;
        }

        selectedCard = selected;

        Debug.Log("Selected card" + selectedCard.cardModel.cardName);
        CardSelectedEvent.Invoke(selectedCard.cardModel);

        selected.selectionAnimation.OnPlayAnimationEvent?.Invoke();
    }

    private void OnCardUnselected(CardView unselected)
    {
        CardUnSelectedEvent.Invoke(unselected.cardModel);

        unselected.selectionAnimation.OnStopAnimationEvent?.Invoke();
    }

    public void OnCardPlayed()
    {
        foreach (var card in cardsInHand)
        {
            if(card == selectedCard)
            {
                cardsInHand.Remove(card);
                break;
            }
        }

        selectedCard.CardUsedEvent.Invoke();
    }

    public void OnTurnEnded()
    {
        DisableHand();
        OnTurnEndClickedEvent.Invoke();
    }

    private void DisableHand()
    {
        cardsInHand.ForEach(x => x.selectButton.interactable = false);
        endTurnButton.interactable = false;
    }

    private void EnableHand()
    {
        cardsInHand.ForEach(x => x.selectButton.interactable = true);
        endTurnButton.interactable = true;
    }
}
