using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private const string _playerTag = "Player";

    private AudioManager _audioManager;

    /*-----Power Up Ids-----*\
    0: SpaceBomb
    1: TripleShot
    2: SpeedBoost
    3: Shield
    \*-----Power Up Ids-----*/
    [SerializeField] private int _powerUpId;
    [SerializeField] private float _powerUpLeftBoundary = -11f;
    [SerializeField] private float _powerUpAvailableTime = 3f;
    [SerializeField] private float _speed = 0.5f;
    private Vector2 _direction = Vector2.left;

    private SpriteRenderer _renderer;
    private List<Color> _colorList = new List<Color>();

    // Start is called before the first frame update
    void Start()
    {
        _audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        if (_audioManager == null)
        {
            Debug.LogError("Audio Manager is null!");
        }
        _renderer = GetComponent<SpriteRenderer>();
        if (_renderer == null)
        {
            Debug.LogError("Renderer is null!");
        }

        if (_powerUpId == 0)
        {
            LoadColorList();
            StartCoroutine(MakePowerUpColorful());
        }
        
        StartCoroutine(DestroyPowerUp());
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(_direction * _speed * Time.deltaTime);

        if (transform.position.x < _powerUpLeftBoundary)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == _playerTag)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                switch (_powerUpId)
                {
                    case 0:
                        player.CollectSpaceBomb();
                        break;
                    case 1:
                        player.ActivateTripleShot();
                        break;
                    case 2:
                        player.ActivateSpeedBoost();
                        break;
                    case 3:
                        player.ActivateShields();
                        break;
                    default:
                        break;
                }
            }
            _audioManager.PlayPowerUpSound();
            Destroy(this.gameObject);
        }
    }

    private IEnumerator DestroyPowerUp()
    {
        yield return new WaitForSeconds(_powerUpAvailableTime);
        Destroy(this.gameObject);
    }

    private IEnumerator MakePowerUpColorful()
    {
        if (_renderer != null)
        {
            while (true)
            {
                foreach (Color color in _colorList)
                {
                    _renderer.color = color;
                    yield return new WaitForSeconds(0.1f);
                }
                
            } 
        }
    }

    private void LoadColorList()
    {
        _colorList.Add(new Color(255, 0, 0)); // Red
        _colorList.Add(new Color(255, 150, 0)); // Orange
        _colorList.Add(new Color(255, 255, 0)); // Yellow
        _colorList.Add(new Color(255, 120, 0)); // Yellow Green
        _colorList.Add(new Color(0, 255, 0)); // Green
        _colorList.Add(new Color(0, 255, 150)); // Light Green
        _colorList.Add(new Color(0, 255, 255)); // Blue Green
        _colorList.Add(new Color(0, 150, 255)); // Light Blue
        _colorList.Add(new Color(0, 0, 255)); // Blue
        _colorList.Add(new Color(120, 0, 255)); // Blue Purple
        _colorList.Add(new Color(255, 0, 255)); // Purple
        _colorList.Add(new Color(255, 0, 120)); // Pink
        _colorList.Add(new Color(255, 0, 0)); // Red
    }
}
