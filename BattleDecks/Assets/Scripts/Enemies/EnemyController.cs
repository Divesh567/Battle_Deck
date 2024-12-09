using UnityEngine.Events;
using UnityEngine;
using System.Collections;
using System;

public class EnemyController : ICardAffectable
{

    //Events
    public class OnTurnStarted : UnityEvent { }
    public OnTurnStarted onTurnStartedEvent = new OnTurnStarted();


    public class TargetSelected : UnityEvent { }
    public TargetSelected TargetSelectedEvent = new TargetSelected();

    public class DamageTaken : UnityEvent<int> { }
    public DamageTaken DamageTakenEvent = new DamageTaken();

    public class AttackDoged : UnityEvent { }
    public AttackDoged AttackDodgedEvent = new AttackDoged();

    public class Stunned : UnityEvent { }
    public Stunned StunnedEvent = new Stunned();

   

    public EnemyModel enemyModel;
    public EnemyView enemyView;
    public Behaviour enemyBehaviour;

   
    public void Init(EnemyModel model, EnemyView view, Behaviour Behaviour)
    {
        enemyModel = model;
        enemyView = view;
        enemyBehaviour = Behaviour;


        enemyModel.InitEnemyStats();
        enemyView.Init(enemyModel);

        enemyModel.enemyDeadEvent.AddListener(() => OnPlayerDead());
        

        DamageTakenEvent.AddListener(TakeDamage);
        AttackDodgedEvent.AddListener(() => DodgeAttack());
        StunnedEvent.AddListener(() => GetStunned());
        onTurnStartedEvent.AddListener(TurnStarted);



        enemyView.TargetSelectedEvent.AddListener(ThisTargetSelected);

    }


    private void OnPlayerDead()
    {
        enemyView.KillEnemey();
    }
    public void OnEffectApplied(CardEffect effect)
    {
        enemyBehaviour.CheckBehaviours(effect, this);
    }

    private void TakeDamage(int damage)
    {
        enemyModel.ReduceHealth(damage);
    }

    private void DodgeAttack()
    {
        EnemyLogger.instance.Showlog("Attack Dodged");
    }

    private void GetStunned()
    {
        enemyModel.Stunned();
    }

    private void TurnStarted()
    {
        TurnEventSystem.NextTurnEventCaller();
    }

    public void ApplyEffectToSelf(CardEffect model)
    {
        // Not Applicable Here
    }

    private void ThisTargetSelected()
    {
        EnemyLogger.instance.Showlog("ENEMY ATTACKED");
        TargetSelector.currentSelectedCard.cardEffects.ForEach(x => OnEffectApplied(x));
        TargetSelectedEvent.Invoke();
    }

    public void EnableSelector()
    {
        enemyView.Collider.enabled = true;
    }
}
