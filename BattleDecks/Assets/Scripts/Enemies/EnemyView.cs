using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using System;


public class EnemyView : MonoBehaviour
{

    public class OnEnemyTurnEndedEvent : UnityEvent { }
    public OnEnemyTurnEndedEvent enemyTurnEndedEvent = new OnEnemyTurnEndedEvent();

    public class OnEnemyTurnStartedEvent : UnityEvent { }
    public OnEnemyTurnStartedEvent enemyTurnStartedEvent = new OnEnemyTurnStartedEvent();

    public class OnEnemyDamageEvent : UnityEvent<int> { }
    public OnEnemyDamageEvent enemyDamageEvent = new OnEnemyDamageEvent();

    public class OnEnemyAttackEvent : UnityEvent { }
    public OnEnemyAttackEvent enemyAttackEvent = new OnEnemyAttackEvent();

    public class OnEnemyDeadEvent : UnityEvent { }
    public OnEnemyDeadEvent enemyDeadEvent = new OnEnemyDeadEvent();

    public class TargetSelected : UnityEvent { }
    public TargetSelected TargetSelectedEvent = new TargetSelected();

    public Collider Collider;


    private EnemyModel enemyModel;
    public EnemyAnimationController enemyAnimator;
    public EnemyUi enemyUi;
    public EnemyStats enemyStats;

    public void Init(EnemyModel enemyModel)
    {
      
        this.enemyModel = enemyModel;


        enemyStats = new EnemyStats(this.enemyModel.enemyStats, this);
       
        enemyUi.InitUi(this.enemyModel);

        AddListenersToEvents();
    }

    private void AddListenersToEvents()
    {
        enemyStats.obvHealth.valueChanged.AddListener(enemyUi.OnHealthReduced);


        enemyDamageEvent.AddListener(OnDamageTaken);
        enemyAttackEvent.AddListener(OnAttackDone);
        enemyTurnStartedEvent.AddListener(OnBattleTurnStarted);
    }

    private void RemoveAllEventListners()
    {
        enemyDamageEvent.AddListener(OnDamageTaken);
        enemyAttackEvent.AddListener(OnAttackDone);
        enemyTurnStartedEvent.AddListener(OnBattleTurnStarted);

        enemyStats.obvHealth.valueChanged.RemoveAllListeners();
    }

    private void OnBattleTurnStarted()
    {
        enemyAnimator.SetReady();
    }


    private void OnDamageTaken(int value)
    {
        enemyStats.ReduceHealth(value);
        enemyAnimator.PlayGetHit();
    }

    private void OnAttackDone()
    {
        enemyAnimator.PlayAttack();

        DOVirtual.DelayedCall(2f, () => {

            enemyTurnEndedEvent.Invoke();

        });
    }

    public void KillEnemey()
    {
        RemoveAllEventListners();
        enemyAnimator.PlayDeath();

        DOVirtual.DelayedCall(2f, () => {

            TargetSelector.unRegisterAction.Invoke(transform.GetComponentInParent<ICardAffectable>());

            Destroy(transform.parent.gameObject);

        });

        

    }

    private void OnMouseDown()
    {
        Collider.enabled = false;
        TargetSelectedEvent.Invoke();
    }

  
}
