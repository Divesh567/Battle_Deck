using DG.Tweening;
using System.Collections;
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
    public CanvasGroup cardsParentCanvasGroup;

    private List<RectTransform> cardSlots = new List<RectTransform>();
    public List<RectTransform> CardSlots { get { return cardSlots; } }


    public GameObject cardPrefabObject;

    [SerializeField]
    private Button endTurnButton;



    private CardView selectedCard = new CardView();

    private int handLimit = 3;
    public int HandLimit { get { return handLimit; } }


    public InventoryController inventoryController;
    public Transform inventoryParent;

    private void Start()
    {
        endTurnButton.onClick.AddListener(OnTurnEnded);
        DisableHand();
    }

    public void SpawnCards(List<CardModel> selectedcards)
    { 
        for (int i = 0; i < selectedcards.Count; i++) // instantiate cards unitl hand limit is reached
        {
            var newCardObj = Instantiate(cardPrefabObject, inventoryParent);
            var newCard = newCardObj.GetComponent<CardView>();
            cardsInHand.Add(newCard); // maintain a list of cards in hand
            newCard.CardModel = selectedcards[i]; // setup cards based on their model

            inventoryController.AddCardToInventory(newCard);

        }

        for (int i = 0; i < handLimit; i++)
        {
            AddNewCard();
        }
    }

    public void  AddNewCard()
    {
        if (cardSlotsParent.childCount >= handLimit) return;

        Debug.Log("HAND ADDED NEW SLOT");
        int newSlotIndex = CreateNewCardSlot();

        GetAndAddCardFromInventory(newSlotIndex);
    }

    public int CreateNewCardSlot()
    {
        var newSlot = Instantiate(new GameObject("slot444234234"), cardSlotsParent);
        int newSlotIndex = cardSlotsParent.childCount - 1;

        newSlot.AddComponent<RectTransform>();
        cardSlots.Add(cardSlotsParent.GetChild(newSlotIndex).GetComponent<RectTransform>());

        return newSlotIndex;
    }

    public void GetAndAddCardFromInventory(int newParentIndex)
    {
        CardView drawCard = inventoryController.allCards[0];

        drawCard.transform.SetParent(cardSlotsParent.GetChild(newParentIndex));

        drawCard.rectTransform.anchoredPosition = Vector2.zero;

        inventoryController.RemoveCardFromInventory(0);

        FanLayoutGroup.OnGroupChangedEvent.Invoke();
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
            Debug.Log("Unselected card" + selectedCard.cardModel.cardUIDetails.CardName);
            selectedCard.CardUnSelectedEvent.Invoke();
        }

        if (selectedCard == selected)
        {
            Debug.Log("Unselected card" + selectedCard.cardModel.cardUIDetails.CardName);
            selectedCard = null;
            return;
        }

        selectedCard = selected;

        Debug.Log("Selected card" + selectedCard.cardModel.cardUIDetails.CardName);
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
                inventoryController.AddCardToInventory(card);
                Transform cardSlot = card.transform.parent;

                card.transform.SetParent(inventoryParent);
                StartCoroutine(DestroyAndUpdateCoroutine(cardSlot.gameObject));
                cardsInHand.Remove(card);
                break;
            }
        }

        selectedCard.CardUsedEvent.Invoke();
       
    }

    private IEnumerator DestroyAndUpdateCoroutine(GameObject childToDestroy)
    {
        if (childToDestroy != null)
        {
            Destroy(childToDestroy); // Mark for destruction
            yield return new WaitForEndOfFrame(); // Wait until end of frame
            FanLayoutGroup.OnGroupChangedEvent.Invoke();// Update the list after destruction
        }
    }

    public void OnTurnEnded()
    {
        DisableHand();
        OnTurnEndClickedEvent.Invoke();
    }

    private void DisableHand()
    {
        cardsParentCanvasGroup.blocksRaycasts = false;
        cardsParentCanvasGroup.alpha = 0.5f;

        endTurnButton.interactable = false;
    }

    private void EnableHand()
    {
        cardsParentCanvasGroup.alpha = 1f;
        endTurnButton.interactable = true;
        cardsParentCanvasGroup.blocksRaycasts = true;

        AddNewCard();
    }
}
