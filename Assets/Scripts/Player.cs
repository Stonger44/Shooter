﻿using System;
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

    private float _playerLeftBoundary = 9f;
    private float _playerRightBoundary = -9f;
    private float _playerUpperBoundary = 5.8f;
    private float _playerLowerBoundary = -3.8f;

    #region PlayerWrap
    //private float _playerLeftWrap = 11f;
    //private float _playerRightWrap = -11f;
    //private float _playerUpperWrap = 7.5f;
    //private float _playerLowerWrap = -5.5f;
    #endregion

    [SerializeField] private GameObject _laser;
    private Quaternion _laserRotation = Quaternion.Euler(0, 0, -90);

    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = _position;
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fire();
        }
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
        Instantiate(_laser, this.transform.position, _laserRotation);
    }
}




