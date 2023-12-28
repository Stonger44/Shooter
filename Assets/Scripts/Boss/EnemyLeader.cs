using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLeader : SpaceShip
{
    [Header("Boundaries")]
    [SerializeField] private float _leftBoundary = 11f;
    [SerializeField] private float _rightBoundary = 21f;
    [SerializeField] private float _upperBoundary = 0.3f;
    [SerializeField] private float _lowerBoundary = -0.3f;

    [Header("Movement")]
    [SerializeField] private float _speed = 2f;
    private Vector2 _direction = Vector2.left;
    private bool _holdPosition = false;

    [Header("PowerCore")]
    [SerializeField] private GameObject _powerCore;
    [SerializeField] private float _xPowerCoreInternalPosition = -6.84f;
    [SerializeField] private float _xPowerCoreExposedPosition = -8.4f;

    [Header("Shields")]
    [SerializeField] private GameObject _shield;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector2(21f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_holdPosition)
        {
            Approach();
        }
        else
        {
            Move();
        }
        
    }

    private void Approach()
    {
        transform.Translate(_direction * _speed * Time.deltaTime);

        if (transform.position.x <= 11f)
        {
            _holdPosition = true;
        }
    }

    private void Move()
    {
        
    }

}
