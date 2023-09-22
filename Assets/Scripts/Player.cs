using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _horizontalAxis;
    [SerializeField] private float _verticalAxis;
    [SerializeField] private Vector3 _direction;
    [SerializeField] private float _speed;
    [SerializeField] private Vector3 _position;

    private const float _playerLeftBoundary = 9f;
    private const float _playerRightBoundary = -9f;
    private const float _playerUpperBoundary = 4.8f;
    private const float _playerLowerBoundary = -4.8f;

    #region PlayerWrap
    //private float _playerLeftWrap = 11f;
    //private float _playerRightWrap = -11f;
    //private float _playerUpperWrap = 7.5f;
    //private float _playerLowerWrap = -5.5f;
    #endregion

    [SerializeField] private GameObject _laser;
    private Vector3 _laserPosition;
    private float _laserOffset = 0.8f;
    private Quaternion _laserRotation = Quaternion.Euler(0, 0, -90);

    [SerializeField] private float _fireRate;
    [SerializeField] private bool _canFire;
    #region Cooldown System using Time.time
    // private float _fireReadyTime;
    #endregion

    [SerializeField] private int _health;

    private SpawnManager _spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = _position;
        
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError($"SpawnManager is null!");
        }
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

        this.transform.position = new Vector3(Mathf.Clamp(this.transform.position.x, _playerRightBoundary, _playerLeftBoundary), Mathf.Clamp(this.transform.position.y, _playerLowerBoundary, _playerUpperBoundary), 0);

        this.transform.Translate(_direction * _speed * Time.deltaTime);
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

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) && _canFire)
        {
            _laserPosition = this.transform.position;
            _laserPosition.x += _laserOffset;

            Instantiate(_laser, _laserPosition, _laserRotation);
            _canFire = false;
            StartCoroutine(ReadyFire());
        }
    }

    private IEnumerator ReadyFire()
    {
        yield return new WaitForSeconds(_fireRate);
        _canFire = true;
    }

    public void Damage()
    {
        _health--;

        if (_health < 1)
        {
            Destroy(this.gameObject);
            _spawnManager.StopSpawning();
        }
    }
}
