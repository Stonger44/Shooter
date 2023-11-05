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

    private Player _player;
    private SpawnManager _spawnManager;
    private AudioManager _audioManager;
    private AudioSource _audioSource;
    private Animator _animator;
    private CircleCollider2D _collider;

    [Header("Boundaries")]
    [SerializeField] private float _enemyLeftBoundary = -11f;
    [SerializeField] private float _enemyRightBoundary = 11f;
    [SerializeField] private float _enemyUpperBoundary = 4.1f;
    [SerializeField] private float _enemyLowerBoundary = -5.1f;

    [Header("Speed")]
    [SerializeField] private float _speed = 4f;
    private Vector2 _position;
    private Vector2 _direction = Vector2.left;

    [Header("Health/Damage")]
    [SerializeField] private int _health = 3;
    private bool _isExploding;
    [SerializeField] private float _powerUpDropChance = 0.25f;

    [Header("Laser")]
    [SerializeField] private GameObject _laserShot;
    [SerializeField] private AudioClip _laserSound;
    [SerializeField] private float _laserShotOffset = -0.955f;
    [SerializeField] private float _fireRate = 1f;
    [SerializeField] private float _xWEZ = 5f;
    [SerializeField] private float _yWEZ = 0.5f;
    private bool _canFire = true;

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
        _audioSource =GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("AudioSource is null!");
        }
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Enemy Animator is null!");
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

        SearchForTarget();
    }

    private void SearchForTarget()
    {
        if (TargetFound() && _canFire)
        {
            Fire();
        }
    }

    private bool TargetFound()
    {
        if (!_isExploding)
        {
            if (transform.position.x < 10)
            {
                if (_player != null)
                {
                    if ((_player.transform.position.y < transform.position.y + _yWEZ) && (_player.transform.position.y > transform.position.y - _yWEZ))
                    {
                        if (_player.transform.position.x < transform.position.x - _xWEZ)
                        {
                            return true;
                        }
                    }
                }
            } 
        }

        return false;
    }

    private void Fire()
    {
        Vector2 laserPosition = transform.position;
        laserPosition.x += _laserShotOffset;
        Instantiate(_laserShot, laserPosition, Quaternion.identity);

        SetLaserSound();
        _audioSource.Play();

        _canFire = false;
        StartCoroutine(ReadyFire());
    }

    private void SetLaserSound()
    {
        _audioSource.clip = _laserSound;
        if (Time.timeScale == 1)
        {
            _audioSource.pitch = 0.5f;
        }
        else
        {
            _audioSource.pitch = 0.3f;
        }
    }

    private IEnumerator ReadyFire()
    {
        yield return new WaitForSeconds(_fireRate);
        _canFire = true;
    }

    private void Move()
    {
        transform.Translate(_direction * _speed * Time.deltaTime);

        if (!_isExploding && transform.position.x < _enemyLeftBoundary)
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
            DamageSelf(_otherTag);
        }
        else if (_otherTag == _laserTag || _otherTag == _tripleShotTag)
        {
            Destroy(other.gameObject);
            DamageSelf(_otherTag);
        }
    }

    private void DamageSelf(string otherTag)
    {
        _health--;

        if (_health < 1 || otherTag == _playerTag || otherTag == _tripleShotTag)
        {
            _player.AddScore(10);
            DestroySelf();
        }
    }

    private void DestroySelf()
    {
        _isExploding = true;
        _collider.enabled = false;
        _animator.SetTrigger("OnEnemyDeath");
        StartCoroutine(RollPowerUpDrop());
        _audioManager.PlayExplosionSound();
        Destroy(this.gameObject, 2.7f);
    }

    private IEnumerator RollPowerUpDrop()
    {
        yield return new WaitForSeconds(0.25f);

        float randomFloat = Random.Range(0f, 1.0f);
        if (randomFloat <= _powerUpDropChance)
        {
            _spawnManager.SpawnPowerUp(this.transform.position);
        }
    }
}
