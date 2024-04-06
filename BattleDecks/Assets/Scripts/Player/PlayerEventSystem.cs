using System;

public class PlayerEventSystem : SingletonBaseClass<PlayerEventSystem>
{
    public static event Action<int> OnStaminaReducedEvent;

    public static event Action<CardModel> CardusedEvent;

    public static void CardusedEventCaller(CardModel model)
    {
        CardusedEvent?.Invoke(model);
    }

    public static void OnStaminaReducedEventCaller(int amount)
    {
        OnStaminaReducedEvent?.Invoke(amount);
    }
}
