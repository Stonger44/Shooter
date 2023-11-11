using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    // 0: Player Laser 
    // 1: Enemy Laser
    [SerializeField] private int _laserId;
    [SerializeField] private float _boundary;
    [SerializeField] private float _speed;
    private Vector2 _laserDirection;

    // Start is called before the first frame update
    void Start()
    {
        if (_laserId == 0)
        {
            _laserDirection = Vector2.right;
        }
        else if (_laserId == 1)
        {
            _laserDirection = Vector2.left;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(_laserDirection * _speed * Time.deltaTime);

        DestroyIfOutOfBounds();
    }

    private void DestroyIfOutOfBounds()
    {
        if (_laserId == 0)
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
        else // _laserId == 1 (Enemy Laser)
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
