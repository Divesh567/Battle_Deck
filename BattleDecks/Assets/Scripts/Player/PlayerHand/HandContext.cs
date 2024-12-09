using System.Collections;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Highest level class in hand context contanins all mvcs
/// </summary>
public class HandContext : BaseContext
{
    [SerializeField]
    private HandView _view;
    [SerializeField]
    private HandController _controller;
    [SerializeField]
    private HandModel _model;


    private TurnClass _turnClass;

    public class TurnStarted : UnityEvent {}
    public TurnStarted OnTurnStartedEvent = new TurnStarted();


    private void Start()
    {
        InitializeController();
        RegisterEvents();

        _turnClass = new TurnClass();
        _turnClass.baseContext = this;
        _turnClass.color = Color.blue;
        _turnClass.RegiterObjectForTurn();

    }

    private void RegisterEvents()
    {
        _controller.CardSelectedEvent.AddListener(OnCardSelected);
        _controller.CardUnSelectedEvent.AddListener(OnCardUnSelected);
        OnTurnStartedEvent.AddListener(OnTurnStarted);

        PlayerEventSystem.CardUsedEvent += OnCardUsed;
    }

    private void InitializeController()
    {
        _controller = new HandController();
        _controller.Initialize(_view);
    }


    private void OnCardSelected(CardModel model)
    {
        PlayerEventSystem.CardSelectedEventCaller(model);
    }

    private void OnCardUnSelected(CardModel model)
    {

    }

    private void OnCardUsed()
    {
        _controller.CardPlayedEvent.Invoke();
    }


    private void OnTurnStarted()
    {
        _controller.OnTurnStartedEvent?.Invoke();
    }

}
