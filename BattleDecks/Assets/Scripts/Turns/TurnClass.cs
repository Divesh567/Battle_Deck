using UnityEngine;



[System.Serializable]
public class TurnClass : ITurnObject
{
    private BaseContext _baseContext;
    public TurnUIDetails turnUIDetails;

    public BaseContext baseContext { get { return _baseContext; } set { _baseContext = value; } }

    public TurnClass(BaseContext baseContext, TurnUIDetails turnUIDetails)
    {
        _baseContext = baseContext;
        this.turnUIDetails = turnUIDetails;

        RegiterObjectForTurn();
    }


    public void RegiterObjectForTurn()
    {
        TurnEventSystem.AddObjectsEventCaller(this);
    }

    public void StartTurn()
    {
        HandContext handContext = _baseContext as HandContext;

        PlayerEventSystem.OnStaminaRegenratedEventCaller(15);

        if(handContext != null)
        {
            handContext.OnTurnStartedEvent?.Invoke();
            TurnLogger.instance.Showlog("Player Turn Started");
            return;
        }

        EnemyContext enemyContext = _baseContext as EnemyContext;
        if (enemyContext != null)
        {
            enemyContext.onTurnStartedEvent?.Invoke();
            TurnLogger.instance.Showlog("Enemy Turn Started" + enemyContext.enemyModel.name);
            return;
        }


    }

    public void EndTurn()
    {
        HandContext handContext = _baseContext as HandContext;

        if (handContext != null)
        {
            TurnEventSystem.NextTurnEventCaller();
            return;
        }

        EnemyContext enemyContext = _baseContext as EnemyContext;
        if (enemyContext != null)
        {
            TurnEventSystem.NextTurnEventCaller();
            return;
        }
    }
}
