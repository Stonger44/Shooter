using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _playerLeftBoundary = 9.4f;
    [SerializeField] private float _playerRightBoundary = -9.4f;
    [SerializeField] private float _playerUpperBoundary = 5.1f;
    [SerializeField] private float _playerLowerBoundary = -5.1f;

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
    [SerializeField] private float _speed = 12f;
    [SerializeField] private float _speedStandard = 12f;

    [SerializeField] private GameObject _laser;
    [SerializeField] private GameObject _tripleShot;

    private Vector2 _laserPosition;
    [SerializeField] private float _laserOffset = 0.8f;

    [SerializeField] private float _fireRate = 0.15f;
    [SerializeField] private float _laserFireRate = 0.15f;
    [SerializeField] private bool _canFire = true;
    #region Cooldown System using Time.time
    // private float _fireReadyTime;
    #endregion

    [SerializeField] private int _lives = 3;

    private SpawnManager _spawnManager;

    [SerializeField] private bool _isTripleShotActive = false;
    [SerializeField] private float _tripleShotFireRate = 0.2f;
    [SerializeField] private int _tripleShotAmmo = 0;

    [SerializeField] private float _speedBoostActiveTime = 5f;
    private float _speedBoostDeactivationTime;
    [SerializeField] private float _speedBoostTimeScale = 0.7f;
    [SerializeField] private float _speedBoostSpeed = 18f;

    [SerializeField] private float _shields = 0;
    [SerializeField] private GameObject _shield;

    [SerializeField] private int _score = 0;
    private UIManager _uiManager;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("SpawnManager is null!");
        }
        _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.Log("UIManager is null!");
        }
        _shield.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        Fire();
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

        transform.position = new Vector2(Mathf.Clamp(transform.position.x, _playerRightBoundary, _playerLeftBoundary), Mathf.Clamp(transform.position.y, _playerLowerBoundary, _playerUpperBoundary));

        CheckSpeedBoost();

        transform.Translate(_direction * _speed * Time.deltaTime);
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

        if (DidPlayerFire() && _canFire)
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
            
            _canFire = false;
            StartCoroutine(ReadyFire());
        }
    }

    private bool DidPlayerFire()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            return true;
        }

        return false;
    }

    private IEnumerator ReadyFire()
    {
        yield return new WaitForSeconds(_fireRate);
        _canFire = true;
    }

    public void Damage()
    {
        if (_shields > 0)
        {
            _shields--;
            Debug.Log($"Shield Power: {_shields}");
            if (_shields < 1)
            {
                _shield.SetActive(false);
            }
            return;
        }

        _lives--;
        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.StopSpawning();
            Destroy(this.gameObject);
        }
    }

    #region PowerUps
    public void ActivateTripleShot()
    {
        _isTripleShotActive = true;
        _fireRate = _tripleShotFireRate;
        _tripleShotAmmo = 15;
    }

    private void CheckTripleShotAmmo()
    {
        _tripleShotAmmo--;

        if (_tripleShotAmmo < 1)
        {
            _isTripleShotActive = false;
            _fireRate = _laserFireRate;
        }
    }

    public void ActivateSpeedBoost()
    {
        Time.timeScale = _speedBoostTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        _speed = _speedBoostSpeed;

        _speedBoostDeactivationTime = Time.time + _speedBoostActiveTime;
    }

    private void CheckSpeedBoost()
    {
        if (Time.time > _speedBoostDeactivationTime)
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            _speed = _speedStandard;
        }
    }

    public void ActivateShields()
    {
        if (_shields < 3)
        {
            _shields++;
            _shield.SetActive(true);
            Debug.Log($"Shield Power: {_shields}");
        }
    }
    #endregion

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }
}
