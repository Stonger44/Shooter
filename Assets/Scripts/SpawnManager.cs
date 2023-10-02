using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private float _enemySpawnRightBoundary = 11.2f;
    [SerializeField] private float _enemySpawnUpperBoundary = 4.9f;
    [SerializeField] private float _enemySpawnLowerBoundary = -4.9f;

    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private float _spawnTime = 1;

    private GameObject _spawnedEnemy;
    private Vector3 _enemySpawnPosition;
    

    [SerializeField] private bool _stopSpawning = false;

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
