using System;
using System.Collections;
using UnityEngine;

public class ShieldGenerator : Damageable
{
    private const string _playerTag = "Player";
    private const string _laserTag = "Laser";
    private const string _tripleShotTag = "TripleShot";
    private const string _blastZoneTag = "BlastZone";
    private const string _missileTag = "Missile";
    private string _otherTag = string.Empty;
    private bool _noPower = false;

    private Player _player;
    private SpriteRenderer _renderer;
    private Color _defaultColor;
    private BoxCollider2D _collider;

    [SerializeField] private int _maxShieldPower = 25;
    [SerializeField] private int _shieldPower = 25;
    [SerializeField] private int _shieldGeneratorPowerLossDamage = 25;
    [SerializeField] Color _shieldGeneratorActiveColor;
    [SerializeField] Color _shieldGeneratorInactiveColor;
    private WaitForSeconds _shieldGeneratorPowerLossWaitForSeconds = new WaitForSeconds(0.4f);

    public static event Action<int> onShieldGeneratorDamage;
    public static event Action onShieldGeneratorPowerDepletion;

    private void OnEnable()
    {
        EnemyLeader.onShieldDepletion += ShieldGeneratorDepletion;
        EnemyLeader.onDefeat += NoPower;
        PowerCore.onPowerCoreRetracted += PowerUpShieldGenerators;
    }

    private void OnDisable()
    {
        EnemyLeader.onShieldDepletion -= ShieldGeneratorDepletion;
        EnemyLeader.onDefeat -= NoPower;
        PowerCore.onPowerCoreRetracted -= PowerUpShieldGenerators;
    }

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is null!");
        }
        _renderer = GetComponent<SpriteRenderer>();
        if (_renderer == null)
        {
            Debug.Log("SpriteRenderer is null!");
        }
        _collider = GetComponent<BoxCollider2D>();
        if (_collider == null)
        {
            Debug.Log("Collider is null!");
        }

        _defaultColor = _shieldGeneratorActiveColor;
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

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(_playerTag))
        {
            _player.Damage();
        }
    }
    private void Damage(int damage)
    {
        _shieldPower -= damage;
        StartCoroutine(DamageFlicker(_renderer, _defaultColor));
        onShieldGeneratorDamage?.Invoke(damage);

        if (_shieldPower <= 0)
        {
            _shieldPower = 0;
            ShieldGeneratorDepletion();
            onShieldGeneratorDamage?.Invoke(_shieldGeneratorPowerLossDamage);
        }
    }

    private void ShieldGeneratorDepletion()
    {
        _collider.enabled = false;
        onShieldGeneratorPowerDepletion?.Invoke();
        StartCoroutine(ShieldGeneratorPowerLoss());
    }

    private IEnumerator ShieldGeneratorPowerLoss()
    {
        yield return _shieldGeneratorPowerLossWaitForSeconds;
        _renderer.color = _shieldGeneratorInactiveColor;
    }

    private void PowerUpShieldGenerators()
    {
        if (!_noPower)
        {
            _renderer.color = _shieldGeneratorActiveColor;
            _shieldPower = _maxShieldPower;
            _collider.enabled = true; 
        }
    }

    private void NoPower()
    {
        _noPower = true;
    }
}
