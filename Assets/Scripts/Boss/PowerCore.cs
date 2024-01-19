using System;
using System.Collections;
using UnityEngine;

public class PowerCore : MonoBehaviour
{
    private const string _playerTag = "Player";
    private const string _laserTag = "Laser";
    private const string _tripleShotTag = "TripleShot";
    private const string _blastZoneTag = "BlastZone";
    private const string _missileTag = "Missile";
    private string _otherTag = string.Empty;

    private Player _player;
    private BoxCollider2D _collider;

    [SerializeField] private GameObject _internalPosition;
    [SerializeField] private GameObject _exposedPosition;
    [SerializeField] private float _movementSpeed = 1.5f;
    private float _powerCoreExposureTime = 8f;
    private float _powerCoreRetractionReadyTime;
    private bool _exposePowerCore = false;
    private bool _retractPowerCore = false;
    private bool _noPower = false;

    [Header("Health")]
    [SerializeField] private int _maxHealth = 15;
    [SerializeField] private int _health = 15;
    [SerializeField] private int _powerCoreDepletionDamage = 20;
    [SerializeField] private GameObject _explosion;
    [SerializeField] private float _midExplosionTime = 0.5f;
    private WaitForSeconds _midExplosionWaitForSeconds;

    public static event Action onPowerCoreRetracted;
    public static event Action onPowerCoreStartMovement;
    public static event Action onPowerCoreStopMovement;

    public static event Action<int> onPowerCoreDamage;
    public static event Action onPowerCoreDepletion;
    public static event Action onPowerCoreDestruction;
    public static event Action<Vector2> onPowerCorePowerUpDrop;

    private void OnEnable()
    {
        EnemyLeader.onShieldDepletion += TriggerPowerCoreExposure;
        EnemyLeader.onEnemyLeaderDefeat += DestroyPowerCore;
    }

    private void OnDisable()
    {
        EnemyLeader.onShieldDepletion -= TriggerPowerCoreExposure;
        EnemyLeader.onEnemyLeaderDefeat -= DestroyPowerCore;
    }

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is null!");
        }
        _collider = GetComponent<BoxCollider2D>();
        if (_collider == null)
        {
            Debug.Log("Collider is null!");
        }

        transform.position = _internalPosition.transform.position;
        _collider.enabled = false;
        _midExplosionWaitForSeconds = new WaitForSeconds(_midExplosionTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (_exposePowerCore)
        {
            ExposePowerCore();
        }

        if (_powerCoreRetractionReadyTime > 0 && Time.time > _powerCoreRetractionReadyTime)
        {
            TriggerPowerCoreRetraction();
        }

        if (_retractPowerCore)
        {
            RetractPowerCore();
        }
    }

    private void TriggerPowerCoreExposure()
    {
        _exposePowerCore = true;
        _powerCoreRetractionReadyTime = Time.time + _powerCoreExposureTime;
        onPowerCoreStartMovement?.Invoke();
    }

    private void TriggerPowerCoreRetraction()
    {
        _retractPowerCore = true;
        _powerCoreRetractionReadyTime = 0;
        onPowerCoreStartMovement?.Invoke();
    }

    private void ExposePowerCore()
    {
        transform.position = Vector2.MoveTowards(transform.position, _exposedPosition.transform.position, _movementSpeed * Time.deltaTime);

        if (transform.position == _exposedPosition.transform.position)
        {
            _exposePowerCore = false;
            _collider.enabled = true;
            onPowerCoreStopMovement?.Invoke();
        }
    }

    private void RetractPowerCore()
    {
        transform.position = Vector2.MoveTowards(transform.position, _internalPosition.transform.position, _movementSpeed * Time.deltaTime);

        if (!_noPower && transform.position == _internalPosition.transform.position)
        {
            _retractPowerCore = false;
            _collider.enabled = false;
            _health = _maxHealth;
            onPowerCoreRetracted?.Invoke();
            onPowerCoreStopMovement?.Invoke();
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
        _health -= damage;
        onPowerCoreDamage?.Invoke(damage);

        if (_health <= 0)
        {
            _health = 0;
            PowerCoreDepletion();
            onPowerCoreDamage?.Invoke(_powerCoreDepletionDamage);
        }
    }

    private void PowerCoreDepletion()
    {
        _collider.enabled = false;
        Instantiate(_explosion, transform.position, Quaternion.identity);
        _powerCoreRetractionReadyTime = Time.time;
        onPowerCoreDepletion?.Invoke();
        StartCoroutine(DropPowerup());
    }

    private IEnumerator DropPowerup()
    {
        yield return _midExplosionWaitForSeconds;
        onPowerCorePowerUpDrop?.Invoke(transform.position);
    }

    private void DestroyPowerCore()
    {
        _noPower = true;
        StartCoroutine(PowerCoreExplosion());
    }

    private IEnumerator PowerCoreExplosion()
    {
        _collider.enabled = false;
        Instantiate(_explosion, transform.position, Quaternion.identity);
        onPowerCoreDestruction?.Invoke();
        yield return _midExplosionWaitForSeconds;
        this.gameObject.SetActive(false);
    }
}
