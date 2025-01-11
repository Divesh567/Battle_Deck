using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EnemyContext : BaseContext , ICardAffectable
{
    public EnemyModel enemyModel;
    public EnemyController enemyController;
    public EnemyView enemyView;

    private TurnClass turnClass;
    public Behaviour behaviour;

    public class OnTurnStarted : UnityEvent { }
    public OnTurnStarted onTurnStartedEvent = new OnTurnStarted();

 

    private void Start()
    {
        enemyController = new EnemyController();
        enemyController.Init(enemyModel, enemyView, behaviour);

        enemyController.TargetSelectedEvent.AddListener(OnTargetSelected);
        enemyController.EnemyDeadEvent.AddListener(RemoveTurns);
        enemyController.OnTurnEndedEvent.AddListener(EndTurn);

        onTurnStartedEvent.AddListener(() => TurnStarted());

        turnClass = new TurnClass(this, enemyModel.enemyTurnUIDetails);

    }

    public void OnTargetSelected ()
    {
        PlayerEventSystem.CardUsedEventEventCaller();
    }
    public void ApplyEffectToSelf(CardEffect model)
    {
        enemyController.OnEffectApplied(model);
    }

    private void TurnStarted()
    {
        enemyController.onTurnStartedEvent?.Invoke();
    }

    private void EndTurn()
    {
        TurnEventSystem.NextTurnEventCaller();
    }

    private void RemoveTurns()
    {
        TurnEventSystem.RemoveObjectsEventCaller(turnClass);
    }

    public void EnableSelector()
    {
        enemyController.EnableSelector();
    }

}
