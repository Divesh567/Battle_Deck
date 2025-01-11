using System;
using System.Collections.Generic;
using UnityEngine.Events;

[Serializable]
public class TurnController
{
    private TurnModel _model;
    private TurnView _view;


    public void Initialzie(TurnModel turnModel, TurnView turnView)
    {
        _model = turnModel;
        _view = turnView;


        _view.Init(_model);

        _view.OnTurnsCreationCompleteEvent.AddListener(StartTurns);


        _model.OnTurnsCreatedEvent.AddListener(OnTurnsCreated);

        _model.OnTurnObjectRemovedEvent.AddListener(OnTurnObjectRemoved);
    }

    public void AddTurnObject(TurnClass turnObject)
    {
        _model.AddTurnObjects(turnObject);
    }


    public void OnTurnsCreated(List<TurnClass> turnClasses)
    {
        _view.CreateTurnUI(turnClasses);
    }

    private void StartTurns()
    {
        _view.StartTurn();
    }

    internal void GoToNextTurn()
    {
        _view.AnimNextTurn();

        _model.CreateNewTurn();
    }

    internal void RemoveTurnObject(TurnClass turnObject)
    {
        _model.RemoveTurnObject(turnObject);
    }
    public void OnTurnObjectRemoved(TurnClass turnClass)
    {
        _view.RemoveTurnFromUI(turnClass);

    }
}