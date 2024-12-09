using System;

public class PlayerEventSystem : SingletonBaseClass<PlayerEventSystem>
{


    public static event Action<CardModel> CardSelectedEvent;

    public static void CardSelectedEventCaller(CardModel model)
    {
        CardSelectedEvent?.Invoke(model);
    }


    public static event Action CardUsedEvent;
    public static void CardUsedEventEventCaller()
    {
        CardUsedEvent?.Invoke();
    }

    public static event Action<int> OnStaminaReducedEvent;
    public static void OnStaminaReducedEventCaller(int amount)
    {
        OnStaminaReducedEvent?.Invoke(amount);
    }


    public static event Action<int> OnDamageTakenEvent;
    public static void OnDamageTakenEventCaller(int amount)
    {
        OnDamageTakenEvent?.Invoke(amount);
    }
}
