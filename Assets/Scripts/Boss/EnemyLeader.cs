using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLeader : SpaceShip
{
    [Header("Boundaries")]
    [SerializeField] private float _leftBoundary = 10.75f;
    [SerializeField] private float _rightBoundary = 11f;
    [SerializeField] private float _upperBoundary = 0.3f;
    [SerializeField] private float _lowerBoundary = -1f;

    [Header("Movement")]
    [SerializeField] private float _speed = 2f;
    private Vector2 _direction = Vector2.left;
    private bool _holdPosition = false;
    [SerializeField] private float _xDirection = -1f;
    [SerializeField] private float _yDirection = 0.2f;

    [Header("PowerCore")]
    [SerializeField] private GameObject _powerCore;
    [SerializeField] private float _xPowerCoreInternalPosition = -6.84f;
    [SerializeField] private float _xPowerCoreExposedPosition = -8.4f;

    [Header("Shields")]
    [SerializeField] private GameObject _shield;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector2(22f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (transform.position.y >= _upperBoundary || transform.position.y <= _lowerBoundary)
        {
            Change_Y_Direction();
        }

        transform.Translate(_direction * _speed * Time.deltaTime);

        if (_holdPosition)
        {
            transform.position = new Vector2(Mathf.Clamp(transform.position.x, _leftBoundary, _rightBoundary), Mathf.Clamp(transform.position.y, _lowerBoundary, _upperBoundary));
        }
        else
        {
            CheckApproach();
        }
    }

    private void CheckApproach()
    {
        if (transform.position.x <= _rightBoundary)
        {
            float yDirection = UnityEngine.Random.value < 0.5f ? -_yDirection : _yDirection;
            _direction = new Vector2(_xDirection, yDirection);

            _holdPosition = true;
        }
    }

    private void Change_Y_Direction()
    {
        if (transform.position.y >= _upperBoundary)
        {
            _direction.y = -_yDirection;
        }
        else if (transform.position.y <= _lowerBoundary)
        {
            _direction.y = _yDirection;
        }
    }
}
