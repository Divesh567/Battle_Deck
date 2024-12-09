using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "Enemy", menuName = "Enemy/New Enemy", order = 0)]
public class EnemyModel : ScriptableObject
{
    public class OnEnemyDeadEvent : UnityEvent {}

    public OnEnemyDeadEvent enemyDeadEvent = new OnEnemyDeadEvent();

    public int MaxHealth;




    public ObservableVariable<int> obvHealth = new ObservableVariable<int>();
    private int _health;
    public int Health
    {
        get { return _health; }

        set
        {
            _health = value;
            obvHealth.Var = _health;
        }
    }



    public bool isStunned;

    [Header("Attack Cards")]
    [SerializeField]
    private List<CardModel> attackCards;

    [Space(10)]

    [Header("Defense Cards")]
    [SerializeField]
    private List<CardModel> defenseCards;

    [Space(10)]

    [Header("Misc Cards")]
    [SerializeField]
    private List<CardModel> miscCards;


    public void InitEnemyStats()
    {
        Health = MaxHealth;
    }
    public void ReduceHealth(int Amount)
    {
        EnemyLogger.instance.Showlog("Health Reduced " + _health + "-" + Amount + " = " + (_health - Amount));
        Health -= Amount;
        if (_health <= 0)
        {
            enemyDeadEvent.Invoke();
        }
    }
    public void Stunned()
    {
        isStunned = true;
        EnemyLogger.instance.Showlog("isStunned " + isStunned);
    }

    public  void PlayAttackCard()
    {
      //  attackCards[0].cardEffects.ForEach(x => x.ApplyEffectToTarget(FindAnyObjectByType<PlayerContext>())); // to be refactored
    }
}
