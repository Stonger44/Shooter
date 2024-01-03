using System;
using System.Collections;
using UnityEngine;

public class MissilePlayer : MonoBehaviour
{
    private const string _enemyTag = "Enemy";
    private const string _enemyleaderTag = "EnemyLeader";
    private const string _shieldGeneratorTag = "ShieldGenerator";
    private string _otherTag = string.Empty;

    private GameObject _target;
    private Rigidbody2D _rigidBody;

    [SerializeField] private float _speed = 12;
    [SerializeField] private float _rotateSpeed = 125;
    [SerializeField] private float _missileActiveTime = 2f;

    [SerializeField] private GameObject _missilePlayerExplosion;

    public static event Action onExplosion;

    // Start is called before the first frame update
    void Start()
    {
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

    public void SetTarget(GameObject target)
    {
        _target = target;
    }

    public void DetonateMissile(GameObject gameObject)
    {
        Vector2 explosionPosition = transform.position;
        explosionPosition.x += 0.5f;
        if (gameObject.CompareTag(_enemyleaderTag) || gameObject.CompareTag(_shieldGeneratorTag))
        {
            Instantiate(_missilePlayerExplosion, explosionPosition, Quaternion.identity);
            onExplosion?.Invoke(); 
        }
        Destroy(this.gameObject);
    }

    private void Move()
    {
        if (_target != null)
        {
            Vector2 lookDirection = (Vector2)_target.transform.position - _rigidBody.position;

            lookDirection.Normalize();

            float rotateAmount = Vector3.Cross(lookDirection, -transform.right).z;

            _rigidBody.angularVelocity = rotateAmount * _rotateSpeed; 
        }

        _rigidBody.velocity = transform.right * _speed;
    }

    private IEnumerator ArmMissile()
    {
        yield return new WaitForSeconds(_missileActiveTime);
        DetonateMissile(this.gameObject);
    }
}
