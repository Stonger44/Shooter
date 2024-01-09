using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLeader : SpaceShip
{
    private const string _playerTag = "Player";
    private const string _laserTag = "Laser";
    private const string _tripleShotTag = "TripleShot";
    private const string _blastZoneTag = "BlastZone";
    private const string _missileTag = "Missile";
    private string _otherTag = string.Empty;

    private Player _player;

    [Header("Boundaries")]
    [SerializeField] private float _leftBoundary = 10.75f;
    [SerializeField] private float _rightBoundary = 11f;
    [SerializeField] private float _upperBoundary = 0.3f;
    [SerializeField] private float _lowerBoundary = -1f;

    [Header("Movement")]
    [SerializeField] private float _speed = 2f;
    private Vector2 _direction = Vector2.left;
    private bool _holdPosition = false;
    [SerializeField] private float _xDirection = -1f;
    [SerializeField] private float _xHoldDirection = -0.05f;
    [SerializeField] private float _yDirection = 0.2f;

    [Header("Shields")]
    [SerializeField] private GameObject _shieldSprite;
    [SerializeField] private int _maxShields = 100;
    [SerializeField] private int _shields = 100;

    [Header("Health")]
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _health = 100;

    [Header("Power Core")]
    [SerializeField] private GameObject _powerCore;
    [SerializeField] private float _xPowerCoreInternalPosition = -6.84f;
    [SerializeField] private float _xPowerCoreExposedPosition = -8.4f;
    [SerializeField] private float _powerCoreMovementSpeed = 1f;
    private WaitForSeconds _powerCoreExposureTime = new WaitForSeconds(8f);
    private Vector2 _powerCoreDirection;
    private bool _exposePowerCore = false;
    private bool _retractPowerCore = false;

    public static event Action onBossApproach;
    public static event Action onCommenceAttack;
    public static event Action<float, float> onShieldDamageTaken;
    public static event Action onShieldDepletion;

    public static event Action<float, float> onHealthDamageTaken;

    private void OnEnable()
    {
        ShieldGenerator.onShieldGeneratorDamage += DamageShield;
    }

    private void OnDisable()
    {
        ShieldGenerator.onShieldGeneratorDamage -= DamageShield;
    }

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is null!");
        }

        transform.position = new Vector2(22f, 0f);

        onBossApproach?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        if (_exposePowerCore || _retractPowerCore)
        {
            MovePowerCore();
        }
    }

    private void Move()
    {
        if (transform.position.y >= _upperBoundary || transform.position.y <= _lowerBoundary)
        {
            Change_Y_Direction();
        }

        if (_holdPosition && (transform.position.x <= _leftBoundary || transform.position.x >= _rightBoundary))
        {
            Change_X_Direction();
        }

        transform.Translate(_direction * _speed * Time.deltaTime);

        if (_holdPosition)
        {
            transform.position = new Vector2(Mathf.Clamp(transform.position.x, _leftBoundary, _rightBoundary), Mathf.Clamp(transform.position.y, _lowerBoundary, _upperBoundary));
        }
        else
        {
            CheckApproach();
        }
    }

    private void CheckApproach()
    {
        if (transform.position.x <= _rightBoundary)
        {
            float yDirection = UnityEngine.Random.value < 0.5f ? -_yDirection : _yDirection;
            _direction = new Vector2(_xHoldDirection, yDirection);

            _holdPosition = true;
            onCommenceAttack?.Invoke();
        }
    }

    private void Change_Y_Direction()
    {
        if (transform.position.y >= _upperBoundary)
        {
            _direction.y = -_yDirection;
        }
        else if (transform.position.y <= _lowerBoundary)
        {
            _direction.y = _yDirection;
        }
    }

    private void Change_X_Direction()
    {
        if (transform.position.x <= _leftBoundary)
        {
            _direction.x = -_xHoldDirection;
        }
        else if (transform.position.x >= _rightBoundary)
        {
            _direction.x = _xHoldDirection;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _otherTag = other.tag;
        switch (_otherTag)
        {
            case _playerTag:
                _player.Damage();
                Damage(5);
                break;
            case _laserTag:
                Destroy(other.gameObject);
                Damage(1);
                break;
            case _tripleShotTag:
                Destroy(other.gameObject);
                Damage(3);
                break;
            case _missileTag:
                MissilePlayer missilePlayer = other.gameObject.GetComponent<MissilePlayer>();
                if (missilePlayer != null)
                {
                    missilePlayer.DetonateMissile(this.gameObject);
                }
                Damage(5);
                break;
            case _blastZoneTag:
                Damage(10);
                break;
            default:
                break;
        }
    }

    private void Damage(int damage)
    {
        if (_shields > 0)
        {
            DamageShield(damage);
            return;
        }

        DamageHealth(damage);
    }

    private void DamageShield(int damage)
    {
        _shields -= damage;
        onShieldDamageTaken?.Invoke(_shields, _maxShields);

        if (_shields <= 0)
        {
            _shields = 0;
            onShieldDepletion?.Invoke();
            StartCoroutine(ShieldFailure(_shieldSprite));
            TriggerPowerCoreExposure();

        }
    }

    private void DamageHealth(int damage)
    {
        _health -= damage;
        onHealthDamageTaken?.Invoke(_health, _maxHealth);

        if (_health <= 0 )
        {
            _health = 0;
            Debug.Log("You Defeated!");
            // Trigger death
        }
    }

    private void MovePowerCore()
    {
        _powerCore.transform.Translate(_powerCoreDirection * _powerCoreMovementSpeed * Time.deltaTime);

        //_powerCore.transform.position = new Vector2(Mathf.Clamp(_powerCore.transform.position.x, _xPowerCoreExposedPosition, _xPowerCoreInternalPosition), _powerCore.transform.position.y);

        if (_exposePowerCore && _powerCore.transform.position.x <= _xPowerCoreExposedPosition)
        {
            _exposePowerCore = false;
        }
        //if (_retractPowerCore && _powerCore.transform.position.x == _xPowerCoreInternalPosition)
        //{
        //    _retractPowerCore = false;
        //}
    }

    private IEnumerator PowerCoreExposureCoolDown()
    {
        yield return _powerCoreExposureTime;
        _retractPowerCore = true;
        _powerCoreDirection = Vector2.down;
    }

    private void TriggerPowerCoreExposure()
    {
        _exposePowerCore = true;
        _powerCoreDirection = Vector2.up;
        // StartCoroutine(PowerCoreExposureCoolDown());
    }
}
