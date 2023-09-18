// using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed;
    private Vector3 _position;
    private Vector3 _direction = Vector3.left;

    private float _enemyLeftBoundary = -12f;
    private float _enemyRightBoundary = 12f;
    private float _enemyUpperBoundary = 5.5f;
    private float _enemyLowerBoundary = -3.5f;

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

        if (this.transform.position.x < _enemyLeftBoundary)
        {
            Warp();
        }
    }

    private void Warp()
    {
        _position.y = Random.Range(_enemyLowerBoundary, _enemyUpperBoundary);
        _position = new Vector3(_enemyRightBoundary, _position.y, 0);
        this.transform.position = _position;
    }
}
