using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRadar : MonoBehaviour
{
    private const string _playerTag = "Player";

    [SerializeField] private EnemyTrooper _enemyTrooper;

    // Start is called before the first frame update
    void Start()
    {
        if (_enemyTrooper == null)
        {
            Debug.LogError("EnemyTrooper is null!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == _playerTag)
        {
            _enemyTrooper.IsRamming(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == _playerTag)
        {
            _enemyTrooper.IsRamming(false);
        }
    }
}
