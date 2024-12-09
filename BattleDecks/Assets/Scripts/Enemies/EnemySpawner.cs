
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    [SerializeField]
    private EnemyContext enemy;

    private void OnEnable()
    {
        EventManager.OnGameStartButtonClickEvent += SpawnEnemy;
     
    }


    private void OnDisable()
    {
        EventManager.OnGameStartButtonClickEvent -= SpawnEnemy;
    }
    private void Start()
    {
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        var newEnemy =  Instantiate(enemy);
        EventManager.OnEnemySpawnedEventCaller(newEnemy); // Not - Used 
        TargetSelector.registerAction.Invoke(newEnemy);
    }



   
}
