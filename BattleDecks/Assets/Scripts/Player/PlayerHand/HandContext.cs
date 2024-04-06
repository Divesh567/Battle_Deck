using System.Collections;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Highest level class in hand context contanins all mvcs
/// </summary>
public class HandContext : TurnClass
{
    [SerializeField]
    private HandView _view;
    [SerializeField]
    private HandController _controller;
    [SerializeField]
    private HandModel _model;

    public class OnTurnStarted : UnityEvent {}
    public OnTurnStarted onTurnStartedEvent = new OnTurnStarted();


    public class CardPlayed : UnityEvent<CardModel>{}
    public CardPlayed CardPlayedEvent = new CardPlayed();



    private void Start()
    {
        _controller = new HandController();
        _controller.Initialize(_view);


        _controller.CardPlayedEvent.AddListener(CardUsed);
        onTurnStartedEvent.AddListener(() => TurnStarted());

        TurnEventSystem.AddObjectsEventCaller(this);
    }


    private void CardUsed(CardModel model)
    {
        CardPlayedEvent.Invoke(model);

        PlayerEventSystem.CardusedEventCaller(model);
    }


    private void TurnStarted()
    {
       
    }

}
