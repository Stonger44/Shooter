﻿// using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed;
    private Vector3 _position;
    private Vector3 _direction = Vector3.left;

    [SerializeField] private float _enemyLeftBoundary = -12f;
    [SerializeField] private float _enemyRightBoundary = 12f;
    [SerializeField] private float _enemyUpperBoundary = 4.5f;
    [SerializeField] private float _enemyLowerBoundary = -4.5f;

    private const string _playerTag = "Player";
    private const string _laserTag = "Laser";

    // Start is called before the first frame update
    void Start()
    {
        Warp();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        this.transform.Translate(_direction * _speed * Time.deltaTime);

        if (transform.position.x < _enemyLeftBoundary)
        {
            Warp();
        }
    }

    private void Warp()
    {
        float yPosition = Random.Range(_enemyLowerBoundary, _enemyUpperBoundary);
        _position = new Vector3(_enemyRightBoundary, yPosition, 0);
        transform.position = _position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == _playerTag)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
            Destroy(this.gameObject);
        }
        else if (other.tag == _laserTag)
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }
    }
}
