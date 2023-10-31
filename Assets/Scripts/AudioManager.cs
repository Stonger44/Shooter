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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayExplosionSound()
    {
        _explosionSound.pitch = Time.timeScale;
        _explosionSound.Play();
    }

    public void PlayPowerUpSound()
    {
        _powerUpSound.pitch = Time.timeScale;
        _powerUpSound.Play();
    }
}
