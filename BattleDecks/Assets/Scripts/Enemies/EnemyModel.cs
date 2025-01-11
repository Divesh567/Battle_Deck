using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "Enemy", menuName = "Enemy/New Enemy", order = 0)]
public class EnemyModel : ScriptableObject
{
   

    public EnemyStats enemyStats;
    public TurnUIDetails enemyTurnUIDetails;

    [Header("Attack Cards")]
    [SerializeField]
    private List<CardModel> attackCards;

    [HideInInspector]
    public List<CardModel> AttackCards { get { return attackCards; }  }

    [Space(10)]

    [Header("Defense Cards")]
    [SerializeField]
    private List<CardModel> defenseCards;


    [HideInInspector]
    public List<CardModel> DefenseCards { get { return defenseCards; } }

    [Space(10)]

    [Header("Misc Cards")]
    [SerializeField]
    private List<CardModel> miscCards;
}

[System.Serializable]
public class EnemyStats
{

    public EnemyView enemyView;
    public EnemyStats(EnemyStats enemyStats, EnemyView enemyView)
    {
        this.MaxHealth = enemyStats.MaxHealth;
        this._health = enemyStats._health;
        this.enemyView = enemyView;
        Health = this.MaxHealth;
    }

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


    public void InitEnemyStats()
    {

    }
    public void ReduceHealth(int Amount)
    {
        EnemyLogger.instance.Showlog("Health Reduced " + _health + "-" + Amount + " = " + (_health - Amount));
        Health -= Amount;
        if (_health <= 0)
        {
            enemyView.enemyDeadEvent.Invoke();

        }
    }
    public void Stunned()
    {

    }



    public bool isStunned;
}
