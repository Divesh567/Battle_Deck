using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;

public class TurnView : MonoBehaviour
{
    [SerializeField]
    private Turn turnPrefab;
    [SerializeField]
    private Image turnPanel;


    private TurnModel model = new TurnModel();
    public List<Turn> turns = new List<Turn>();
    [Space(20)]

    [Header("UTILs COMPONENTS")]
    [SerializeField]
    private TurnSlotController slotController;

    public class TurnsCreationCompleteEvent : UnityEvent { }
    public TurnsCreationCompleteEvent OnTurnsCreationCompleteEvent = new TurnsCreationCompleteEvent();

    public void Init(TurnModel model)
    {
        this.model = model;
        slotController.OnNextTurnAnimationComplete.AddListener(StartTurn);
    }


    public void CreateTurnUI(List<TurnClass> turns)
    {
        for(int i = 0; i < turns.Count; i++)
        {
            Transform nextSlotParent = slotController.FindNextEmptySlot();

            Turn newTurnInstance = Instantiate(turnPrefab, nextSlotParent);
            newTurnInstance.turnObject = turns[i];
            newTurnInstance.name = turns[i].turnUIDetails.OwnerName;
            newTurnInstance.playerImage.sprite = turns[i].turnUIDetails.OwnerSprite;
            newTurnInstance.playerImage.name = turns[i].turnUIDetails.OwnerName;
            newTurnInstance.playerName.text = turns[i].turnUIDetails.OwnerName;

            this.turns.Add(newTurnInstance);

        }

        OnTurnsCreationCompleteEvent.Invoke();
        OnTurnsCreationCompleteEvent.RemoveAllListeners();

    }

    public void RemoveTurnFromUI(TurnClass turnClass)
    {
        var turnsToDestroy = turns.FindAll(x => x.playerName.text == turnClass.turnUIDetails.OwnerName).ToList();
        turns.RemoveAll(x => x.playerName.text == turnClass.turnUIDetails.OwnerName);
        StartCoroutine(RemoveTurnsCouroutine(turnsToDestroy));
    }
    IEnumerator RemoveTurnsCouroutine(List<Turn> turnsToDestroy)
    {
        turnsToDestroy.ForEach(x => Destroy(x.gameObject));

        yield return new WaitForEndOfFrame();

        slotController.ReorganizeTurns();
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