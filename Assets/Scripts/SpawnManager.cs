using System;
using System.Collections;
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
    2: EnemyMissileer
    \*-----Enemy Array Indices-----*/
    [Header("Enemy")]
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject _enemyLeader;
    [SerializeField] private float _enemyLeaderDramaticWaitTime = 3f;
    [SerializeField] private GameObject[] _enemies;
    [SerializeField] private int[] _enemySpawnDistribution;
    [SerializeField] private float _enemySpawnTime = 2f;
    [SerializeField] private float _minEnemySpawnTime = 1f;
    [SerializeField] private float _enemySpawnTimeDecrement = 0.1f;
    private GameObject _spawnedEnemy;
    private Vector2 _enemySpawnPosition;

    /*-----PowerUp Array Indices-----*\
    0: SpeedBoost
    1: TripleShot
    2: HomingMissile
    3: SpaceBomb
    4: Shield
    5: PlayerLife
    6: SlowBomb
    \*-----PowerUp Array Indices-----*/
    [Header("PowerUps")]
    [SerializeField] private GameObject[] _powerUps;
    [SerializeField] private int[] _powerUpSpawnDistribution;

    [Header("Game Management")]
    [SerializeField] private bool _canSpawn = false;
    [SerializeField] private int _currentWave;
    [SerializeField] private int _waveEnemyTotalCount;
    [SerializeField] private int _enemiesSpawned;

    public static event Action onEnemyLeaderSpawn;

    private void OnEnable()
    {
        GameManager.onBossWaveFinalEnemy += CallEnemyleader;
        PowerCore.onPowerCorePowerUpDrop += SpawnPowerUp;
    }

    private void OnDisable()
    {
        GameManager.onBossWaveFinalEnemy -= CallEnemyleader;
        PowerCore.onPowerCorePowerUpDrop -= SpawnPowerUp;
    }

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("GameManager is null!");
        }

        if (_enemies.Length != _enemySpawnDistribution.Length)
        {
            Debug.LogError("Enemies and EnemySpawnDistribution have different lengths!");
        }
        if (_powerUps.Length != _powerUpSpawnDistribution.Length)
        {
            Debug.LogError("PowerUps and PowerUpSpawnDistribution have different lengths!");
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
        int spawnIndex = GetSpawnIndex(_powerUpSpawnDistribution);
        Instantiate(_powerUps[spawnIndex], spawnPosition, Quaternion.identity);
    }

    private void CallEnemyleader()
    {
        StartCoroutine(SpawnEnemyLeader());
    }

    private IEnumerator SpawnEnemyLeader()
    {
        yield return new WaitForSeconds(_enemyLeaderDramaticWaitTime);

        _spawnedEnemy = Instantiate(_enemyLeader);
        _spawnedEnemy.transform.parent = _enemyContainer.transform;
        // _enemiesSpawned++;

        onEnemyLeaderSpawn?.Invoke();
    }

    private IEnumerator SpawnEnemy()
    {
        _currentWave = _gameManager.GetCurrentWave();
        _waveEnemyTotalCount = _gameManager.GetWaveEnemyTotalCount();
        _enemiesSpawned = 0;

        while (_canSpawn)
        {
            float yPositionEnemySpawn = UnityEngine.Random.Range(_enemySpawnLowerBoundary, _enemySpawnUpperBoundary);
            _enemySpawnPosition = new Vector2(_enemySpawnRightBoundary, yPositionEnemySpawn);

            int spawnIndex = GetSpawnIndex(_enemySpawnDistribution);

            // Only spawn EnemyTrooper on Wave 1
            if (_currentWave == 1)
            {
                spawnIndex = 0;
            }

            _spawnedEnemy = Instantiate(_enemies[spawnIndex], _enemySpawnPosition, Quaternion.identity);
            _spawnedEnemy.transform.parent = _enemyContainer.transform;
            _enemiesSpawned++;

            if (_gameManager.IsGameOver() ||
                _enemiesSpawned == _waveEnemyTotalCount ||
                (_gameManager.IsBossWave() && _enemiesSpawned == _waveEnemyTotalCount - 1))
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

    private int GetSpawnIndex(int[] spawnDistributionArray)
    {
        int spawnIndex = 0;
        int totalWeight = 0;

        foreach (int weight in spawnDistributionArray)
        {
            totalWeight += weight;
        }

        int randomValue = UnityEngine.Random.Range(0, totalWeight + 1);

        for (int i = 0; i < spawnDistributionArray.Length; i++)
        {
            if (randomValue <= spawnDistributionArray[i])
            {
                spawnIndex = i;
                break;
            }
            else
            {
                randomValue -= spawnDistributionArray[i];
            }
        }

        return spawnIndex;
    }
}
