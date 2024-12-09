using System;
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
  

    public List<Turn> turns = new List<Turn>();
    [Space(20)]

    [Header("UTILs COMPONENTS")]
    [SerializeField]
    private TurnSlotController slotController;

    public class TurnsCreationCompleteEvent : UnityEvent { }
    public TurnsCreationCompleteEvent OnTurnsCreationCompleteEvent = new TurnsCreationCompleteEvent();

    private void Start()
    {
        slotController.OnNextTurnAnimationComplete.AddListener(StartTurn);
    }


    public void CreateTurnUI(TurnClass turnObjects)
    {

        Transform nextSlotParent = slotController.FindNextEmptySlot();

        Turn newTurnInstance = Instantiate(turnPrefab, nextSlotParent);
        newTurnInstance.turnObject = turnObjects;
        newTurnInstance.playerImage.color = turnObjects.color;

        turns.Add(newTurnInstance);


        if(turns.Count > 9)
        {
            OnTurnsCreationCompleteEvent.Invoke();
            OnTurnsCreationCompleteEvent.RemoveAllListeners();
        }
      

    }

    public void StartTurn()
    {
        turns[0].turnObject.StartTurn();
    }

    public void AnimNextTurn()
    {
        turns.RemoveAt(0);
        slotController.AnimateNextTurn();
    } 
}