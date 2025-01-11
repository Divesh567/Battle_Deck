using System.Collections.Generic;
using UnityEngine.Events;
/// <summary>
/// Second highest level class for deck system in Main menu, Controls flow between deck view and deck model
/// </summary>
public class DeckController
{
    
    private DeckView _view;
    private DeckModel _model;

    public bool limitReached = false;

    public void InitalizeController(DeckView view, DeckModel model)
    {
        _view = view;
        _model = model;

        _view.InitDeckView(_model);
    }


    public void AddCardToDeck(CardView cardView)
    {
        if (limitReached) return; 

        //Update card button event to remove function
        cardView.CardUnSelectedEvent.RemoveAllListeners();
        cardView.CardUnSelectedEvent.AddListener(() => RemoveCardFromDeck(cardView));
        cardView.UpdateButtonOnCardSelected();

        //Update view and model
        _view.AddCardToDeck(cardView);
        _model.OnDeckSizeChanged(1);
    }

    public void RemoveCardFromDeck(CardView cardView)
    {
        EventManager.CardUnSelectedEventCaller(cardView);
        //Update view and model
        _view.RemoveCard(cardView);
        _model.OnDeckSizeChanged(-1);
    }

    public void SaveSelectedCards()
    {
        PersistentData.selectedCards = new List<CardModel>();
        foreach (CardView view in _view.cards)
        {
            PersistentData.selectedCards.Add(view.CardModel);
        }
    }


}
