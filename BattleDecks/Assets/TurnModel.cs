using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

[System.Serializable]
public class TurnModel
{
    private List<TurnClass> objectsList = new List<TurnClass>();


    public List<TurnClass> turnsList = new List<TurnClass>();

    public class TurnsCreatedEvent : UnityEvent<List<TurnClass>> {}
    public TurnsCreatedEvent OnTurnsCreatedEvent = new TurnsCreatedEvent();


    private int _turnLimit = 10;

    public int TurnLimit { get { return _turnLimit; } set { _turnLimit = value; } }


    public void AddTurnObjects(TurnClass turnObject)
    {
        objectsList.Add(turnObject);

        if(objectsList.Count > 1)
        {
            UpdateTurnsList();
        }
    }

    private void UpdateTurnsList()
    {
        turnsList.Clear();

        for(int i = 0; i < _turnLimit; i++)
        {
            TurnClass randomObject = objectsList[Random.Range(0, objectsList.Count)];

            turnsList.Add(randomObject);
            Debug.Log(turnsList[i].baseContext.gameObject.name);
        }

        OnTurnsCreatedEvent.Invoke(turnsList);

    }

    public void CreateNewTurn()
    {
        turnsList.Clear();

        TurnClass randomObject = objectsList[Random.Range(0, objectsList.Count)];

        turnsList.Add(randomObject);

        OnTurnsCreatedEvent.Invoke(turnsList);
    }

}