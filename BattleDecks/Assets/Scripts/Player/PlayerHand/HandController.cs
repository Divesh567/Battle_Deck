using System;
using UnityEngine.Events;
/// <summary>
/// Controller class for hand context controls flow of hand in game
/// </summary>
public class HandController
{
    private HandModel _model;
    private HandView _view;


    public class CardPlayed : UnityEvent{}
    public CardPlayed CardPlayedEvent = new CardPlayed();

    public class CardSelected : UnityEvent<CardModel> { }
    public CardSelected CardSelectedEvent = new CardSelected();

    public class CardUnSelected : UnityEvent<CardModel> { }
    public CardUnSelected CardUnSelectedEvent = new CardUnSelected();

    public class TurnStarted : UnityEvent { }
    public TurnStarted OnTurnStartedEvent = new TurnStarted();

    public void Initialize(HandView view)
    {
        _view = view;
        RegisterViewEvents();
        RegisterInternalEvents();
        SpawnCards();
    }

    private void RegisterViewEvents()
    {
        _view.CardSelectedEvent.AddListener(OnCardSelected);
        _view.CardUnSelectedEvent.AddListener(OnCardUnSelected);
        _view.OnTurnEndClickedEvent.AddListener(EndTurn);
    }

    private void RegisterInternalEvents()
    {
        OnTurnStartedEvent.AddListener(OnTurnStarted);
        CardPlayedEvent.AddListener(OnCardPlayed);
    }


    private void EndTurn()
    {
        TurnEventSystem.NextTurnEventCaller();  
    }

    public void SpawnCards()
    {
        var cardList = PersistentData.selectedCards;
        _view.SpawnCards(cardList);
    }

    private void OnCardSelected(CardModel model)
    {
        CardSelectedEvent.Invoke(model);
    }

    private void OnCardUnSelected(CardModel model)
    {
        CardUnSelectedEvent.Invoke(model);
    }

    private void OnCardPlayed()
    {

        _view.OnCardPlayed();
    }

    private void OnTurnStarted()
    {
        _view.InitCardEvents();
    }
}
