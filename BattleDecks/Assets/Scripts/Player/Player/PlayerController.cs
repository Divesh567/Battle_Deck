using UnityEngine.Events;

public class PlayerController : ICardAffectable
{

    public class CardEffectOnSelf : UnityEvent<CardEffect> { }
    public CardEffectOnSelf CardEffectOnSelfEvent = new CardEffectOnSelf();

    public class StaminaReduced : UnityEvent<int> { }
    public StaminaReduced StaminaReducedEvent = new StaminaReduced();

    public class DamageTaken : UnityEvent<int> { }
    public DamageTaken DamageTakenEvent = new DamageTaken();

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
        DamageTakenEvent.AddListener(OnDamageTaken);
    }

    public void ApplyCardEffectToThis(CardEffect effect)
    {
        _playerBehaviour.CheckBehaviours(effect, this);
    }

    public void ApplyEffectToSelf(CardEffect effect)
    {
        _playerBehaviour.CheckBehaviours(effect, this);
    }

    private void OnStaminaReducedEvent(int amount)
    {
        PlayerEventSystem.OnStaminaReducedEventCaller(amount);
    }

    private void OnDamageTaken(int amount)
    {
        PlayerEventSystem.OnDamageTakenEventCaller(amount);
    }

    public void EnableSelector() // To be refactored
    {
        throw new System.NotImplementedException();
    }
}
