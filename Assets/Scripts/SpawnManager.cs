using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private float _enemySpawnRightBoundary = 11.2f;
    [SerializeField] private float _enemySpawnUpperBoundary = 3.7f;
    [SerializeField] private float _enemySpawnLowerBoundary = -4.9f;

    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private float _enemySpawnTime = 2;
    private GameObject _spawnedEnemy;
    private Vector2 _enemySpawnPosition;
    [SerializeField] private GameObject _asteroidPrefab;
    [SerializeField] private float _asteroidSpawnTime = 4;
    private GameObject _spawnedAsteroid;
    private Vector2 _asteroidSpawnPosition;

    [SerializeField] private GameObject[] _powerUps;

    [SerializeField] private bool _stopSpawning = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemy());
        StartCoroutine(SpawnAsteroid());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StopSpawning()
    {
        _stopSpawning = true;
    }

    public void SpawnPowerUp(Vector2 spawnPosition)
    {
        if (!_stopSpawning)
        {
            int randomPowerUpIndex = Random.Range(0, _powerUps.Length);
            Instantiate(_powerUps[randomPowerUpIndex], spawnPosition, Quaternion.identity);
        }
    }

    private IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(_enemySpawnTime);

        while (!_stopSpawning)
        {
            float yPositionEnemySpawn = Random.Range(_enemySpawnLowerBoundary, _enemySpawnUpperBoundary);
            _enemySpawnPosition = new Vector2(_enemySpawnRightBoundary, yPositionEnemySpawn);
            _spawnedEnemy = Instantiate(_enemyPrefab, _enemySpawnPosition, Quaternion.identity);
            _spawnedEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(_enemySpawnTime);
        }
    }

    private IEnumerator SpawnAsteroid()
    {
        yield return new WaitForSeconds(_asteroidSpawnTime);

        while (!_stopSpawning)
        {
            float yPositionAsteroidSpawn = Random.Range(_enemySpawnLowerBoundary, _enemySpawnUpperBoundary);
            _asteroidSpawnPosition = new Vector2(_enemySpawnRightBoundary, yPositionAsteroidSpawn);
            _spawnedAsteroid = Instantiate(_asteroidPrefab, _asteroidSpawnPosition, Quaternion.identity);
            _spawnedAsteroid.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(_asteroidSpawnTime);
        }
    }
}
