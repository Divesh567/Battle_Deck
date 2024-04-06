using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Logger<T> : Logger where T : Logger<T>
{
    private static T _instance;
    public static T instance { get { return _instance; } }

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


    public bool canShowLog;
    public Color preFixColor;

    public void Showlog(object obj)
    {
        if (canShowLog)
        {
            obj.ToString();
            var logString = $"<color=#{ColorUtility.ToHtmlStringRGB(preFixColor)}>{obj}</color>";
            Log(logString);
        }
    }

    private void Log(string obj)
    {
        Debug.Log(obj);
    }
}

public abstract class Logger : MonoBehaviour
{

}

