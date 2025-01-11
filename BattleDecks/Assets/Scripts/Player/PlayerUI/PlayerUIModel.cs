using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New PlayerUIModel", menuName = "Player/UI/PlayerUIModel", order = 2)]

[Serializable]
public class PlayerUIModel : ScriptableObject
{
    public class StaminaReduced : UnityEvent<int> { }
    public StaminaReduced StaminaConsumedEvent = new StaminaReduced();
    public class StaminaRegenerated : UnityEvent<int> { }
    public StaminaRegenerated StaminaRegeneratedEvent = new StaminaRegenerated();
    public class HealthReduced : UnityEvent<int> { }
    public HealthReduced HealthReducedEvent = new HealthReduced();

    public ObservableVariable<int> obvStamina = new ObservableVariable<int>();
    private int _stamina;

    public int Stamina 
    {
        get { return _stamina; }

        set 
        {
            _stamina = value;
             obvStamina.Var = _stamina;
        }
    }


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


    public void InitModel()
    {
        Health = 100;

        StaminaConsumedEvent.AddListener(OnStaminaReduced);
        StaminaRegeneratedEvent.AddListener(OnStaminaRegenrated);
        HealthReducedEvent.AddListener(OnHealthReduced);

        Stamina = 100;
       
    }


    void OnStaminaReduced(int amount)
    {
        PlayerLogger.instance?.Showlog("Stamina Reduce by " + amount + " Points");

        Stamina -= amount;
    }

    void OnStaminaRegenrated(int amount)
    {
        PlayerLogger.instance?.Showlog("Stamina Regenrated by " + amount + " Points");

        Stamina += amount;
    }

    void OnHealthReduced(int amount)
    {
        Health -= amount;
    }
}