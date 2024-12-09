using UnityEngine;
using System;
using UnityEngine.Events;

public class PlayerUIController
{
    private PlayerUIModel _model;
    private PlayerUIView _view;

    public class StaminaReduced : UnityEvent<int> { }
    public StaminaReduced StaminaConsumedEvent = new StaminaReduced();

    public class DamageTaken : UnityEvent<int> { }
    public DamageTaken DamageTakenEvent = new DamageTaken();

    public void Initalize(PlayerUIModel model, PlayerUIView view )
    {
        _model = model;
        _view = view;

        _model.InitModel();
        _view.Init(model);

        StaminaConsumedEvent.AddListener(OnStaminaConsumedEvent);
    }

    
    public void OnStaminaConsumedEvent(int amount)
    {
        _model.StaminaConsumedEvent.Invoke(amount);
        
    }

    public void OnDamageTakenEvent(int amount)
    {
        _model.StaminaConsumedEvent.Invoke(amount);
    }


}