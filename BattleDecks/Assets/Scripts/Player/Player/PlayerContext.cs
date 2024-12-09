using System.Collections.Generic;
using UnityEngine;

public class PlayerContext : BaseContext, ICardAffectable
{

    [SerializeField]
    private PlayerView _view;
    private PlayerController controller;
    [SerializeField]
    private PlayerModel _model;

    public Behaviour behaviour;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize() 
    {
        controller = new PlayerController();
        controller.InitalizeController(_view, _model, behaviour);
    }

    private void OnEnable()
    {
        PlayerEventSystem.CardSelectedEvent += OnCardSelected;
        PlayerEventSystem.CardUsedEvent += ApplyEffect;
    }

    private void OnDisable()
    {
        PlayerEventSystem.CardSelectedEvent -= OnCardSelected;
        PlayerEventSystem.CardUsedEvent -= ApplyEffect;

    }
    private void Start()
    {
        TargetSelector.registerAction.Invoke(this);
    }


    private void OnCardSelected(CardModel model)
    {
        Debug.Log("TARGET SELECTED");
        TargetSelector.activateTargets(model);
    }



    public void ApplyEffect()
    {
        var selectedCard = TargetSelector.currentSelectedCard;

        foreach(CardEffect effect in selectedCard.cardEffects)
        {
            if (effect.UseOnSelf)
                ApplyEffectToSelf(effect);
        }
      
    }

    public void EnableSelector()
    {
        
    }

    public void ApplyEffectToSelf(CardEffect effect)
    {
        controller.CardEffectOnSelfEvent.Invoke(effect);
    }
}
