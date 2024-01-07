using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource _explosionSound;
    private AudioSource _powerUpSound;
    private AudioSource _powerDownSound;

    private void OnEnable()
    {
        MissilePlayer.onExplosion += PlayExplosionSound;
        SpaceBomb.onExplosion += PlayExplosionSound;
        ShieldGenerator.onShieldGeneratorPowerDepletion += PlayPowerDownSound;
    }

    private void OnDisable()
    {
        MissilePlayer.onExplosion -= PlayExplosionSound;
        SpaceBomb.onExplosion -= PlayExplosionSound;
        ShieldGenerator.onShieldGeneratorPowerDepletion += PlayPowerDownSound;
    }

    // Start is called before the first frame update
    void Start()
    {
        _explosionSound = GameObject.Find("ExplosionSound").GetComponent<AudioSource>();
        if (_explosionSound == null)
        {
            Debug.LogError("Explosion Sound is null!");
        }
        _powerUpSound = GameObject.Find("PowerUpSound").GetComponent<AudioSource>();
        if (_powerUpSound == null)
        {
            Debug.LogError("PowerUp Sound is null!");
        }
        _powerDownSound = GameObject.Find("PowerDownSound").GetComponent<AudioSource>();
        if (_powerDownSound == null)
        {
            Debug.LogError("PowerDown Sound is null!");
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
}
