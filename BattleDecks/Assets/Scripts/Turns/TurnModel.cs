using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System.Linq;
using System;


[System.Serializable]
public class TurnModel
{
    [SerializeField]
    public List<TurnClass> objectsList = new List<TurnClass>();

    [SerializeField]
    public List<TurnClass> turnsList = new List<TurnClass>();

    public class TurnsCreatedEvent : UnityEvent<List<TurnClass>> {}
    public TurnsCreatedEvent OnTurnsCreatedEvent = new TurnsCreatedEvent();

    public class TurnRemovedEvent : UnityEvent<TurnClass> { }
    public TurnRemovedEvent OnTurnRemovedEvent = new TurnRemovedEvent();

    public class TurnObjectRemovedEvent : UnityEvent<TurnClass> { }
    public TurnObjectRemovedEvent OnTurnObjectRemovedEvent = new TurnObjectRemovedEvent();


    private int _turnLimit = 6;

    public int TurnLimit { get { return _turnLimit; } set { _turnLimit = value; } }

    public void AddTurnObjects(TurnClass turnObject)
    {
        objectsList.Add(turnObject);

        if(objectsList.Count > 3)
        {
            UpdateTurnsList();
        }
    }

    private int currentTurnPlayer = 0;
    private void UpdateTurnsList()
    {
        turnsList.Clear();

        for(int i = 0; i < _turnLimit; i++)
        {

            TurnClass newTurn = objectsList[currentTurnPlayer];
            currentTurnPlayer++;
            if (currentTurnPlayer >= objectsList.Count) currentTurnPlayer = 0;

            turnsList.Add(newTurn);

            Debug.Log("Turn added" + turnsList[i].baseContext.gameObject.name);
        }

        OnTurnsCreatedEvent.Invoke(turnsList);

    }

    internal void RemoveTurnObject(TurnClass turnObject)
    {
        turnsList.RemoveAll(x => x.turnUIDetails.OwnerName == turnObject.turnUIDetails.OwnerName);
        objectsList.Remove(turnObject);

        OnTurnObjectRemovedEvent.Invoke(turnObject);
    }

    public void CreateNewTurn()
    {
        turnsList.RemoveAt(0);

        TurnClass newTurn = objectsList[currentTurnPlayer];

        currentTurnPlayer++;

        if (currentTurnPlayer >= objectsList.Count) currentTurnPlayer = 0;

        turnsList.Add(newTurn);

        List<TurnClass> newTurnList = new List<TurnClass>();
        newTurnList.Add(turnsList.Last());

        OnTurnsCreatedEvent.Invoke(newTurnList);
    }

}