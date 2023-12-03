using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;

public class Missile : MonoBehaviour
{
    private const string _playerTag = "Player";
    private string _otherTag = string.Empty;

    // 0: Player Missile 
    // 1: Enemy Missile
    [SerializeField] private int _missileId;
    [SerializeField] private float _speed;
    [SerializeField] private float _missileActiveTime = 3f;
    private Vector2 _missileDirection;
    [SerializeField] private GameObject _missileExplosion;

    // Start is called before the first frame update
    void Start()
    {
        if (_missileId == 0)
        {
            _missileDirection = Vector2.right;
        }
        else if (_missileId == 1)
        {
            _missileDirection = Vector2.left;
        }

        StartCoroutine(ArmMissile());
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(_missileDirection * _speed * Time.deltaTime);

        // Make Missile rotate and follow player
    }

    private IEnumerator ArmMissile()
    {
        yield return new WaitForSeconds(_missileActiveTime);
        DetonateMissile();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _otherTag = other.tag;

        if (_otherTag == _playerTag)
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
