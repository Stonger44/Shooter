// using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Enemy : MonoBehaviour
{
    private const string _playerTag = "Player";
    private const string _laserTag = "Laser";
    private const string _tripleShotTag = "TripleShot";
    private string _otherTag = string.Empty;

    [SerializeField] private float _enemyLeftBoundary = -11.2f;
    [SerializeField] private float _enemyRightBoundary = 11.2f;
    [SerializeField] private float _enemyUpperBoundary = 4.9f;
    [SerializeField] private float _enemyLowerBoundary = -4.9f;

    [SerializeField] private int _health = 3;
    [SerializeField] private float _speed = 6f;
    private Vector2 _position;
    private Vector2 _direction = Vector2.left;

    private SpawnManager _spawnManager;
    private Player _player;
    [SerializeField] private float _powerUpDropChance = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is null!");
        }
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("SpawnManager is null!");
        }
        Warp();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(_direction * _speed * Time.deltaTime);

        if (transform.position.x < _enemyLeftBoundary)
        {
            Warp();
        }
    }

    private void Warp()
    {
        float yPosition = Random.Range(_enemyLowerBoundary, _enemyUpperBoundary);
        _position = new Vector2(_enemyRightBoundary, yPosition);
        transform.position = _position;
        _health = 3;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _otherTag = other.tag;

        if (_otherTag == _playerTag)
        {
            _player.Damage();
            Damage(_otherTag);
        }
        else if (_otherTag == _laserTag || _otherTag == _tripleShotTag)
        {
            Destroy(other.gameObject);
            Damage(_otherTag);
        }
    }

    private void Damage(string otherTag)
    {
        _health--;

        if (_health < 1 || otherTag == _playerTag || otherTag == _tripleShotTag)
        {
            RollPowerUpDrop();
            _player.AddScore(10);
            Destroy(this.gameObject);
        }
    }

    private void RollPowerUpDrop()
    {
        float randomFloat = Random.Range(0f, 1.0f);
        if (randomFloat <= _powerUpDropChance)
        {
            _spawnManager.SpawnPowerUp(this.transform.position);
        }
    }
}
