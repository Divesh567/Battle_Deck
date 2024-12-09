using UnityEngine;


public class TurnClass : ITurnObject
{
    private BaseContext _baseContext;
    public Color color;

    public BaseContext baseContext { get { return _baseContext; } set { _baseContext = value; } }


    public void RegiterObjectForTurn()
    {
        TurnEventSystem.AddObjectsEventCaller(this);
    }

    public void StartTurn()
    {
        HandContext handContext = _baseContext as HandContext;
        if(handContext != null)
        {
            handContext.OnTurnStartedEvent?.Invoke();
            Debug.Log("PLAYER TURN STARTED");
            return;
        }

        EnemyContext enemyContext = _baseContext as EnemyContext;
        if (enemyContext != null)
        {
            enemyContext.onTurnStartedEvent?.Invoke();
            return;
        }


    }
}
