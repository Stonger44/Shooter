using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private const string _playerTag = "Player";

    private AudioManager _audioManager;
    private SpriteRenderer _renderer;

    /*-----PowerUp Ids-----*\
    0: TripleShot
    1: SpeedBoost
    2: Shield
    3: SpaceBomb
    4: PlayerLife
    5: SlowBomb
    \*-----PowerUp Ids-----*/
    [SerializeField] private int _powerUpId;
    [SerializeField] private float _powerUpLeftBoundary = -11f;
    [SerializeField] private float _powerUpAvailableTime = 3f;
    [SerializeField] private float _speed = 0.5f;
    private Vector2 _direction = Vector2.left;

    [Header("Colors")]
    [SerializeField] bool _showPowerUpBlinkColors = false;
    [SerializeField] bool _showPowerDownBlinkColors = false;
    [SerializeField] private float _colorBlinkTime = 0.1f;
    private List<Color> _colorList = new List<Color>();
    private WaitForSeconds _colorWaitForSeconds;

    [Header("PowerDowns")]
    [SerializeField] private bool _isPowerDown;
    [SerializeField] private GameObject _powerDownExplosion;
    [SerializeField] private CircleCollider2D _collider;

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
        if (_isPowerDown && _collider == null)
        {
            Debug.LogError("PowerUp Collider is null!");
        }
        if (_isPowerDown && _powerDownExplosion == null)
        {
            Debug.LogError("PowerDownExplosion is null!");
        }

        _colorWaitForSeconds = new WaitForSeconds(_colorBlinkTime);

        if (_showPowerUpBlinkColors || _showPowerDownBlinkColors)
        {
            BlinkColors(); 
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
                        player.ActivateTripleShot();
                        CollectPowerUp();
                        break;
                    case 1:
                        player.ActivateSpeedBoost();
                        CollectPowerUp();
                        break;
                    case 2:
                        player.ActivateShields();
                        CollectPowerUp();
                        break;
                    case 3:
                        player.CollectSpaceBomb();
                        CollectPowerUp();
                        break;
                    case 4:
                        player.CollectPlayerLife();
                        CollectPowerUp();
                        break;
                    case 5:
                        player.DetonateSlowBomb();
                        StartCoroutine(DestroyPowerDown());
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void CollectPowerUp()
    {
        _audioManager.PlayPowerUpSound();
        Destroy(this.gameObject);
    }

    private IEnumerator DestroyPowerUp()
    {
        yield return new WaitForSeconds(_powerUpAvailableTime);
        Destroy(this.gameObject);
    }

    private IEnumerator DestroyPowerDown()
    {
        _collider.enabled = false;
        _powerDownExplosion.SetActive(true);
        _audioManager.PlayExplosionSound();
        Destroy(this.gameObject, 2.7f);

        yield return new WaitForSeconds(0.20f);
        _renderer.enabled = false;
    }

    private void BlinkColors()
    {
        LoadColorList();
        StartCoroutine(MakePowerUpColorful());
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
                    yield return _colorWaitForSeconds;
                }
                
            } 
        }
    }

    private void LoadColorList()
    {
        if (_showPowerUpBlinkColors)
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
        else if (_showPowerDownBlinkColors)
        {
            _colorList.Add(Color.white);
            _colorList.Add(Color.red);
        }
    }
}
