using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource _explosionSound;
    private AudioSource _powerUpSound;
    private AudioSource _powerDownSound;
    private AudioSource _powerCoreMovementSound;

    private void OnEnable()
    {
        Player.onShieldDepletion += PlayPowerDownSound;
        MissilePlayer.onExplosion += PlayExplosionSound;
        SpaceBomb.onExplosion += PlayExplosionSound;
        ShieldGenerator.onShieldGeneratorPowerDepletion += PlayPowerDownSound;
        PowerCore.onPowerCoreStartMovement += PlayPowerCoreMovementSound;
        PowerCore.onPowerCoreStopMovement += StopPowerCoreMovementSound;
        PowerCore.onPowerCoreRetracted += PlayPowerUpSound;
        PowerCore.onPowerCoreDepletion += PlayExplosionSound;
        PowerCore.onPowerCoreDepletion += PlayPowerDownSound;
    }

    private void OnDisable()
    {
        Player.onShieldDepletion -= PlayPowerDownSound;
        MissilePlayer.onExplosion -= PlayExplosionSound;
        SpaceBomb.onExplosion -= PlayExplosionSound;
        ShieldGenerator.onShieldGeneratorPowerDepletion -= PlayPowerDownSound;
        PowerCore.onPowerCoreStartMovement -= PlayPowerCoreMovementSound;
        PowerCore.onPowerCoreStopMovement -= StopPowerCoreMovementSound;
        PowerCore.onPowerCoreRetracted -= PlayPowerUpSound;
        PowerCore.onPowerCoreDepletion -= PlayExplosionSound;
        PowerCore.onPowerCoreDepletion -= PlayPowerDownSound;
    }

    // Start is called before the first frame update
    void Start()
    {
        _explosionSound = GameObject.Find("ExplosionSound").GetComponent<AudioSource>();
        if (_explosionSound == null)
        {
            Debug.LogError("ExplosionSound is null!");
        }
        _powerUpSound = GameObject.Find("PowerUpSound").GetComponent<AudioSource>();
        if (_powerUpSound == null)
        {
            Debug.LogError("PowerUpSound is null!");
        }
        _powerDownSound = GameObject.Find("PowerDownSound").GetComponent<AudioSource>();
        if (_powerDownSound == null)
        {
            Debug.LogError("PowerDownSound is null!");
        }
        _powerCoreMovementSound = GameObject.Find("PowerCoreMovementSound").GetComponent<AudioSource>();
        if (_powerCoreMovementSound == null)
        {
            Debug.LogError("PowerCoreMovementSound is null!");
        }

        AudioListener.volume = 0.5f;
    }

    public void PlayExplosionSound()
    {
        if (Time.timeScale == 1f)
        {
            _explosionSound.pitch = 0.25f;
        }
        else
        {
            _explosionSound.pitch = 0.15f;
        }
        _explosionSound.Play();
    }

    public void PlayPowerUpSound()
    {
        _powerUpSound.pitch = Time.timeScale;
        _powerUpSound.Play();
    }

    private void PlayPowerDownSound()
    {
        _powerDownSound.pitch = Time.timeScale / 2;
        _powerDownSound.Play();
    }

    private void PlayPowerCoreMovementSound()
    {
        _powerCoreMovementSound.pitch = Time.timeScale;
        _powerCoreMovementSound.Play();
    }

    private void StopPowerCoreMovementSound()
    {
        _powerCoreMovementSound.pitch = Time.timeScale;
        _powerCoreMovementSound.Stop();
    }
}
