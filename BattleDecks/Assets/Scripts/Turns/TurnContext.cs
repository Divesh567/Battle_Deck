using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnContext : MonoBehaviour
{

    private TurnModel turnModel = new TurnModel();
    private TurnController turnController;
    [SerializeField]
    private TurnView turnView;


    private void OnEnable()
    {
        TurnEventSystem.AddObjectsEvent += AddTurnObjects;
        TurnEventSystem.RemoveObjectsEvent += RemoveTurnObject;
        TurnEventSystem.NextTurnEvent += GoToNextTurn;
    }

    private void OnDisable()
    {
        TurnEventSystem.AddObjectsEvent -= AddTurnObjects;
        TurnEventSystem.RemoveObjectsEvent -= RemoveTurnObject;
        TurnEventSystem.NextTurnEvent -= GoToNextTurn;
    }

    private void Awake()
    {
        turnController = new TurnController();
        turnController.Initialzie(turnModel, turnView);
    }

    private void AddTurnObjects(TurnClass turnObject)
    {
        turnController.AddTurnObject(turnObject);
    }

    private void RemoveTurnObject(TurnClass turnObject)
    {
        turnController.RemoveTurnObject(turnObject);
    }

    private void GoToNextTurn()
    {
        turnController.GoToNextTurn();
    }

   
}
