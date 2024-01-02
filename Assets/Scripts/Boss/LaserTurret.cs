using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTurret : MonoBehaviour
{
    [Header("Laser")]
    [SerializeField] private GameObject _laser;
    [SerializeField] private AudioClip _laserSound;
    private WaitForSeconds _fireDelay = new WaitForSeconds(1f);
    private WaitForSeconds _fireRate = new WaitForSeconds(0.5f);
    private bool _canFire = false;
    private bool _ceaseFire = false;

    private AudioSource _audioSource;

    [Header("Turret")]
    [SerializeField] private Vector3 _eulerAngle;
    [SerializeField] private float _zDegrees = 30f;
    [SerializeField] private GameObject _barrelTip;

    private void OnEnable()
    {
        EnemyLeader.onCommenceAttack += CommenceFiring;
        Player.onPlayerDeath += CeaseFire;
    }

    private void OnDisable()
    {
        EnemyLeader.onCommenceAttack -= CommenceFiring;
        Player.onPlayerDeath -= CeaseFire;
    }

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("AudioSource is null!");
        }

        _eulerAngle = new Vector3(0, 0, _zDegrees);
    }

    // Update is called once per frame
    void Update()
    {
        if (_canFire)
        {
            _canFire = false;
            StartCoroutine(Fire());
        }

        Rotate();
    }

    private void Rotate()
    {
        CheckRotation();
        transform.Rotate(_eulerAngle * Time.deltaTime);
    }

    private void CheckRotation()
    {
        if (transform.rotation.eulerAngles.z >= _zDegrees && transform.rotation.eulerAngles.z < 180f ||
            transform.rotation.eulerAngles.z <= 360f - _zDegrees && transform.rotation.eulerAngles.z > 180f)
        {
            _eulerAngle = -_eulerAngle;
        }
    }

    private IEnumerator Fire()
    {
        for (int i = 0; i < 5; i++)
        {
            yield return _fireRate;
            Instantiate(_laser, _barrelTip.transform.position, transform.rotation);
            SetLaserSound();
            _audioSource.Play(); 
        }

        StartCoroutine(ReadyFire());
    }

    private IEnumerator ReadyFire()
    {
        yield return _fireDelay;
        if (!_ceaseFire)
        {
            _canFire = true;
        }
    }

    private void SetLaserSound()
    {
        _audioSource.clip = _laserSound;
        if (Time.timeScale == 1)
        {
            _audioSource.pitch = 0.5f;
        }
        else
        {
            _audioSource.pitch = 0.3f;
        }
    }

    private void CommenceFiring()
    {
        StartCoroutine(ReadyFire());
    }

    private void CeaseFire()
    {
        _ceaseFire = true;
    }
}
