using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileControl : MonoBehaviour
{
    private Player _player;

    [Header("Missile")]
    [SerializeField] private GameObject _missile;
    [SerializeField] private float _xMissileOffset = -0.4f;
    [SerializeField] private float _yMissileOffset = 0.82f;
    private WaitForSeconds _fireRate = new WaitForSeconds(3f);
    private WaitForSeconds _fireDelay = new WaitForSeconds(0.5f);
    private bool _canFire = false;

    private void OnEnable()
    {
        EnemyLeader.onCommenceAttack += CommenceFiring;
    }

    private void OnDisable()
    {
        EnemyLeader.onCommenceAttack -= CommenceFiring;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_canFire)
        {
            _canFire = false;
            StartCoroutine(Fire());
        }
    }

    private IEnumerator Fire()
    {
        yield return _fireDelay;
        FireMissile(_xMissileOffset, _yMissileOffset);
        yield return _fireDelay;
        FireMissile(_xMissileOffset, -_yMissileOffset);
        StartCoroutine(ReadyFire());
    }

    private void FireMissile(float xOffset, float yOffset)
    {
        Vector2 missilePosition = transform.position;
        
        missilePosition.x += xOffset;
        missilePosition.y += yOffset;

        Instantiate(_missile, missilePosition, Quaternion.identity);
    }

    private IEnumerator ReadyFire()
    {
        yield return _fireRate;
        _canFire = true;
    }

    private void CommenceFiring()
    {
        _canFire = true;
    }
}
