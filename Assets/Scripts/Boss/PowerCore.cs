using System;
using System.Collections;
using UnityEngine;

public class PowerCore : MonoBehaviour
{
    [SerializeField] private GameObject _internalPosition;
    [SerializeField] private GameObject _exposedPosition;
    [SerializeField] private float _movementSpeed = 1f;
    private WaitForSeconds _powerCoreExposureTime = new WaitForSeconds(8f);
    private bool _exposePowerCore = false;
    private bool _retractPowerCore = false;

    public static event Action onPowerCoreRetracted;
    public static event Action onPowerCoreStartMovement;
    public static event Action onPowerCoreStopMovement;

    private void OnEnable()
    {
        EnemyLeader.onShieldDepletion += TriggerPowerCoreExposure;
    }

    private void OnDisable()
    {
        EnemyLeader.onShieldDepletion -= TriggerPowerCoreExposure;
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.position = _internalPosition.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (_exposePowerCore)
        {
            ExposePowerCore();
        }

        if (_retractPowerCore)
        {
            RetractPowerCore();
        }
    }

    private IEnumerator PowerCoreExposureCoolDown()
    {
        yield return _powerCoreExposureTime;
        _retractPowerCore = true;
        onPowerCoreStartMovement?.Invoke();
    }

    private void TriggerPowerCoreExposure()
    {
        _exposePowerCore = true;
        StartCoroutine(PowerCoreExposureCoolDown());
        onPowerCoreStartMovement?.Invoke();
    }

    private void ExposePowerCore()
    {
        transform.position = Vector2.MoveTowards(transform.position, _exposedPosition.transform.position, _movementSpeed * Time.deltaTime);

        if (transform.position == _exposedPosition.transform.position)
        {
            _exposePowerCore = false;
            onPowerCoreStopMovement?.Invoke();
        }
    }

    private void RetractPowerCore()
    {
        transform.position = Vector2.MoveTowards(transform.position, _internalPosition.transform.position, _movementSpeed * Time.deltaTime);

        if (transform.position == _internalPosition.transform.position)
        {
            _retractPowerCore = false;
            onPowerCoreRetracted?.Invoke();
            onPowerCoreStopMovement?.Invoke();
        }
    }
}
