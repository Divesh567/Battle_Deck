using UnityEngine.Events;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Enemy/New Enemy", order = 0)]
public class EnemyModel : ScriptableObject
{
    public class OnPlayerDeadEvent : UnityEvent { }

    public OnPlayerDeadEvent playerDeadEvent = new OnPlayerDeadEvent();

    public int MaxHealth; 
    private int _currentHealth; // implement observable for these variables

    public int currentHealth => _currentHealth;

    public bool isStunned;
    public void InitEnemyStats()
    {
        _currentHealth = MaxHealth;
    }
    public void ReduceHealth(int Amount)
    {
        EnemyLogger.instance.Showlog("Health Reduced " + _currentHealth + "-" + Amount + " = " + (_currentHealth - Amount));
        _currentHealth -= Amount;
        if (_currentHealth <= 0)
        {
            playerDeadEvent.Invoke();
        }

    }

    public void Stunned()
    {
        isStunned = true;
        EnemyLogger.instance.Showlog("isStunned " + isStunned);
    }
}
