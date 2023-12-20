using System.Collections;
using UnityEngine;

public class SpaceShip : MonoBehaviour
{
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
