
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    [SerializeField]
    private EnemyContext enemy;

    private void OnEnable()
    {
        EventManager.OnGameStartButtonClickEvent += SpawnEnemy;
        SpawnEnemy();
    }


    private void OnDisable()
    {
        EventManager.OnGameStartButtonClickEvent -= SpawnEnemy;
    }

    private void SpawnEnemy()
    {
        var newEnemy =  Instantiate(enemy);
        EventManager.OnEnemySpawnedEventCaller(newEnemy);
    }
}
