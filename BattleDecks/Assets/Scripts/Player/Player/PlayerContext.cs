using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerContext : MonoBehaviour, ICardAffectable
{

    [SerializeField]
    private PlayerView _view;
    private PlayerController controller;
    [SerializeField]
    private PlayerModel _model;

    public Behaviour behaviour;

    public List<EnemyContext> currentEnemies = new List<EnemyContext>();

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
        EventManager.OnEnemySpawnedEvent += AddEnemyTolist;


        PlayerEventSystem.CardusedEvent += UseCard;

    }

    private void OnDisable()
    {
        EventManager.OnEnemySpawnedEvent -= AddEnemyTolist;


        PlayerEventSystem.CardusedEvent -= UseCard;

    }

    public void AddEnemyTolist(EnemyContext enemy)
    {
        currentEnemies.Add(enemy);
    }

    private void UseCard(CardModel model)
    {
        model.cardEffects.ForEach(x => x.ApplyEffectToTarget(currentEnemies[0]));

        model.cardEffects.ForEach(x => x.ApplyEffectToTarget(this));
    }

    public void ApplyCardToSelf(CardEffect effect)
    {
        controller.CardEffectOnSelfEvent.Invoke(effect);
    }
}
