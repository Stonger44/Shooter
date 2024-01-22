using System.Collections;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    private WaitForSeconds _damageflickerWaitTime = new WaitForSeconds(0.2f);
    private Color _damageColor = Color.red;

    protected IEnumerator DamageDisplay()
    {
        for (int i = 0; i < 4;i++)
        {
            yield return _damageflickerWaitTime;

        }
    }

    protected IEnumerator ShieldFailure(GameObject shield)
    {
        WaitForSeconds _shieldFlickerTime = new WaitForSeconds(0.05f);
        bool showShieldSprite = false;

        for (int i = 0; i < 3; i++)
        {
            yield return _shieldFlickerTime;
            shield.SetActive(showShieldSprite);
            showShieldSprite = !showShieldSprite;
        }
    }
}
