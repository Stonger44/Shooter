using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
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

    [SerializeField] private GameObject _asteroidSprite;
    [SerializeField] private GameObject _explosion;

    [Header("Boundaries")]
    [SerializeField] private float _asteroidLeftBoundary = -11.3f;
    [SerializeField] private float _asteroidRightBoundary = 11.3f;
    [SerializeField] private float _asteroidUpperBoundary = 3.5f;
    [SerializeField] private float _asteroidLowerBoundary = -4.7f;

    [Header("Movement")]
    [SerializeField] private float _speed = 2f;
    private Vector2 _position;
    private Vector2 _direction = Vector2.left;

    [Header("Health/Damage")]
    [SerializeField] private int _health = 6;
    private bool _isExploding;
    [SerializeField] private int _pointsOnDeath = 200;
    [SerializeField] private int _pointsOnBoundary = -20;

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
            Debug.LogError("Asteroid Collider is null!");
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

        if (!_isExploding && transform.position.x < _asteroidLeftBoundary)
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
        float yPosition = Random.Range(_asteroidLowerBoundary, _asteroidUpperBoundary);
        _position = new Vector2(_asteroidRightBoundary, yPosition);
        transform.position = _position;
        _health = 6;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _otherTag = other.tag;

        switch (_otherTag)
        {
            case _playerTag:
                _player.Damage();
                Damage(_otherTag);
                break;
            case _laserTag:
            case _tripleShotTag:
                Destroy(other.gameObject);
                Damage(_otherTag);
                break;
            case _blastZoneTag:
                Damage(_otherTag);
                break;
            default:
                break;
        }
    }

    private void Damage(string otherTag)
    {
        _health--;

        if (otherTag == _tripleShotTag)
        {
            _health -= 3;
        }

        if (_health < 1 || otherTag == _playerTag || otherTag == _blastZoneTag)
        {
            StartCoroutine(DestroyAsteroid());
        }
    }

    private IEnumerator DestroyAsteroid()
    {
        _isExploding = true;
        _collider.enabled = false;
        _explosion.SetActive(true);
        _audioManager.PlayExplosionSound();
        Destroy(this.gameObject, 2.7f);

        yield return new WaitForSeconds(0.25f);

        _gameManager.UpdateScore(_pointsOnDeath);
        _gameManager.UpdateEnemyCount(1);
        _asteroidSprite.SetActive(false);
        _spawnManager.SpawnPowerUp(this.transform.position);
    }
}
