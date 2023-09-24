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

    private const float _enemyLeftBoundary = -12f;
    private const float _enemyRightBoundary = 12f;
    private const float _enemyUpperBoundary = 4.5f;
    private const float _enemyLowerBoundary = -4.5f;

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
