using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PowerUp : MonoBehaviour
{
    private const string _playerTag = "Player";

    [SerializeField] private float _powerUpLeftBoundary = -11.1f;

    [SerializeField] private float _speed = 0.5f;
    private Vector2 _direction = Vector2.left;

    [SerializeField] private float _powerUpAvailableTime = 3f;
    [SerializeField] private int _powerUpID;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyPowerUp());
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
                switch (_powerUpID)
                {
                    case 0:
                        player.ActivateTripleShot();
                        break;
                    case 1:
                        player.ActivateSpeedBoost();
                        break;
                    default:

                        break;
                }
            }
            Destroy(this.gameObject);
        }
    }

    private IEnumerator DestroyPowerUp()
    {
        yield return new WaitForSeconds(_powerUpAvailableTime);
        Destroy(this.gameObject);
    }
}
