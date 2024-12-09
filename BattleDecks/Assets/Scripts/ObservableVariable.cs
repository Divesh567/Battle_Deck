using UnityEngine.Events;
using System;

[Serializable]
public class ObservableVariable<T> : UnityEvent<T>
{

    private T _var;
    public T  Var
    {
        get { return _var; }
        set 
        { 
            _var = value;
            valueChanged.Invoke(_var);
        }
    }

    public class OnValueChanged : UnityEvent<T>
    {
        
    }

    public OnValueChanged valueChanged = new OnValueChanged();



}
