using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
public class TurnModel
{
    private List<TurnClass> turnsList = new List<TurnClass>();

    public class TurnsCreatedEvent : UnityEvent<List<TurnClass>> { }
    public TurnsCreatedEvent OnTurnsCreatedEvent = new TurnsCreatedEvent();
    public void AddTurnObjects(TurnClass turnObject)
    {
        turnsList.Add(turnObject);

        OnTurnsCreatedEvent.Invoke(turnsList);

    }

}