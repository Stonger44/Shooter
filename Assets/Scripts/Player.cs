using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : Damageable
{
    private const string _laserEnemyTag = "LaserEnemy";

    private GameManager _gameManager;
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private AudioManager _audioManager;
    private AudioSource _audioSource;
    private Camera _camera;

    [Header("Boundaries")]
    [SerializeField] private float _playerLeftBoundary = -9.7f;
    [SerializeField] private float _playerRightBoundary = 9.7f;
    [SerializeField] private float _playerUpperBoundary = 4.25f;
    [SerializeField] private float _playerLowerBoundary = -5.25f;

    #region PlayerWrap
    //private Vector3 _position;

    //[SerializeField] private float _playerLeftWrap = 10.5f;
    //[SerializeField] private float _playerRightWrap = -10.5f;
    //[SerializeField] private float _playerUpperWrap = 6f;
    //[SerializeField] private float _playerLowerWrap = -6f;
    #endregion

    private float _horizontalAxis;
    private float _verticalAxis;
    private Vector2 _direction;

    [Header("Speed")]
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _speedStandard = 5f;

    [Header("AfterBurner")]
    [SerializeField] private GameObject _thruster;
    [SerializeField] private GameObject _afterBurner;
    [SerializeField] private float _afterBurnerSpeedMultiplier = 2f;
    [SerializeField] private float _afterBurnerMaxActiveTime = 3f;
    [SerializeField] private float _afterBurnerTimeRemaining;
    [SerializeField] private float _afterBurnerDepletionRate = 1.25f;
    [SerializeField] private float _afterBurnerRechargeRate = 1f;
    [SerializeField] private float _afterBurnerCoolDownTime = 3f;
    [SerializeField] private bool _afterBurnerIsInCoolDown = false;

    [Header("Laser")]
    [SerializeField] private GameObject _laser;
    [SerializeField] private float _laserOffset = 0.57f;
    [SerializeField] private float _laserFireRate = 0.15f;
    [SerializeField] private bool _canFire = true;
    [SerializeField] private AudioClip _laserSound;
    private Vector2 _laserPosition;
    private float _fireRate;

    #region Cooldown System using Time.time
    // private float _fireReadyTime;
    #endregion

    [Header("Damage")]
    [SerializeField] private List<GameObject> _damageEffectList;
    private List<GameObject> _inactiveDamageEffectList = new List<GameObject>();
    private GameObject _activeDamageEffect;
    [SerializeField] private GameObject _deathExplosion;
    [SerializeField] private GameObject _engineDamage;
    [SerializeField] private float _engineDamageTime = 4f;
    private float _engineRepairTime;
    [SerializeField] private float _engineDamageSpeedMultiplier = 0.3f;
    [SerializeField] private bool _isEngineDamaged = false;
    private int _lives = 3;
    private Vector2 _deathPosition = new Vector2(-44f, 0f);
    private bool _isInvulnerable = false;
    private WaitForSeconds _invulnerableWaitForSeconds = new WaitForSeconds(0.5f);
    [SerializeField] private SpriteRenderer _renderer;
    private Color _defaultColor;
    [SerializeField] private SpriteRenderer _shieldRenderer;
    private Color _defaultShieldColor;

    [Header("TripleShot")]
    [SerializeField] private GameObject _tripleShot;
    [SerializeField] private float _tripleShotFireRate = 0.18f;
    [SerializeField] private int _tripleShotAmmo;
    [SerializeField] private int _tripleShotMaxAmmo = 15;
    private bool _isTripleShotActive = false;

    [Header("SpeedBoost")]
    [SerializeField] private float _speedBoostActiveTime = 3f;
    [SerializeField] private float _speedBoostTimeScale = 0.5f;
    [SerializeField] private float _speedBoostSpeed = 10f;
    private bool _isSpeedBoostActive = false;
    private float _speedBoostDeactivationTime;

    [Header("Shields")]
    [SerializeField] private GameObject _shield;
    [SerializeField] private int _shieldLevel = 0;

    [Header("SpaceBomb")]
    [SerializeField] private GameObject _spaceBomb;
    private Vector2 _spaceBombPosition;
    [SerializeField] private float _spaceBombOffset = 0.665f;
    [SerializeField] private float _spaceBombFireRate = 0.75f;
    [SerializeField] private int _spaceBombAmmo;
    [SerializeField] private int _spaceBombMaxAmmo = 3;
    private bool _canFireSpaceBomb = true;

    [Header("HomingMissile")]
    [SerializeField] private GameObject _homingMissile;
    private Vector2 _homingMissilePosition;
    [SerializeField] private float _homingMissileOffset = 0.665f;
    [SerializeField] private float _homingMissileFireRate = 0.75f;
    [SerializeField] private int _homingMissileAmmo;
    [SerializeField] private int _homingMissileMaxAmmo = 6;
    private bool _canFireHomingMissile = true;

    private bool _holdFire = false;

    public static event Action<GameObject> onAttractPowerUp;
    public static event Action onStopAttractingPowerUp;
    public static event Action onShieldDepletion;
    public static event Action onPlayerDeath;

    private void OnEnable()
    {
        EnemyLeader.onApproach += HoldFire;
        EnemyLeader.onCommenceAttack += WeaponsFree;
    }

    private void OnDisable()
    {
        EnemyLeader.onApproach -= HoldFire;
        EnemyLeader.onCommenceAttack -= WeaponsFree;
    }

    // Start is called before the first frame update
    void Start()
    {
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
        _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("UIManager is null!");
        }
        _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        if (_audioManager == null)
        {
            Debug.LogError("AudioManager is null!");
        }
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("Player AudioSource is null!");
        }
        _camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        if (_camera == null)
        {
            Debug.LogError("Camera is null!");
        }

        _defaultColor = _renderer.color;
        _defaultShieldColor = _shieldRenderer.color;
        _fireRate = _laserFireRate;
        _afterBurnerTimeRemaining = _afterBurnerMaxActiveTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (_lives > 0)
        {
            Move();

            AttractPowerUp();

            if (!_holdFire)
            {
                Fire();

                FireHomingMissile();

                FireSpaceBomb(); 
            }
        }
    }

    private void AttractPowerUp()
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            onAttractPowerUp?.Invoke(this.gameObject);
        }
        if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
        {
            onStopAttractingPowerUp?.Invoke();
        }
    }

    public int GetPlayerLives()
    {
        return _lives;
    }

    public void Damage()
    {
        if (!_isInvulnerable)
        {
            _isInvulnerable = true;
            StartCoroutine(InvulnerablilityCoolDown());

            if (_shieldLevel > 0)
            {
                _shieldLevel--;
                StartCoroutine(DamageFlicker(_shieldRenderer, _defaultShieldColor));
                if (_shieldLevel < 1)
                {
                    StartCoroutine(ShieldFailure(_shield));
                    onShieldDepletion?.Invoke();
                }
                _uiManager.UpdateShieldsUI(_shieldLevel);

                return;
            }

            _lives--;
            StartCoroutine(DamageFlicker(_renderer, _defaultColor));
            _audioManager.PlayExplosionSound();
            _uiManager.UpdateLivesUI(_lives);
            StartCoroutine(ShowPlayerDamage());

            if (_lives < 1)
            {
                _spawnManager.StopSpawning();
                DestroyPlayer();
            }
        }
    }

    private IEnumerator InvulnerablilityCoolDown()
    {
        yield return _invulnerableWaitForSeconds;
        _isInvulnerable = false;
    }

    public bool GetAfterBurnerCoolDown()
    {
        return _afterBurnerIsInCoolDown;
    }

    public void ActivateSpeedBoost()
    {
        Time.timeScale = _speedBoostTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        _afterBurnerDepletionRate /= 2;
        _afterBurnerRechargeRate /= 2;
        _afterBurnerCoolDownTime *= 2;

        _isSpeedBoostActive = true;

        _speedBoostDeactivationTime = Time.time + _speedBoostActiveTime;
        _uiManager.UpdateSpeedBoostBar(_speedBoostActiveTime, _speedBoostDeactivationTime);
    }

    public bool IsSpeedBoostActive()
    {
        return _isSpeedBoostActive;
    }

    public float GetSpeedBoostTimeScale()
    {
        return _speedBoostTimeScale;
    }

    public void ActivateTripleShot()
    {
        _isTripleShotActive = true;
        _fireRate = _tripleShotFireRate;
        _tripleShotAmmo = _tripleShotMaxAmmo;
        _uiManager.UpdateTripleShotAmmo(_tripleShotAmmo);
    }

    public void CollectHomingMissiles()
    {
        _homingMissileAmmo = _homingMissileMaxAmmo;
        _uiManager.UpdateHomingMissileAmmo(_homingMissileAmmo);
    }

    public void CollectSpaceBomb()
    {
        if (_spaceBombAmmo < _spaceBombMaxAmmo)
        {
            _spaceBombAmmo++;
            _uiManager.UpdateSpaceBombAmmo(_spaceBombAmmo);
        }
    }

    public void ActivateShields()
    {
        if (_shieldLevel < 3)
        {
            _shieldLevel++;
            _shield.SetActive(true);
            _uiManager.UpdateShieldsUI(_shieldLevel);
        }
    }

    public void CollectPlayerLife()
    {
        if (_lives < 3)
        {
            _lives++;
            _uiManager.UpdateLivesUI(_lives);

            _activeDamageEffect = _damageEffectList.FirstOrDefault(dmgEfct => dmgEfct.activeInHierarchy == true);
            if (_activeDamageEffect != null)
            {
                _activeDamageEffect.SetActive(false);
            }
        }
    }

    public void DetonateSlowBomb()
    {
        Damage();
        _isEngineDamaged = true;
        _engineDamage.SetActive(true);
        _engineRepairTime = Time.time + _engineDamageTime;
    }

    private void Move()
    {
        _horizontalAxis = Input.GetAxis("Horizontal");
        _verticalAxis = Input.GetAxis("Vertical");

        _direction.x = _horizontalAxis;
        _direction.y = _verticalAxis;

        #region PlayerWrap
        //_position = this.transform.position;

        //if (this.transform.position.x > _playerLeftWrap)
        //{
        //    _position.x = _playerRightWrap;
        //}
        //else if (this.transform.position.x < _playerRightWrap)
        //{
        //    _position.x = _playerLeftWrap;
        //}

        //if (this.transform.position.y > _playerUpperWrap)
        //{
        //    _position.y = _playerLowerWrap;
        //}
        //else if (this.transform.position.y < _playerLowerWrap)
        //{
        //    _position.y = _playerUpperWrap;
        //}

        //this.transform.position = _position;
        #endregion

        if (_isSpeedBoostActive)
        {
            CheckSpeedBoostTime(); 
        }

        if (!_isSpeedBoostActive)
        {
            _speed = _speedStandard;
        }
        else
        {
            _speed = _speedBoostSpeed;
        }

        CheckAfterBurner();

        if (_isEngineDamaged)
        {
            CheckEngineDamage();
        }

        transform.Translate(_direction * _speed * Time.deltaTime);

        transform.position = new Vector2(Mathf.Clamp(transform.position.x, _playerLeftBoundary, _playerRightBoundary), Mathf.Clamp(transform.position.y, _playerLowerBoundary, _playerUpperBoundary));
    }

    private void CheckEngineDamage()
    {
        if (Time.time > _engineRepairTime)
        {
            _isEngineDamaged = false;
            _engineDamage.SetActive(false);
        }
        else
        {
            _speed *= _engineDamageSpeedMultiplier;
        }
    }

    private void CheckSpeedBoostTime()
    {
        if (Time.time > _speedBoostDeactivationTime)
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            _afterBurnerDepletionRate *= 2;
            _afterBurnerRechargeRate *= 2;
            _afterBurnerCoolDownTime /= 2;
            
            _isSpeedBoostActive = false;
        }
        _uiManager.UpdateSpeedBoostBar(_speedBoostActiveTime, _speedBoostDeactivationTime);
    }

    private void CheckAfterBurner()
    {
        if (!_afterBurnerIsInCoolDown)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _afterBurnerTimeRemaining -= _afterBurnerDepletionRate * Time.deltaTime;

                if (_afterBurnerTimeRemaining <= 0)
                {
                    _afterBurnerTimeRemaining = 0;
                    _afterBurnerIsInCoolDown = true;
                    StartCoroutine(AfterBurnerCoolDown());
                    StartCoroutine(_uiManager.AfterBurnerCoolDown());
                }

                if (_afterBurnerTimeRemaining > 0)
                {
                    _thruster.SetActive(false);
                    _afterBurner.SetActive(true);

                    _speed *= _afterBurnerSpeedMultiplier;
                }
                else
                {
                    _thruster.SetActive(true);
                    _afterBurner.SetActive(false);
                }
            }
            else
            {
                _afterBurnerTimeRemaining += _afterBurnerRechargeRate * Time.deltaTime;

                if (_afterBurnerTimeRemaining >= _afterBurnerMaxActiveTime)
                {
                    _afterBurnerTimeRemaining = _afterBurnerMaxActiveTime;
                }

                _thruster.SetActive(true);
                _afterBurner.SetActive(false);
            } 
        }
        _uiManager.UpdateAfterBurnerBar(_afterBurnerTimeRemaining, _afterBurnerMaxActiveTime);
    }

    private IEnumerator AfterBurnerCoolDown()
    {
        yield return new WaitForSeconds(_afterBurnerCoolDownTime);
        _afterBurnerIsInCoolDown = false;
    }

    private void Fire()
    {
        #region Cooldown System using Time.time
        //if (Input.GetKeyDown(KeyCode.Space) && (Time.time > _fireReadyTime))
        //{
        //    _laserPosition = this.transform.position;
        //    _laserPosition.x += _laserOffset;

        //    Instantiate(_laser, _laserPosition, _laserRotation);
        //    _fireReadyTime = Time.time + _fireRate;
        //} 
        #endregion

        if (Input.GetKeyDown(KeyCode.Return) && _canFire)
        {
            _laserPosition = transform.position;
            _laserPosition.x += _laserOffset;

            if (_isTripleShotActive)
            {
                Instantiate(_tripleShot, _laserPosition, Quaternion.identity);
                CheckTripleShotAmmo();
            }
            else
            {
                Instantiate(_laser, _laserPosition, Quaternion.identity);
            }

            SetLaserSound(_isTripleShotActive, _speed);
            _audioSource.Play();

            _canFire = false;
            StartCoroutine(ReadyFire());
        }
    }

    private IEnumerator ReadyFire()
    {
        yield return new WaitForSeconds(_fireRate);
        _canFire = true;
    }

    private void CheckTripleShotAmmo()
    {
        _tripleShotAmmo--;
        _uiManager.UpdateTripleShotAmmo(_tripleShotAmmo);

        if (_tripleShotAmmo < 1)
        {
            _isTripleShotActive = false;
            _fireRate = _laserFireRate;
        }
    }

    private void SetLaserSound(bool isTripleShotActive, float speed)
    {
        _audioSource.clip = _laserSound;
        if (isTripleShotActive)
        {
            if (_speed == _speedStandard)
            {
                _audioSource.pitch = 0.75f;
            }
            else
            {
                _audioSource.pitch = 0.55f;
            }
        }
        else
        {
            if (_speed == _speedStandard)
            {
                _audioSource.pitch = 1f;
            }
            else
            {
                _audioSource.pitch = 0.8f;
            }
        }
    }

    private void FireHomingMissile()
    {   
        if (_canFireHomingMissile && _homingMissileAmmo > 0 && Input.GetKeyDown(KeyCode.RightShift))
        {
            _homingMissileAmmo--;
            _uiManager.UpdateHomingMissileAmmo(_homingMissileAmmo);

            _homingMissilePosition = transform.position;
            _homingMissilePosition.x += _homingMissileOffset;
            Instantiate(_homingMissile, _homingMissilePosition, Quaternion.identity);

            _canFireHomingMissile = false;
            StartCoroutine(ReadyFireHomingMissile());
        }
    }

    private IEnumerator ReadyFireHomingMissile()
    {
        yield return new WaitForSeconds(_homingMissileFireRate);
        _canFireHomingMissile = true;
    }

    private void FireSpaceBomb()
    {
        if (_canFireSpaceBomb && _spaceBombAmmo > 0 && Input.GetKeyDown(KeyCode.Space))
        {
            _spaceBombAmmo--;
            _uiManager.UpdateSpaceBombAmmo(_spaceBombAmmo);

            _spaceBombPosition = transform.position;
            _spaceBombPosition.x += _spaceBombOffset;
            Instantiate(_spaceBomb, _spaceBombPosition, Quaternion.identity);

            _canFireSpaceBomb = false;
            StartCoroutine(ReadyFireSpaceBomb());
        }
    }

    private IEnumerator ReadyFireSpaceBomb()
    {
        yield return new WaitForSeconds(_spaceBombFireRate);
        _canFireSpaceBomb = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == _laserEnemyTag)
        {
            Damage();
            Destroy(other.gameObject);
        }
    }

    private IEnumerator ShowPlayerDamage()
    {
        yield return new WaitForSeconds(0.12f);

        _inactiveDamageEffectList = _damageEffectList.Where(dmgEfct => dmgEfct.activeInHierarchy == false).ToList();
        int randomIndex = UnityEngine.Random.Range(0, _inactiveDamageEffectList.Count);
        _inactiveDamageEffectList[randomIndex].SetActive(true);

        StartCoroutine(_camera.CameraShake());
    }

    private void DestroyPlayer()
    {
        Instantiate(_deathExplosion, this.transform.position, Quaternion.Euler(0, 0, 90));
        _audioManager.PlayExplosionSound();
        _gameManager.PauseBGM();
        this.transform.position = _deathPosition;
        onPlayerDeath?.Invoke();
    }

    private void HoldFire()
    {
        _holdFire = true;
    }

    private void WeaponsFree()
    {
        _holdFire = false;
    }
}
