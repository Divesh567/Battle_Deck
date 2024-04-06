using System;
using UnityEngine.Events;
using UnityEngine;

public class TurnEventSystem : SingletonBaseClass<TurnEventSystem>
{

    public static event Action<TurnClass> AddObjectsEvent;

    public static void AddObjectsEventCaller(TurnClass turnObject)
    {
        AddObjectsEvent?.Invoke(turnObject);
    }

}
