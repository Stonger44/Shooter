using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private const string _playerTag = "Player";
    private const string _laserTag = "Laser";
    private const string _tripleShotTag = "TripleShot";
    private string _otherTag = string.Empty;

    private SpawnManager _spawnManager;
    private Player _player;
    private CircleCollider2D _collider;

    [SerializeField] private float _enemyLeftBoundary = -11.2f;
    [SerializeField] private float _enemyRightBoundary = 11.2f;
    [SerializeField] private float _enemyUpperBoundary = 3.7f;
    [SerializeField] private float _enemyLowerBoundary = -4.9f;

    [SerializeField] private float _speed = 6f;
    private Vector2 _position;
    private Vector2 _direction = Vector2.left;

    [SerializeField] private float _rotationSpeed;
    private float _rotationRange = 200;

    [SerializeField] private int _health = 6;
    private bool _isExploding;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is null!");
        }
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("SpawnManager is null!");
        }
        _collider = GetComponent<CircleCollider2D>();
        if (_collider == null)
        {
            Debug.LogError("Enemy Collider is null!");
        }

        _rotationSpeed = Random.Range(-_rotationRange, _rotationRange);
        Warp();
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
        Move();
    }

    private void Rotate()
    {
        transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);
    }

    private void Move()
    {
        transform.Translate(_direction * _speed * Time.deltaTime);

        if (!_isExploding && transform.position.x < _enemyLeftBoundary)
        {
            Warp();
        }
    }

    private void Warp()
    {
        float yPosition = Random.Range(_enemyLowerBoundary, _enemyUpperBoundary);
        _position = new Vector2(_enemyRightBoundary, yPosition);
        transform.position = _position;
        _health = 6;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _otherTag = other.tag;

        if (_otherTag == _playerTag)
        {
            _player.Damage();
            Damage(_otherTag);
        }
        else if (_otherTag == _laserTag || _otherTag == _tripleShotTag)
        {
            Destroy(other.gameObject);
            Damage(_otherTag);
        }
    }

    private void Damage(string otherTag)
    {
        _health--;

        if (otherTag == _tripleShotTag)
        {
            _health -= 3;
        }

        if (_health < 1 || otherTag == _playerTag)
        {
            _player.AddScore(20);
            _isExploding = true;
            _collider.enabled = false;
            StartCoroutine(DropPowerUp());
            Destroy(this.gameObject, 0.25f);
        }
    }

    private IEnumerator DropPowerUp()
    {
        yield return new WaitForSeconds(0.2f);

        float randomFloat = Random.Range(0f, 1.0f);
        _spawnManager.SpawnPowerUp(this.transform.position);
    }
}
