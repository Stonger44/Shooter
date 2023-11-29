using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRavager : MonoBehaviour
{
    private const string _playerTag = "Player";
    private const string _laserTag = "Laser";
    private const string _tripleShotTag = "TripleShot";
    private const string _blastZoneTag = "BlastZone";
    private string _otherTag = string.Empty;

    private Player _player;
    private GameManager _gameManager;
    private SpawnManager _spawnManager;
    private AudioManager _audioManager;
    private CircleCollider2D _collider;

    [Header("Boundaries")]
    [SerializeField] private float _enemyLeftBoundary = -11.3f;
    [SerializeField] private float _enemyRightBoundary = 11.3f;
    [SerializeField] private float _enemyUpperBoundary = 3.5f;
    [SerializeField] private float _enemyLowerBoundary = -4.7f;

    [Header("Movement")]
    [SerializeField] private float _speed = 2f;
    private Vector2 _position;
    private Vector2 _direction = Vector2.left;

    [Header("Health/Damage")]
    [SerializeField] private int _maxHealth = 12;
    [SerializeField] private int _health = 12;
    private bool _isExploding;
    [SerializeField] private int _pointsOnDeath = 200;
    [SerializeField] private int _pointsOnBoundary = -20;
    [SerializeField] private GameObject _explosion;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is null!");
        }
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("GameManager is null!");
        }
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("SpawnManager is null!");
        }
        _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        if (_audioManager == null)
        {
            Debug.LogError("AudioManager is null!");
        }
        _collider = GetComponent<CircleCollider2D>();
        if (_collider == null)
        {
            Debug.LogError("Enemy Collider is null!");
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

        if (!_isExploding && transform.position.x < _enemyLeftBoundary)
        {
            if (!_gameManager.IsGameOver())
            {
                _gameManager.UpdateScore(_pointsOnBoundary);
            }
            Warp();
        }
    }

    private void Warp()
    {
        float yPosition = Random.Range(_enemyLowerBoundary, _enemyUpperBoundary);
        _position = new Vector2(_enemyRightBoundary, yPosition);
        transform.position = _position;
        _health = _maxHealth;
    }
}
