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



        _view.OnTurnsCreationCompleteEvent.AddListener(StartTurns);
        _model.OnTurnsCreatedEvent.AddListener(OnTurnsCreated);
    }

    public void AddTurnObject(TurnClass turnObject)
    {
        _model.AddTurnObjects(turnObject);
       
    }

    public void OnTurnsCreated(List<TurnClass> turnClasses)
    {
        turnClasses.ForEach(x => _view.CreateTurnUI(x));
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
}