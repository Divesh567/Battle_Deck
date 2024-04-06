using UnityEngine.Events;

public class PlayerController : ICardAffectable
{

    public class CardEffectOnSelf : UnityEvent<CardEffect> { }
    public CardEffectOnSelf CardEffectOnSelfEvent = new CardEffectOnSelf();

    public class StaminaReduced : UnityEvent<int> { }
    public StaminaReduced StaminaReducedEvent = new StaminaReduced();



    PlayerView _view;
    PlayerModel _model;
    public Behaviour _playerBehaviour;


    public void InitalizeController(PlayerView view, PlayerModel model, Behaviour enemyBehaviour)
    {
        _view = view;
        _model = model;
        _playerBehaviour = enemyBehaviour;

        CardEffectOnSelfEvent.AddListener(ApplyCardEffectToThis);
        StaminaReducedEvent.AddListener(OnStaminaReducedEvent);
    }

    public void ApplyCardEffectToThis(CardEffect effect)
    {
        _playerBehaviour.CheckBehaviours(effect, this);
    }

    public void ApplyCardToSelf(CardEffect model)
    {
        
    }

    private void OnStaminaReducedEvent(int amount)
    {
        PlayerEventSystem.OnStaminaReducedEventCaller(amount);
    }
}
