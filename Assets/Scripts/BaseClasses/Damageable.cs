using System.Collections;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    private WaitForSeconds _damageflickerWaitTime = new WaitForSeconds(0.08f);

    protected IEnumerator DamageFlicker(SpriteRenderer renderer, Color defaultColor)
    {
        for (int i = 0; i < 4;i++)
        {
            yield return _damageflickerWaitTime;
            if (i % 2 == 0)
            {
                renderer.color = Color.red;
            }
            else
            {
                renderer.color = defaultColor;
            }
        }
        renderer.color = defaultColor;
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
