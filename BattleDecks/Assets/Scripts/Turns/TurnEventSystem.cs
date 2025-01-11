using System;

public static class TurnEventSystem 
{
    public static event Action<TurnClass> AddObjectsEvent;

    public static void AddObjectsEventCaller(TurnClass turnObject)
    {
        AddObjectsEvent?.Invoke(turnObject);
    }

    public static event Action<TurnClass> RemoveObjectsEvent;
    public static void RemoveObjectsEventCaller(TurnClass turnObject)
    {
        RemoveObjectsEvent?.Invoke(turnObject);
    }


    public static event Action OnTurnEndedEvent;

    public static void OnTurnEndedEventCaller()
    {
        OnTurnEndedEvent?.Invoke();
    }



    public static event Action NextTurnEvent;

    public static void NextTurnEventCaller()
    {
        NextTurnEvent?.Invoke();
    } 
}
