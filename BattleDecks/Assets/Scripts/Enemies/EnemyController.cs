using UnityEngine.Events;

public class EnemyController : ICardAffectable
{

    //Events
    public class DamageTaken : UnityEvent<int> { }
    public DamageTaken DamageTakenEvent = new DamageTaken();

    public class AttackDoged : UnityEvent { }
    public AttackDoged AttackDodgedEvent = new AttackDoged();

    public class Stunned : UnityEvent { }
    public Stunned StunnedEvent = new Stunned();



    public EnemyModel enemyModel;
    public EnemyView enemyView;
    public Behaviour enemyBehaviour;

   
    public void Init(EnemyModel model, EnemyView view, Behaviour Behaviour)
    {
        enemyModel = model;
        enemyView = view;
        enemyBehaviour = Behaviour;


        enemyModel.InitEnemyStats();


        enemyModel.playerDeadEvent.AddListener(() => OnPlayerDead());

        DamageTakenEvent.AddListener(TakeDamage);
        AttackDodgedEvent.AddListener(() => DodgeAttack());
        StunnedEvent.AddListener(() => GetStunned());
    }


    private void OnPlayerDead()
    {
        enemyView.KillEnemey();
    }
    public void OnEffectApplied(CardEffect effect)
    {
        enemyBehaviour.CheckBehaviours(effect, this);
    }

    private void TakeDamage(int damage)
    {
        enemyModel.ReduceHealth(damage);
    }

    private void DodgeAttack()
    {
        EnemyLogger.instance.Showlog("Attack Dodged");
    }

    private void GetStunned()
    {
        enemyModel.Stunned();
    }

    public void ApplyCardToSelf(CardEffect model)
    {
        // Not Applicable Here
    }
}
