using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TurnView : MonoBehaviour
{
    [SerializeField]
    private Turn turnPrefab;
    [SerializeField]
    private Image turnPanel;

    public List<Turn> turns;

    public class TurnsCreationCompleteEvent : UnityEvent { }
    public TurnsCreationCompleteEvent OnTurnsCreationCompleteEvent = new TurnsCreationCompleteEvent();

    public void CreateTurnUI(TurnClass turnObjects)
    {
        Turn newTurnInstance = Instantiate(turnPrefab, turnPanel.transform);

        newTurnInstance.turnObject = turnObjects;

        OnTurnsCreationCompleteEvent.Invoke();



    } 
}