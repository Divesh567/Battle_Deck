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

        onTurnStartedEvent.AddListener(() => TurnStarted());


        turnClass = new TurnClass();
        turnClass.baseContext = this;
        turnClass.color = Color.red;
        turnClass.RegiterObjectForTurn();



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
        StartCoroutine(WaitforTurnEnd());
      
    }

    private IEnumerator WaitforTurnEnd()
    {
        yield return new WaitForSeconds(4f);

        enemyController.onTurnStartedEvent?.Invoke();
    }
    public void EnableSelector()
    {
        enemyController.EnableSelector();
    }

}
