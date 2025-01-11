using UnityEngine.Events;
using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;

public class EnemyController : ICardAffectable
{

    //Events
    public class OnTurnStarted : UnityEvent { }
    public OnTurnStarted onTurnStartedEvent = new OnTurnStarted();

    public class OnTurnEnded : UnityEvent { }
    public OnTurnEnded OnTurnEndedEvent = new OnTurnEnded();

    public class TargetSelected : UnityEvent { }
    public TargetSelected TargetSelectedEvent = new TargetSelected();

    public class DamageTaken : UnityEvent<int> { }
    public DamageTaken DamageTakenEvent = new DamageTaken();

    public class AttackDoged : UnityEvent { }
    public AttackDoged AttackDodgedEvent = new AttackDoged();

    public class Attack : UnityEvent { }
    public Attack AttackEvent = new Attack();

    public class Stunned : UnityEvent { }
    public Stunned StunnedEvent = new Stunned();

    public class OnEnemyDeadEvent : UnityEvent { }
    public OnEnemyDeadEvent EnemyDeadEvent = new OnEnemyDeadEvent();


    public EnemyModel enemyModel;
    public EnemyView enemyView;
    public Behaviour enemyBehaviour;

   
    public void Init(EnemyModel model, EnemyView view, Behaviour Behaviour)
    {
        enemyModel = model;
        enemyView = view;
        enemyBehaviour = Behaviour;


        enemyView.enemyStats.InitEnemyStats();
        enemyView.Init(enemyModel);

        enemyView.enemyDeadEvent.AddListener(() => OnPlayerDead());
        enemyView.enemyTurnEndedEvent.AddListener(() => OnTurnEndedEvent.Invoke());

        DamageTakenEvent.AddListener(TakeDamage);
        AttackDodgedEvent.AddListener(() => DodgeAttack());
        AttackEvent.AddListener(() => AttackPlayer());
        StunnedEvent.AddListener(() => GetStunned());


        onTurnStartedEvent.AddListener(TurnStarted);
        enemyView.TargetSelectedEvent.AddListener(ThisTargetSelected);

    }


    private void OnPlayerDead()
    {
        enemyView.KillEnemey();
        EnemyDeadEvent.Invoke();
    }
    public void OnEffectApplied(CardEffect effect)
    {
        enemyBehaviour.CheckBehaviours(effect, this);
    }

    private void TakeDamage(int damage)
    {
        enemyView.enemyDamageEvent.Invoke(damage);
    }

    private void DodgeAttack()
    {
        EnemyLogger.instance.Showlog("Attack Dodged");
    }

    private void AttackPlayer()
    {
        int damageValue = 0;

        DamageEffect damageEffect = enemyModel.AttackCards[0].cardEffects[0] as DamageEffect;
        if (damageEffect != null)
        {
            damageValue = damageEffect.damageAmount;
        }

        EnemyLogger.instance.Showlog(enemyModel.AttackCards[0].cardUIDetails.CardName + "attack To Player for" + damageValue + "points");

        PlayerEventSystem.OnDamageTakenEventCaller(damageValue);

        enemyView.enemyAttackEvent.Invoke();
    }

   

    private void GetStunned()
    {
        enemyView.enemyStats.Stunned();
    }

    private void TurnStarted()
    {
        enemyView.enemyTurnStartedEvent.Invoke();

        DOVirtual.DelayedCall(2f, () => 
        {

            AttackEvent.Invoke();

        });// #TODO to be refactored 

       
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
