using UnityEngine;


public class EnemyContext : MonoBehaviour,ICardAffectable
{
    public EnemyModel enemyModel;
    public EnemyController enemyController;
    public EnemyView enemyView;


    public Behaviour behaviour;

    private void Start()
    {
        enemyController = new EnemyController();

        enemyController.Init(enemyModel, enemyView, behaviour); 
    }


    public void ApplyCardToSelf(CardEffect model)
    {
        enemyController.OnEffectApplied(model);
    }
}
