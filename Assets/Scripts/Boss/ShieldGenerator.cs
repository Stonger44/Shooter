using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldGenerator : MonoBehaviour
{
    private const string _playerTag = "Player";
    private const string _laserTag = "Laser";
    private const string _tripleShotTag = "TripleShot";
    private const string _blastZoneTag = "BlastZone";
    private const string _missileTag = "Missile";
    private string _otherTag = string.Empty;

    private Player _player;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _collider;

    [SerializeField] private int _shieldPower = 25;
    [SerializeField] private int _shieldGeneratorPowerLossDamage = 50;
    [SerializeField] Color _shieldGeneratorActiveColor;
    [SerializeField] Color _shieldGeneratorInactiveColor;

    public static event Action<int> onShieldGeneratorPowerDepletion;

    private void OnEnable()
    {
        EnemyLeader.onShieldDepletion += ShieldGeneratorDepletion;
    }

    private void OnDisable()
    {
        EnemyLeader.onShieldDepletion -= ShieldGeneratorDepletion;
    }

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is null!");
        }
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            Debug.Log("SpriteRenderer is null!");
        }
        _collider = GetComponent<BoxCollider2D>();
        if (_collider == null)
        {
            Debug.Log("Collider is null!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
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
        _shieldPower -= damage;

        if (_shieldPower <= 0)
        {
            _shieldPower = 0;
            ShieldGeneratorDepletion();
            onShieldGeneratorPowerDepletion?.Invoke(_shieldGeneratorPowerLossDamage);
        }
    }

    private void ShieldGeneratorDepletion()
    {
        _collider.enabled = false;
        _spriteRenderer.color = _shieldGeneratorInactiveColor;
    }
}
