using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject _enemyPrefab;
    private GameObject _spawnedEnemy;
    private Vector3 _enemySpawnPosition;
    [SerializeField] private float _spawnTime;

    [SerializeField] private float _enemySpawnRightBoundary = 12f;
    [SerializeField] private float _enemySpawnUpperBoundary = 4.5f;
    [SerializeField] private float _enemySpawnLowerBoundary = -4.5f;

    private bool _stopSpawning;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator SpawnEnemy()
    {
        while (_stopSpawning == false)
        {
            float xPositionEnemySpawn = Random.Range(_enemySpawnLowerBoundary, _enemySpawnUpperBoundary);
            _enemySpawnPosition = new Vector3(xPositionEnemySpawn, _enemySpawnRightBoundary, 0);
            _spawnedEnemy = Instantiate(_enemyPrefab, _enemySpawnPosition, Quaternion.identity);
            _spawnedEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(_spawnTime);
        }
    }

    public void StopSpawning()
    {
        _stopSpawning = true;
    }
}
