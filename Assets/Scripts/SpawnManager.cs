﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Boundaries")]
    [SerializeField] private float _enemySpawnRightBoundary = 11.2f;
    [SerializeField] private float _enemySpawnUpperBoundary = 3.7f;
    [SerializeField] private float _enemySpawnLowerBoundary = -4.9f;

    [Header("Enemy")]
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private float _enemySpawnTime = 2;
    private GameObject _spawnedEnemy;
    private Vector2 _enemySpawnPosition;

    [Header("Asteroid")]
    [SerializeField] private GameObject _asteroidPrefab;
    [SerializeField] private float _asteroidSpawnTime = 4;
    private GameObject _spawnedAsteroid;
    private Vector2 _asteroidSpawnPosition;

    /*-----Power Up Array Indices-----*\
    0: SpaceBomb
    1: TripleShot
    2: SpeedBoost
    3: Shield
    \*-----Power Up Array Indices-----*/
    [Header("PowerUps")]
    [SerializeField] private GameObject[] _powerUps;
    [SerializeField] private float _spaceBombSpawnChance = 0.10f;

    [Header("Game Management")]
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
        /*-----Power Up Array Indices-----*\
        0: SpaceBomb
        1: TripleShot
        2: SpeedBoost
        3: Shield
        \*-----Power Up Array Indices-----*/
        int randomIndex = 0;
        float randomFloat = Random.Range(0f, 1.0f);
        if (randomFloat < _spaceBombSpawnChance)
        {
            randomIndex = 0;
        }
        else
        {
            randomIndex = Random.Range(1, _powerUps.Length);
        }

        Instantiate(_powerUps[randomIndex], spawnPosition, Quaternion.identity);
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
