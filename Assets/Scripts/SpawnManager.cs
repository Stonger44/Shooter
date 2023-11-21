using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Boundaries")]
    [SerializeField] private float _enemySpawnRightBoundary = 11.2f;
    [SerializeField] private float _enemySpawnUpperBoundary = 3.7f;
    [SerializeField] private float _enemySpawnLowerBoundary = -4.9f;


    /*-----Enemy Array Indices-----*\
    0: Enemy
    1: Asteroid
    \*-----Enemy Array Indices-----*/
    [Header("Enemy")]
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject[] _enemies;
    [SerializeField] private float _enemySpawnTime = 2;
    private GameObject _spawnedEnemy;
    private Vector2 _enemySpawnPosition;

    //[Header("Asteroid")]
    //[SerializeField] private GameObject _asteroidPrefab;
    //[SerializeField] private float _asteroidSpawnTime = 4;
    //private GameObject _spawnedAsteroid;
    //private Vector2 _asteroidSpawnPosition;

    /*-----Power Up Array Indices-----*\
    0: TripleShot
    1: SpeedBoost
    2: Shield
    3: SpaceBomb
    4: PlayerLife
    \*-----Power Up Array Indices-----*/
    [Header("PowerUps")]
    [SerializeField] private GameObject[] _powerUps;
    [SerializeField] private float _rareSpawnChance = 0.2f;

    [Header("Game Management")]
    [SerializeField] private bool _stopSpawning = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemy());
        //StartCoroutine(SpawnAsteroid());
    }

    public void StopSpawning()
    {
        _stopSpawning = true;
    }

    public void SpawnPowerUp(Vector2 spawnPosition)
    {
        int randomIndex = GetRandomIndex(_powerUps, 3);
        Instantiate(_powerUps[randomIndex], spawnPosition, Quaternion.identity);
    }

    private IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(_enemySpawnTime);

        while (!_stopSpawning)
        {
            float yPositionEnemySpawn = Random.Range(_enemySpawnLowerBoundary, _enemySpawnUpperBoundary);
            _enemySpawnPosition = new Vector2(_enemySpawnRightBoundary, yPositionEnemySpawn);

            int randomIndex = GetRandomIndex(_enemies, 1);
            _spawnedEnemy = Instantiate(_enemies[randomIndex], _enemySpawnPosition, Quaternion.identity);
            _spawnedEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(_enemySpawnTime);
        }
    }

    private int GetRandomIndex(GameObject[] spawnList, int rareSpawnIndexStart)
    {
        int randomIndex = Random.Range(0, spawnList.Length);

        // This is to make certain spawns more rare:
        // if the index is one of the rare spawns, _rareSpawnChance to spawn, else roll one more time
        if (randomIndex >= rareSpawnIndexStart)
        {
            if (Random.value > _rareSpawnChance)
            {
                // Roll again
                randomIndex = Random.Range(0, spawnList.Length);
            }
        }

        return randomIndex;
    }

    //private IEnumerator SpawnAsteroid()
    //{
    //    yield return new WaitForSeconds(_asteroidSpawnTime);

    //    while (!_stopSpawning)
    //    {
    //        float yPositionAsteroidSpawn = Random.Range(_enemySpawnLowerBoundary, _enemySpawnUpperBoundary);
    //        _asteroidSpawnPosition = new Vector2(_enemySpawnRightBoundary, yPositionAsteroidSpawn);
    //        _spawnedAsteroid = Instantiate(_asteroidPrefab, _asteroidSpawnPosition, Quaternion.identity);
    //        _spawnedAsteroid.transform.parent = _enemyContainer.transform;
    //        yield return new WaitForSeconds(_asteroidSpawnTime);
    //    }
    //}
}
