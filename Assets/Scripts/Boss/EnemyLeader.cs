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
    private PolygonCollider2D _collider;

    [Header("Boundaries")]
    [SerializeField] private float _leftBoundary = 10.75f;
    [SerializeField] private float _rightBoundary = 11f;
    [SerializeField] private float _upperBoundary = 0.3f;
    [SerializeField] private float _lowerBoundary = -1f;

    [Header("Movement")]
    [SerializeField] private Vector2 _spawnPosition = new Vector2(22f, 0f);
    [SerializeField] private float _speed = 2f;
    private Vector2 _direction = Vector2.left;
    private bool _holdPosition = false;
    [SerializeField] private float _xHoldDirection = -0.05f;
    [SerializeField] private float _yDirection = 0.2f;

    [Header("Shields")]
    [SerializeField] private GameObject _shieldSprite;
    [SerializeField] private int _maxShields = 100;
    [SerializeField] private int _shields = 100;

    [Header("Health/Damage")]
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _health = 100;
    [SerializeField] private GameObject _explosion;
    [SerializeField] private List<GameObject> _damageEffectList;
    private WaitForSeconds _damageEffectWaitForSeconds = new WaitForSeconds(1f);
    private WaitForSeconds _midExplosionWaitForSeconds = new WaitForSeconds(0.5f);

    public static event Action onApproach;
    public static event Action onCommenceAttack;
    public static event Action<float, float> onShieldDamage;
    public static event Action<float, float> onShieldCharge;
    public static event Action onShieldDepletion;

    public static event Action<float, float> onHealthDamage;
    public static event Action onDefeat;

    private void OnEnable()
    {
        ShieldGenerator.onShieldGeneratorDamage += DamageShield;
        PowerCore.onPowerCoreRetracted += RaiseShields;
        PowerCore.onPowerCoreDamage += DamageHealth;
    }

    private void OnDisable()
    {
        ShieldGenerator.onShieldGeneratorDamage -= DamageShield;
        PowerCore.onPowerCoreRetracted -= RaiseShields;
        PowerCore.onPowerCoreDamage -= DamageHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is null!");
        }
        _collider = GetComponent<PolygonCollider2D>();
        if (_collider == null)
        {
            Debug.LogError("Collider is null!");
        }

        transform.position = _spawnPosition;

        onApproach?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
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
                Damage(2);
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
                Damage(4);
                break;
            case _blastZoneTag:
                Damage(4);
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

        if (_shields <= 0)
        {
            _shields = 0;
        }

        onShieldDamage?.Invoke(_shields, _maxShields);

        if (_shields == 0)
        {
            onShieldDepletion?.Invoke();
            StartCoroutine(ShieldFailure(_shieldSprite));
        }
    }

    private void DamageHealth(int damage)
    {
        _health -= damage;

        if (_health <= 0)
        {
            _health = 0;
        }

        onHealthDamage?.Invoke(_health, _maxHealth);

        if (_health == 0)
        {
            EnemyLeaderDefeated();
        }
    }

    private void RaiseShields()
    {
        if (_health > 0)
        {
            _shieldSprite.SetActive(true);
            _shields = _maxShields;
            onShieldCharge?.Invoke(_shields, _maxShields); 
        }
    }

    private void EnemyLeaderDefeated()
    {
        _collider.enabled = false;
        onDefeat?.Invoke();
        StartCoroutine(DestructionExplosions());
    }

    private IEnumerator DestructionExplosions()
    {
        int damageEffectCount = _damageEffectList.Count;

        for (int i = 0; i < damageEffectCount; i++)
        {
            yield return _damageEffectWaitForSeconds;

            int randomIndex = UnityEngine.Random.Range(0, _damageEffectList.Count);
            Instantiate(_explosion, _damageEffectList[randomIndex].transform.position, Quaternion.identity);
            StartCoroutine(DisplayDamageEffect(randomIndex));
        }
    }

    private IEnumerator DisplayDamageEffect(int index)
    {
        yield return _midExplosionWaitForSeconds;
        _damageEffectList[index].SetActive(true);
        _damageEffectList.RemoveAt(index);
    }
}
