using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGrunt : MonoBehaviour
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
    private AudioSource _audioSource;
    private Animator _animator;
    private CircleCollider2D _collider;

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
    [SerializeField] private int _health = 3;
    [SerializeField] private int _pointsOnDeath = 100;
    [SerializeField] private int _pointsOnBoundary = -10;
    private bool _isExploding;
    [SerializeField] private float _powerUpDropChance = 0.15f;

    [Header("Targeting System")]
    [SerializeField] private float _rayCastOffset = -3.5f;
    [SerializeField] private float _rayCastDistance = 16f;
    private Vector2 _rayCastOrigin;

    [Header("Laser")]
    [SerializeField] private GameObject _laserShot;
    [SerializeField] private AudioClip _laserSound;
    [SerializeField] private float _laserShotOffset = -0.955f;
    [SerializeField] private float _initialFireDelay = 0.75f;
    [SerializeField] private float _fireRate = 3f;
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

        if (!_isExploding)
        {
            ScanForTarget();
        }
    }

    private void ScanForTarget()
    {
        _rayCastOrigin = transform.position;
        _rayCastOrigin.x += _rayCastOffset;

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
        yield return new WaitForSeconds(0.5f);

        Vector2 laserPosition = transform.position;
        laserPosition.x += _laserShotOffset;
        Instantiate(_laserShot, laserPosition, Quaternion.identity);
        
        SetLaserSound();
        _audioSource.Play();

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
        _health = 3;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _otherTag = other.tag;

        switch (_otherTag)
        {
            case _playerTag:
                _player.Damage();
                DamageSelf(_otherTag);
                break;
            case _laserTag:
            case _tripleShotTag:
                Destroy(other.gameObject);
                DamageSelf(_otherTag);
                break;
            case _blastZoneTag:
                DamageSelf(_otherTag);
                break;
            default:
                break;
        }
    }

    private void DamageSelf(string otherTag)
    {
        _health--;

        if (_health < 1 || otherTag == _playerTag || otherTag == _tripleShotTag || otherTag == _blastZoneTag)
        {
            DestroySelf();
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

        if (Random.value < _powerUpDropChance)
        {
            _spawnManager.SpawnPowerUp(this.transform.position);
        }
    }
}
