using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerCore : MonoBehaviour
{
    [SerializeField] private float _xPowerCoreInternalPosition = -6.84f;
    [SerializeField] private float _xPowerCoreExposedPosition = -8.4f;
    [SerializeField] private float _powerCoreMovementSpeed = 2f;
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

    private void ExposePowerCore()
    {

    }

    private void RetractPowerCore()
    {

    }

    private IEnumerator PowerCoreExposureCoolDown()
    {
        yield return _powerCoreExposureTime;
        _exposePowerCore = false;
        _retractPowerCore = true;
    }

    private void TriggerPowerCoreExposure()
    {
        _exposePowerCore = true;
        StartCoroutine(PowerCoreExposureCoolDown());
    }
}
