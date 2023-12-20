﻿using System.Collections;
using UnityEngine;

public class MissilePlayer : MonoBehaviour
{
    private const string _enemyTag = "Enemy";
    private string _otherTag = string.Empty;

    private GameObject _target;
    private Rigidbody2D _rigidBody;

    [SerializeField] private float _speed = 12;
    [SerializeField] private float _rotateSpeed = 125;
    [SerializeField] private float _missileActiveTime = 2f;
    [SerializeField] private GameObject _missileExplosion;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        if (_rigidBody == null)
        {
            Debug.LogError("RigidBody is null!");
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector2 lookDirection = (Vector2)_target.transform.position - _rigidBody.position;

        lookDirection.Normalize();

        float rotateAmount = Vector3.Cross(lookDirection, -transform.right).z;

        _rigidBody.angularVelocity = -rotateAmount * _rotateSpeed;

        _rigidBody.velocity = -transform.right * _speed;
    }

    private IEnumerator ArmMissile()
    {
        yield return new WaitForSeconds(_missileActiveTime);
        DetonateMissile();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _otherTag = other.tag;

        if (_otherTag == _enemyTag)
        {
            DetonateMissile();
        }
    }

    private void DetonateMissile()
    {
        Instantiate(_missileExplosion, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
}
