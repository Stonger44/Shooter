﻿using System;
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

    [SerializeField] private GameObject _internalPosition;
    [SerializeField] private GameObject _exposedPosition;
    [SerializeField] private float _movementSpeed = 1f;
    private float _powerCoreExposureTime = 8f;
    private float _powerCoreRetractionReadyTime;
    private bool _exposePowerCore = false;
    private bool _retractPowerCore = false;

    [Header("Health")]
    [SerializeField] private int _maxHealth = 25;
    [SerializeField] private int _health = 25;
    [SerializeField] private int _powerCoreDepletionDamage = 30;

    public static event Action onPowerCoreRetracted;
    public static event Action onPowerCoreStartMovement;
    public static event Action onPowerCoreStopMovement;

    public static event Action<int> onPowerCoreDamage;

    private void OnEnable()
    {
        EnemyLeader.onShieldDepletion += TriggerPowerCoreExposure;
    }

    private void OnDisable()
    {
        EnemyLeader.onShieldDepletion -= TriggerPowerCoreExposure;
    }

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is null!");
        }

        transform.position = _internalPosition.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (_exposePowerCore)
        {
            ExposePowerCore();
        }

        if (_retractPowerCore)
        {
            RetractPowerCore();
        }

        if (_powerCoreRetractionReadyTime > 0 && Time.time > _powerCoreRetractionReadyTime)
        {
            TriggerPowerCoreRetraction();
        }
    }

    private void TriggerPowerCoreRetraction()
    {
        _retractPowerCore = true;
        _powerCoreRetractionReadyTime = 0;
        onPowerCoreStartMovement?.Invoke();
    }

    private void TriggerPowerCoreExposure()
    {
        _exposePowerCore = true;
        _powerCoreRetractionReadyTime = Time.time + _powerCoreExposureTime;
        onPowerCoreStartMovement?.Invoke();
    }

    private void ExposePowerCore()
    {
        transform.position = Vector2.MoveTowards(transform.position, _exposedPosition.transform.position, _movementSpeed * Time.deltaTime);

        if (transform.position == _exposedPosition.transform.position)
        {
            _exposePowerCore = false;
            onPowerCoreStopMovement?.Invoke();
        }
    }

    private void RetractPowerCore()
    {
        transform.position = Vector2.MoveTowards(transform.position, _internalPosition.transform.position, _movementSpeed * Time.deltaTime);

        if (transform.position == _internalPosition.transform.position)
        {
            _retractPowerCore = false;
            onPowerCoreRetracted?.Invoke();
            onPowerCoreStopMovement?.Invoke();
            _health = _maxHealth;
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
        // Explosion
        // Drop Powerup
        // Retract PowerCore
        _powerCoreRetractionReadyTime = Time.time;
        Debug.Log("PowerCore damaged, initiating retraction!");
    }
}
