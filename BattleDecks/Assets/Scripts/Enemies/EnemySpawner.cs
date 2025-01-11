
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    [SerializeField]
    private List<EnemyContext> enemies;

    [SerializeField]
    private List<Transform> enemySpawnPositions;



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

        for(int i = 0; i < enemies.Count; i++)
        {
            var newEnemy = Instantiate(enemies[i], enemySpawnPositions[i].position, Quaternion.identity);
            EventManager.OnEnemySpawnedEventCaller(newEnemy); // Not - Used 
            TargetSelector.registerAction.Invoke(newEnemy);
        }

       


    }



   
}
