using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource _explosionSound;
    private AudioSource _powerUpSound;

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
}
