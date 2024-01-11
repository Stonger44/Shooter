using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerCore : MonoBehaviour
{
    [SerializeField] private GameObject _internalPosition;
    [SerializeField] private GameObject _exposedPosition;
    [SerializeField] private float _movementSpeed = 1f;
    private WaitForSeconds _powerCoreExposureTime = new WaitForSeconds(8f);
    private bool _exposePowerCore = false;
    private bool _retractPowerCore = false;

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
        if (_exposePowerCore || _retractPowerCore)
        {
            ChangePosition();
        }
    }

    private void ChangePosition()
    {
        if (_exposePowerCore)
        {
            transform.position = Vector2.MoveTowards(transform.position, _exposedPosition.transform.position, _movementSpeed * Time.deltaTime);

            if (transform.position == _exposedPosition.transform.position)
            {
                _exposePowerCore = false;
            }
        }

        if (_retractPowerCore)
        {
            transform.position = Vector2.MoveTowards(transform.position, _internalPosition.transform.position, _movementSpeed * Time.deltaTime);

            if (transform.position == _internalPosition.transform.position)
            {
                _retractPowerCore = false;
            }
        }
    }

    private IEnumerator PowerCoreExposureCoolDown()
    {
        yield return _powerCoreExposureTime;
        _retractPowerCore = true;
    }

    private void TriggerPowerCoreExposure()
    {
        _exposePowerCore = true;
        StartCoroutine(PowerCoreExposureCoolDown());
    }
}
