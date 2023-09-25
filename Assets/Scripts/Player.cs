using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float _horizontalAxis;
    private float _verticalAxis;
    private Vector3 _direction;
    [SerializeField] private float _speed;

    [SerializeField] private float _playerLeftBoundary = 9f;
    [SerializeField] private float _playerRightBoundary = -9f;
    [SerializeField] private float _playerUpperBoundary = 4.8f;
    [SerializeField] private float _playerLowerBoundary = -4.8f;

    #region PlayerWrap
    //private Vector3 _position;

    //[SerializeField] private float _playerLeftWrap = 11f;
    //[SerializeField] private float _playerRightWrap = -11f;
    //[SerializeField] private float _playerUpperWrap = 6f;
    //[SerializeField] private float _playerLowerWrap = -6f;
    #endregion

    [SerializeField] private GameObject _laser;
    private Vector3 _laserPosition;
    [SerializeField] private float _laserOffset = 1.07f;

    [SerializeField] private float _fireRate;
    [SerializeField] private bool _canFire;
    #region Cooldown System using Time.time
    // private float _fireReadyTime;
    #endregion

    [SerializeField] private int _lives;

    private SpawnManager _spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("SpawnManager is null!");
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

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, _playerRightBoundary, _playerLeftBoundary), Mathf.Clamp(transform.position.y, _playerLowerBoundary, _playerUpperBoundary), 0);

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

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) && _canFire)
        {
            _laserPosition = transform.position;
            _laserPosition.x += _laserOffset;

            Instantiate(_laser, _laserPosition, Quaternion.identity);
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
        _lives--;

        if (_lives < 1)
        {
            _spawnManager.StopSpawning();
            Destroy(this.gameObject);
        }
    }
}
