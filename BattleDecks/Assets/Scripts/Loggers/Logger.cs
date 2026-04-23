using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DefaultExecutionOrder(-100)]
public abstract class Logger<T> : Logger where T : Logger<T>
{
    private static T _instance;
    public static T instance { get { return _instance; } }

    [SerializeField] private string categoryName = nameof(T);

    protected virtual void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = (T)this;
            DontDestroyOnLoad(gameObject);
        }
    }
    protected void OnDestroy()
    {
        _instance = null;
    }


    [SerializeField] private bool canShowLog = true;
    [SerializeField] private Color prefixColor = Color.white;

    public void Log(object obj)
    {
        if (canShowLog)
        {
            var message = obj?.ToString() ?? "null";
            var logString = $"<color=#{ColorUtility.ToHtmlStringRGB(prefixColor)}>[{categoryName}] {message}</color>";
            Debug.Log(obj);
        }
    }

}

public abstract class Logger : MonoBehaviour
{

}

