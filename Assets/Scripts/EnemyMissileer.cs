﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMissileer : MonoBehaviour
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
    private PolygonCollider2D _collider;

    [SerializeField] private GameObject _explosion;
    [SerializeField] private SpriteRenderer _renderer;

    [Header("Boundaries")]
    [SerializeField] private float _enemyLeftBoundary = -11.2f;
    [SerializeField] private float _enemyRightBoundary = 11f;
    [SerializeField] private float _enemyUpperBoundary = 4.15f;
    [SerializeField] private float _enemyLowerBoundary = -5.15f;

    [Header("Movement")]
    [SerializeField] private float _speed = 4f;
    private Vector2 _position;
    private Vector2 _direction = Vector2.left;
    private bool _willStrafe;
    private bool _isStrafing;

    [Header("Health/Damage")]
    [SerializeField] private int _maxHealth = 3;
    [SerializeField] private int _health = 3;
    [SerializeField] private int _pointsOnDeath = 100;
    [SerializeField] private int _pointsOnBoundary = -10;
    private bool _isExploding;
    [SerializeField] private float _powerUpDropChance = 0.15f;

    [Header("Targeting System")]
    [SerializeField] private float _xRayCastOffset = -3.5f;
    [SerializeField] private float _yRayCastOffset = 0.558f;
    [SerializeField] private float _rayCastDistance = 16f;
    private Vector2 _rayCastOrigin;

    [Header("Missile")]
    [SerializeField] private GameObject _missile;
    [SerializeField] private float _xMissileOffset = -1.177f;
    [SerializeField] private float _yMissileOffset = -1.177f;
    [SerializeField] private float _fireRate = 3f;
    private WaitForSeconds _fireDelay = new WaitForSeconds(1f);
    private bool _canFire = true;

    [Header("Thrusters")]
    [SerializeField] private GameObject _thrusters;

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
        _collider = GetComponent<PolygonCollider2D>();
        if (_collider == null)
        {
            Debug.LogError("Collider is null!");
        }
        if (_explosion == null)
        {
            Debug.LogError("Explosion is null!");
        }
        _renderer = GetComponent<SpriteRenderer>();
        if (_renderer == null)
        {
            Debug.LogError("Renderer is null!");
        }

        Warp();
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        if (!_isExploding)
        {
            ScanForTarget();
        }
    }

    private void ScanForTarget()
    {
        _rayCastOrigin = transform.position;
        _rayCastOrigin.x += _xRayCastOffset;

        RaycastHit2D hitObject = Physics2D.Raycast(_rayCastOrigin, Vector2.left, _rayCastDistance);
        //Debug.DrawRay(_rayCastOrigin, Vector2.left * _rayCastDistance, Color.green);

        if (hitObject.collider != null)
        {
            if (hitObject.collider.tag == _playerTag)
            {
                if (_canFire)
                {
                    _canFire = false;
                    StartCoroutine(Fire());
                }
            }

        }
    }

    private IEnumerator Fire()
    {
        yield return _fireDelay;

        if (!_isExploding)
        {
            FireMissile(_xMissileOffset, _yMissileOffset);
            yield return _fireDelay;
            FireMissile(_xMissileOffset, -_yMissileOffset);
        }

        StartCoroutine(ReadyFire());
    }

    private void FireMissile(float xOffset, float yOffset)
    {
        Vector2 missilePosition = transform.position;
        missilePosition.x += xOffset;
        missilePosition.y += yOffset;
        Instantiate(_missile, missilePosition, Quaternion.identity);
    }

    private IEnumerator ReadyFire()
    {
        yield return new WaitForSeconds(_fireRate);
        _canFire = true;
    }

    private void Move()
    {
        if (!_isExploding && !_isStrafing)
        {
            _willStrafe = Random.value < (0.2f * Time.deltaTime);

            if (_willStrafe)
            {
                Strafe();
            }
        }

        transform.Translate(_direction * _speed * Time.deltaTime);

        if (!_isExploding)
        {
            transform.position = new Vector2(transform.position.x, Mathf.Clamp(transform.position.y, _enemyLowerBoundary, _enemyUpperBoundary));
        }

        if (!_isExploding && transform.position.x < _enemyLeftBoundary)
        {
            if (!_gameManager.IsGameOver())
            {
                _gameManager.UpdateScore(_pointsOnBoundary);
            }
            Warp();
        }
    }

    private void Strafe()
    {
        float randomY = Random.value < 0.5f ? -1 : 1;
        _direction = new Vector2(_direction.x, randomY);
        _isStrafing = true;
        StartCoroutine(StrafeDuration());
    }

    private IEnumerator StrafeDuration()
    {
        yield return new WaitForSeconds(1f);
        if (!_isExploding)
        {
            _direction = Vector2.left;
            _isStrafing = false;
        }
    }

    private void Warp()
    {
        float yPosition = Random.Range(_enemyLowerBoundary, _enemyUpperBoundary);
        _position = new Vector2(_enemyRightBoundary, yPosition);
        transform.position = _position;
        _health = _maxHealth;
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
        if (otherTag == _tripleShotTag)
        {
            _health -= 3;
        }
        else
        {
            _health--;
        }

        if (_health < 1 || otherTag == _playerTag || otherTag == _blastZoneTag)
        {
            DestroySelf();
        }
    }

    private void DestroySelf()
    {
        _isExploding = true;
        _collider.enabled = false;
        _explosion.SetActive(true);
        StartCoroutine(DisableThrustersAndRollPowerUp());
        _audioManager.PlayExplosionSound();
        Destroy(this.gameObject, 2.7f);
    }

    private IEnumerator DisableThrustersAndRollPowerUp()
    {
        yield return new WaitForSeconds(0.25f);

        _gameManager.UpdateScore(_pointsOnDeath);
        _gameManager.UpdateEnemyCount();
        _thrusters.SetActive(false);
        _renderer.enabled = false;

        if (Random.value < _powerUpDropChance)
        {
            _spawnManager.SpawnPowerUp(this.transform.position);
        }
    }
}