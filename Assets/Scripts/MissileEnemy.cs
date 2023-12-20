using System.Collections;
using UnityEngine;

public class MissileEnemy : MonoBehaviour
{
    private const string _playerTag = "Player";
    private string _otherTag = string.Empty;

    private GameObject _player;
    private Rigidbody2D _rigidBody;

    [SerializeField] private float _speed = 12;
    [SerializeField] private float _rotateSpeed = 125;
    [SerializeField] private float _missileActiveTime = 2f;
    [SerializeField] private GameObject _missileExplosion;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player");
        if (_player == null)
        {
            Debug.LogError("Player is null!");
        }
        _rigidBody = GetComponent<Rigidbody2D>();
        if (_rigidBody == null)
        {
            Debug.LogError("RigidBody is null!");
        }

        StartCoroutine(ArmMissile());
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector2 lookDirection = (Vector2)_player.transform.position - _rigidBody.position;

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
