using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;

public class Missile : MonoBehaviour
{
    // 0: Player Missile 
    // 1: Enemy Missile
    [SerializeField] private int _missileId;
    [SerializeField] private float _boundary;
    [SerializeField] private float _speed;
    private Vector2 _missileDirection;

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
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(_missileDirection * _speed * Time.deltaTime);

        DestroyIfOutOfBounds();
    }

    private void DestroyIfOutOfBounds()
    {
        if (_missileId == 0)
        {
            if (transform.position.x > _boundary)
            {
                if (transform.parent != null)
                {
                    Destroy(transform.parent.gameObject);
                }
                Destroy(this.gameObject);
            }
        }
        else // _missileId == 1 (Enemy Missile)
        {
            if (transform.position.x < _boundary)
            {
                if (transform.parent != null)
                {
                    Destroy(transform.parent.gameObject);
                }
                Destroy(this.gameObject);
            }
        }
    }

}
