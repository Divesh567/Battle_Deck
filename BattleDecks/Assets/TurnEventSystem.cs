using System;

public static class TurnEventSystem 
{
    public static event Action<TurnClass> AddObjectsEvent;

    public static void AddObjectsEventCaller(TurnClass turnObject)
    {
        AddObjectsEvent?.Invoke(turnObject);
    }

    public static event Action NextTurnEvent;

    public static void NextTurnEventCaller()
    {
        NextTurnEvent?.Invoke();
    }

}
