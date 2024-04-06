using System;

public class EventManager : SingletonBaseClass<EventManager>
{

    public static event Action<CardView> CardSelectedEvent;


    public static void CardSelectedEventCaller(CardView view)
    {
        CardSelectedEvent?.Invoke(view);
    }


    public static event Action<CardView> CardUnSelectedEvent;

    public static void CardUnSelectedEventCaller(CardView view)
    {
        CardUnSelectedEvent?.Invoke(view);
    }


    public static event Action<bool> CardLimtReachedEvent;

    public static void CardLimtReachedEventCaller(bool isLimitReached)
    {
        CardLimtReachedEvent?.Invoke(isLimitReached);
    }


    public static event Action OnGameStartButtonClickEvent;

    public static void OnGameStarButtontEventCaller()
    {
        OnGameStartButtonClickEvent?.Invoke();
    }


    public static event Action<EnemyContext> OnEnemySpawnedEvent;

    public static void OnEnemySpawnedEventCaller(EnemyContext enemy)
    {
        OnEnemySpawnedEvent?.Invoke(enemy);
    }


}
