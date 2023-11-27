using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private GameManager _gameManager;

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
    [SerializeField] private float _enemySpawnTime = 2f;
    [SerializeField] private float _minEnemySpawnTime = 1f;
    [SerializeField] private float _enemySpawnTimeDecrement = 0.1f;
    private GameObject _spawnedEnemy;
    private Vector2 _enemySpawnPosition;

    /*-----PowerUp Array Indices-----*\
    0: TripleShot
    1: SpeedBoost
    2: Shield
    3: SpaceBomb
    4: PlayerLife
    5: SlowBomb
    \*-----PowerUp Array Indices-----*/
    [Header("PowerUps")]
    [SerializeField] private GameObject[] _powerUps;
    [SerializeField] private float _rareSpawnChance = 0.2f;

    [Header("Game Management")]
    [SerializeField] private bool _canSpawn = false;
    [SerializeField] private int _waveEnemyTotalCount;
    [SerializeField] private int _enemiesSpawned;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("GameManager is null!");
        }
    }

    public void StopSpawning()
    {
        _canSpawn = false;
    }

    public void StartSpawning()
    {
        _canSpawn = true;
        StartCoroutine(SpawnEnemy());
    }

    public void SpawnPowerUp(Vector2 spawnPosition)
    {
        int randomIndex = GetRandomIndex(_powerUps, 3);
        Instantiate(_powerUps[randomIndex], spawnPosition, Quaternion.identity);
    }

    private IEnumerator SpawnEnemy()
    {
        _waveEnemyTotalCount = _gameManager.GetWaveEnemyTotalCount();
        _enemiesSpawned = 0;

        while (_canSpawn)
        {
            float yPositionEnemySpawn = Random.Range(_enemySpawnLowerBoundary, _enemySpawnUpperBoundary);
            _enemySpawnPosition = new Vector2(_enemySpawnRightBoundary, yPositionEnemySpawn);

            int randomIndex = GetRandomIndex(_enemies, 1);
            _spawnedEnemy = Instantiate(_enemies[randomIndex], _enemySpawnPosition, Quaternion.identity);
            _spawnedEnemy.transform.parent = _enemyContainer.transform;
            _enemiesSpawned++;

            if (_enemiesSpawned == _waveEnemyTotalCount)
            {
                StopSpawning();
            }

            yield return new WaitForSeconds(_enemySpawnTime);
        }

        if (_enemySpawnTime > _minEnemySpawnTime)
        {
            _enemySpawnTime -= _enemySpawnTimeDecrement;
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
}
