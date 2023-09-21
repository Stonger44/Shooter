using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemy;
    private Vector3 _enemySpawnPosition;
    [SerializeField] private GameObject _player;
    private Player _playerScript;

    // Start is called before the first frame update
    void Start()
    {
        _playerScript = _player.GetComponent<Player>();
        StartCoroutine(SpawnEnemy());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator SpawnEnemy()
    {
        while (_playerScript.Health > 0)
        {
            _enemySpawnPosition.x = Random.Range(-3.5f, 5.5f);
            _enemySpawnPosition = new Vector3(_enemySpawnPosition.x, 12f, 0);
            Instantiate(_enemy, _enemySpawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(3);
        }
    }
}
