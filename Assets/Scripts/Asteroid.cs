using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private const string _playerTag = "Player";
    private const string _laserTag = "Laser";
    private const string _tripleShotTag = "TripleShot";
    private string _otherTag = string.Empty;

    private Player _player;
    private SpawnManager _spawnManager;
    private AudioManager _audioManager;
    private CircleCollider2D _collider;

    [SerializeField] private GameObject _asteroidSprite;
    [SerializeField] private GameObject _explosion;

    [SerializeField] private float _asteroidLeftBoundary = -11.2f;
    [SerializeField] private float _asteroidRightBoundary = 11.2f;
    [SerializeField] private float _asteroidUpperBoundary = 3.7f;
    [SerializeField] private float _asteroidLowerBoundary = -4.9f;

    [SerializeField] private float _speed = 2f;
    private Vector2 _position;
    private Vector2 _direction = Vector2.left;

    [SerializeField] private int _health = 6;
    private bool _isExploding;

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

        if (otherTag == _tripleShotTag)
        {
            _health -= 3;
        }

        if (_health < 1 || otherTag == _playerTag)
        {
            _player.AddScore(20);
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

        _asteroidSprite.SetActive(false);
        _spawnManager.SpawnPowerUp(this.transform.position);
    }
}
