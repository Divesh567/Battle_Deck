using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class TurnController
{
    private TurnModel _model;
    private TurnView _view;


 

    public void Initialzie(TurnModel turnModel, TurnView turnView)
    {
        _model = turnModel;
        _view = turnView;

        _view.OnTurnsCreationCompleteEvent.AddListener(StartTurns);
    }

    public void AddTurnObject(TurnClass turnObject)
    {
        _model.AddTurnObjects(turnObject);
        _view.CreateTurnUI(turnObject);
    }


    private void StartTurns()
    {
        EnemyLogger.instance.Showlog("Turn 1 Started");

    }
}