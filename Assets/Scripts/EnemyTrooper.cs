﻿using System.Collections;
using UnityEngine;

public class EnemyTrooper : Damageable
{
    private const string _playerTag = "Player";
    private const string _powerUpTag = "PowerUp";
    private const string _laserTag = "Laser";
    private const string _tripleShotTag = "TripleShot";
    private const string _blastZoneTag = "BlastZone";
    private const string _missileTag = "Missile";
    private string _otherTag = string.Empty;

    private Player _player;
    private GameManager _gameManager;
    private SpawnManager _spawnManager;
    private AudioManager _audioManager;
    private CircleCollider2D _collider;
    private Animator _animator;
    private AudioSource _audioSource;

    [Header("Boundaries")]
    [SerializeField] private float _enemyLeftBoundary = -11.2f;
    [SerializeField] private float _enemyRightBoundary = 11f;
    [SerializeField] private float _enemyUpperBoundary = 4.15f;
    [SerializeField] private float _enemyLowerBoundary = -5.15f;

    [Header("Movement")]
    [SerializeField] private float _standardSpeed = 3f;
    [SerializeField] private float _rammingSpeed = 6f;
    [SerializeField] private float _jinkingSpeed = 6f;
    private float _speed;
    private Vector2 _position;
    private Vector2 _direction = Vector2.left;
    private bool _shouldJink;
    private bool _isJinking;
    private bool _isRamming;

    [Header("Thrusters")]
    [SerializeField] private GameObject _thrusters;
    [SerializeField] private GameObject _afterBurner;

    [Header("Health/Damage")]
    [SerializeField] private int _maxHealth = 1;
    [SerializeField] private int _health = 1;
    [SerializeField] private int _maxShields = 2;
    [SerializeField] private int _shieldLevel = 2;
    [SerializeField] private GameObject _shield;
    [SerializeField] private int _pointsOnDeath = 100;
    [SerializeField] private int _pointsOnBoundary = -10;
    private bool _isExploding;
    [SerializeField] private float _powerUpDropChance = 0.5f;
    [SerializeField] private SpriteRenderer _renderer;
    private Color _defaultColor;


    [Header("Targeting System")]
    [SerializeField] private float _xRayCastOffset = -3.5f;
    [SerializeField] private float _rayCastDistance = 16f;
    private Vector2 _rayCastOrigin;
    [SerializeField] private LayerMask _layerMask;

    [Header("Laser")]
    [SerializeField] private GameObject _laser;
    [SerializeField] private AudioClip _laserSound;
    [SerializeField] private float _xLaserOffset = -0.811f;
    private WaitForSeconds _fireDelay = new WaitForSeconds(1f);
    [SerializeField] private float _fireRate = 3f;
    private bool _canFire = true;

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
            Debug.LogError("Collider is null!");
        }
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Animator is null!");
        }
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("AudioSource is null!");
        }

        _defaultColor = _renderer.color;
        _speed = _standardSpeed;

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

    public void IsRamming(bool isRamming)
    {
        _isRamming = isRamming;

        if (!_isExploding && !_isRamming)
        {
            _direction = Vector2.left;
            _speed = _standardSpeed;
            _thrusters.SetActive(true);
            _afterBurner.SetActive(false);
        }
    }

    public void ShouldJink()
    {
        _shouldJink = true;
    }

    private void Move()
    {
        if (!_isExploding && _isRamming)
        {
            PrepareForRammingSpeed();
        }

        if (_shouldJink && !_isExploding && !_isRamming && !_isJinking)
        {
            Jink();
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

    private void PrepareForRammingSpeed()
    {
        _direction = (Vector2)_player.transform.position - (Vector2)transform.position;
        _direction.Normalize();
        _speed = _rammingSpeed;
        _thrusters.SetActive(false);
        _afterBurner.SetActive(true);
    }

    private void Jink()
    {
        float randomY = Random.value < 0.5f ? -1 : 1;
        _direction = new Vector2(_direction.x/2, randomY);
        _isJinking = true;
        _speed = _jinkingSpeed;
        _thrusters.SetActive(false);
        _afterBurner.SetActive(true);
        StartCoroutine(JinkDuration());
    }

    private IEnumerator JinkDuration()
    {
        yield return new WaitForSeconds(0.5f);
        if (!_isExploding)
        {
            _direction = Vector2.left;
            _speed = _standardSpeed;
            _thrusters.SetActive(true);
            _afterBurner.SetActive(false);
        }
        yield return new WaitForSeconds(2f);
        _shouldJink = false;
        _isJinking = false;
    }

    private void Warp()
    {
        float yPosition = Random.Range(_enemyLowerBoundary, _enemyUpperBoundary);
        _position = new Vector2(_enemyRightBoundary, yPosition);
        transform.position = _position;
        _health = _maxHealth;
        _shieldLevel = _maxShields;
        _shield.SetActive(true);
    }

    private void ScanForTarget()
    {
        _rayCastOrigin = transform.position;
        _rayCastOrigin.x += _xRayCastOffset;

        RaycastHit2D hitObject = Physics2D.Raycast(_rayCastOrigin, Vector2.left, _rayCastDistance, _layerMask);
        // Debug.DrawRay(_rayCastOrigin, Vector2.left * _rayCastDistance, Color.green);

        if (hitObject.collider?.tag == _playerTag || hitObject.collider?.tag == _powerUpTag)
        {
            if (_canFire)
            {
                _canFire = false;
                StartCoroutine(Fire());
            }
        }
    }

    private IEnumerator Fire()
    {
        yield return _fireDelay;

        if (!_isExploding)
        {
            Vector2 laserPosition = transform.position;
            laserPosition.x += _xLaserOffset;
            Instantiate(_laser, laserPosition, Quaternion.identity);

            SetLaserSound();
            _audioSource.Play(); 
        }

        StartCoroutine(ReadyFire());
    }

    private IEnumerator ReadyFire()
    {
        yield return new WaitForSeconds(_fireRate);
        _canFire = true;
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
            case _missileTag:
                MissilePlayer missilePlayer = other.gameObject.GetComponent<MissilePlayer>();
                if (missilePlayer != null)
                {
                    missilePlayer.DetonateMissile(this.gameObject);
                }
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
        if (otherTag == _playerTag || otherTag == _tripleShotTag || otherTag == _missileTag || otherTag == _blastZoneTag)
        {
            if (_shieldLevel > 0)
            {
                _shieldLevel = 0;
                StartCoroutine(ShieldFailure(_shield));
            }
            _health = 0;
            DestroySelf();
            return;
        }

        if (_shieldLevel > 0)
        {
            _shieldLevel--;

            if (_shieldLevel < 1)
            {
                StartCoroutine(ShieldFailure(_shield));
            }
        }
        else
        {
            _health--;
            StartCoroutine(DamageFlicker(_renderer, _defaultColor));

            if (_health < 1)
            {
                DestroySelf();
            }
        }
    }

    private void DestroySelf()
    {
        _isExploding = true;
        _collider.enabled = false;
        _animator.SetTrigger("OnEnemyDeath");
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
        _afterBurner.SetActive(false);

        if (Random.value < _powerUpDropChance)
        {
            _spawnManager.SpawnPowerUp(this.transform.position);
        }
    }
}
