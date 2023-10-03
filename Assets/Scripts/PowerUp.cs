using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private float _powerUpLeftBoundary = -11.1f;

    [SerializeField] private float _speed = 0.5f;
    private Vector2 _direction = Vector2.left;

    private const string _playerTag = "Player";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(_direction * _speed * Time.deltaTime);

        if (transform.position.x < _powerUpLeftBoundary)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == _playerTag)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.ActivateTripleShot();
            }
            Destroy(this.gameObject);
        }
    }
}
