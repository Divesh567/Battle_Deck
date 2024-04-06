using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Highest level class for deck system in Main menu, Initializes Controller and has inspector reference to all views 
/// </summary>

public class DeckContext : MonoBehaviour
{
    [SerializeField]
    private DeckView _view;
    private DeckController controller;
    private DeckModel model;


    private void OnEnable()
    {
        InitalizeController();


        EventManager.CardSelectedEvent += controller.AddCardToDeck;
        EventManager.OnGameStartButtonClickEvent += controller.SaveSelectedCards;
    }

    private void OnDisable()
    {
        EventManager.CardSelectedEvent -= controller.AddCardToDeck;
        EventManager.OnGameStartButtonClickEvent -= controller.SaveSelectedCards;
    }

    
    private void InitalizeController()
    {
        controller = new DeckController();
        model = new DeckModel();
        controller.InitalizeController(_view, model);


        model.DeckLimitReachedEvent.AddListener(() => OnDeckChanged(true));
        model.DeckLimitNotRecheadEvent.AddListener(() => OnDeckChanged(false));
    }

    //Update this context and other contexts about deckfull when card is added or removed
    private void OnDeckChanged(bool isLimitReached)
    {
        EventManager.CardLimtReachedEventCaller(isLimitReached);
        controller.limitReached = isLimitReached;
    }
}
