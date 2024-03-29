﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameManager _gameManager;
    private Player _player;
    private Camera _camera;

    [Header("Game State")]
    [SerializeField] private GameObject _gameOverUI;
    [SerializeField] private GameObject _restartUI;
    [SerializeField] private GameObject _returnToMainMenuUI;
    [SerializeField] private GameObject _paused;
    [SerializeField] private WaitForSeconds _blinkWaitForSeconds = new WaitForSeconds(0.5f);
    private bool _displayGameOver = false;

    [Header("Game")]
    [SerializeField] private Text _score;
    [SerializeField] private GameObject _waveBanner;
    [SerializeField] private Text _waveBannerText;
    [SerializeField] private Text _waveCount;
    [SerializeField] private Text _enemyCount;
    [SerializeField] private GameObject _youDefeated;
    [SerializeField] private GameObject _theBoss;

    [Header("Boss")]
    [SerializeField] private GameObject _bossUI;
    private bool _displayBoss = false;

    [Header("SpeedBoost")]
    [SerializeField] private Image _speedBoostBar;

    [Header("AfterBurner")]
    [SerializeField] private Image _afterBurnerBar;
    [SerializeField] private GameObject _afterBurnerCoolDown;

    [Header("Lives")]
    [SerializeField] private Image _lives;
    [SerializeField] private Sprite[] _livesSpriteArray;

    [Header("Shields")]
    [SerializeField] private GameObject[] _shieldsArray;

    [Header("TripleShot")]
    [SerializeField] private Text _tripleShotAmmo;
    private string _tripleShotAmmoCount;

    [Header("Homing Missile")]
    [SerializeField] private GameObject[] _homingMissileArray;

    [Header("SpaceBomb")]
    [SerializeField] private GameObject[] _spaceBombArray;

    [Header("Enemyleader")]
    [SerializeField] private GameObject _enemyLeaderContainer;
    [SerializeField] private Image _enemyLeaderShield;
    [SerializeField] private Image _enemyLeaderHealth;

    public static event Action onGameClear;

    private void OnEnable()
    {
        SpawnManager.onEnemyLeaderSpawn += DisplayBossUI;
        EnemyLeader.onCommenceAttack += DisplayEnemyLeaderUI;
        EnemyLeader.onShieldDamage += UpdateEnemyLeaderShield;
        EnemyLeader.onShieldCharge += UpdateEnemyLeaderShield;
        EnemyLeader.onHealthDamage += UpdateEnemyLeaderHealth;
        EnemyLeader.onDestruction += InitiateGameClearUI;
    }

    private void OnDisable()
    {
        SpawnManager.onEnemyLeaderSpawn -= DisplayBossUI;
        EnemyLeader.onCommenceAttack -= DisplayEnemyLeaderUI;
        EnemyLeader.onShieldDamage -= UpdateEnemyLeaderShield;
        EnemyLeader.onShieldCharge -= UpdateEnemyLeaderShield;
        EnemyLeader.onHealthDamage -= UpdateEnemyLeaderHealth;
        EnemyLeader.onDestruction -= InitiateGameClearUI;
    }

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("GameManager is null!");
        }
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is null!");
        }
        _camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        if (_camera == null)
        {
            Debug.LogError("Camera is null!");
        }

        _tripleShotAmmo.text = string.Empty;
        _speedBoostBar.fillAmount = 0f;
        foreach (GameObject shield in _shieldsArray)
        {
            shield.SetActive(false);
        }
        foreach (GameObject spaceBomb in _spaceBombArray)
        {
            spaceBomb.SetActive(false);
        }
        foreach (GameObject missile in _homingMissileArray)
        {
            missile.SetActive(false);
        }

        _enemyLeaderShield.fillAmount = 1f;
        _enemyLeaderHealth.fillAmount = 1f;
    }

    public IEnumerator WaveCleared(int waveNumber)
    {
        _waveBannerText.text = $"Wave {waveNumber} Cleared!";
        _waveBanner.SetActive(true);
        yield return new WaitForSeconds(2f);
        _waveBanner.SetActive(false);
    }

    public IEnumerator UpdateWave(int waveNumber, int enemyCount)
    {
        _waveBannerText.text = $"Wave {waveNumber}";
        _waveBanner.SetActive(true);
        _waveCount.text = $"Wave: {waveNumber}";
        _enemyCount.text = $"Enemies: {enemyCount}";
        yield return new WaitForSeconds(2f);
        _waveBanner.SetActive(false);
    }

    public void TogglePausedUI()
    {
        bool showPausedUI = !_paused.activeInHierarchy;
        _paused.SetActive(showPausedUI);
        _restartUI.SetActive(showPausedUI);
        _returnToMainMenuUI.SetActive(showPausedUI);
    }

    public void UpdateScoreUI(int score)
    {
        _score.text = $"Score: {score}";
    }

    public void UpdateEnemyCount(int enemyCount)
    {
        _enemyCount.text = $"Enemies: {enemyCount}";
    }

    public void UpdateSpeedBoostBar(float speedBoostActiveTime, float speedBoostDeactivationTime)
    {
        float speedBoostTimeRemaining = speedBoostDeactivationTime - Time.time;

        if (speedBoostTimeRemaining <= 0)
        {
            _speedBoostBar.fillAmount = 0f;
            return;
        }

        float speedBoostActivePercent = speedBoostTimeRemaining / speedBoostActiveTime;
        _speedBoostBar.fillAmount = speedBoostActivePercent;
    }

    public void UpdateAfterBurnerBar(float afterBurnerTimeRemaining, float afterBurnerMaxActiveTime)
    {
        float afterBurnerPercent = afterBurnerTimeRemaining / afterBurnerMaxActiveTime;
        _afterBurnerBar.fillAmount = afterBurnerPercent;
    }

    public IEnumerator AfterBurnerCoolDown()
    {
        if (_player != null)
        {
            bool isAfterBurnerInCoolDown = true;
            bool displayAfterBurnerCoolDown = false;

            while (isAfterBurnerInCoolDown)
            {
                displayAfterBurnerCoolDown = !displayAfterBurnerCoolDown;
                _afterBurnerCoolDown.SetActive(displayAfterBurnerCoolDown);
                yield return new WaitForSeconds(0.5f);
                isAfterBurnerInCoolDown = _player.GetAfterBurnerCoolDown();
            }
        }
        _afterBurnerCoolDown.SetActive(false);
    }

    public void UpdateLivesUI(int lives)
    {
        _lives.sprite = _livesSpriteArray[lives];

        if (lives < 1)
        {
            InitiateGameOverUI();
        }
    }

    public void UpdateShieldsUI(int shieldLevel)
    {
        for (int i = 0; i < _shieldsArray.Length; i++)
        {
            if (i < shieldLevel)
            {
                _shieldsArray[i].SetActive(true);
            }
            else
            {
                _shieldsArray[i].SetActive(false);
            }
        }
    }

    public void UpdateTripleShotAmmo(int tripleShotAmmo)
    {
        _tripleShotAmmoCount = string.Empty;

        for (int i = 0; i < tripleShotAmmo; i++)
        {
            _tripleShotAmmoCount += "|";
        }

        _tripleShotAmmo.text = _tripleShotAmmoCount;
    }

    public void UpdateHomingMissileAmmo(int homingMissileAmmo)
    {
        for (int i = 0; i < _homingMissileArray.Length; i++)
        {
            if (i < homingMissileAmmo)
            {
                _homingMissileArray[i].SetActive(true);
            }
            else
            {
                _homingMissileArray[i].SetActive(false);
            }
        }
    }

    public void UpdateSpaceBombAmmo(int spaceBombAmmo)
    {
        for (int i = 0; i < _spaceBombArray.Length; i++)
        {
            if (i < spaceBombAmmo)
            {
                _spaceBombArray[i].SetActive(true);
            }
            else
            {
                _spaceBombArray[i].SetActive(false);
            }
        }
    }

    private void InitiateGameOverUI()
    {
        // Set game time back to normal in case SpeedBoost is active when the player dies
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        StartCoroutine(_camera.CameraShake());
        StartCoroutine(GameOverBlink());
        StartCoroutine(DisplayGameOverUI());
    }

    private IEnumerator GameOverBlink()
    {
        yield return _blinkWaitForSeconds;

        while (true)
        {
            yield return _blinkWaitForSeconds;
            
            _displayGameOver = !_displayGameOver;
            _gameOverUI.SetActive(_displayGameOver);
        }
    }

    private IEnumerator DisplayGameOverUI()
    {
        yield return new WaitForSeconds(3);
        _restartUI.SetActive(true);
        _returnToMainMenuUI.SetActive(true);
        _gameManager.GameOver();
    }

    private void DisplayBossUI()
    {
        StartCoroutine(BossBlink());
    }

    private IEnumerator BossBlink()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return _blinkWaitForSeconds;

            _displayBoss = !_displayBoss;
            _bossUI.SetActive(_displayBoss);
        }
    }

    private void DisplayEnemyLeaderUI()
    {
        _enemyLeaderContainer.SetActive(true);
    }

    private void UpdateEnemyLeaderShield(float currentShieldLevel, float maxShieldLevel)
    {
        float shieldPercent = currentShieldLevel / maxShieldLevel;
        _enemyLeaderShield.fillAmount = shieldPercent;
    }

    private void UpdateEnemyLeaderHealth(float currentHealth, float maxHealth)
    {
        float healthPercent = currentHealth / maxHealth;
        _enemyLeaderHealth.fillAmount = healthPercent;
    }

    private void InitiateGameClearUI()
    {
        if (!_gameManager.IsGameOver())
        {
            StartCoroutine(DisplayGameClearUI()); 
        }
    }

    private IEnumerator DisplayGameClearUI()
    {
        _youDefeated.SetActive(true);
        yield return new WaitForSeconds(2);
        _theBoss.SetActive(true);
        yield return new WaitForSeconds(2);
        _restartUI.SetActive(true);
        _returnToMainMenuUI.SetActive(true);
        onGameClear?.Invoke();
    }
}
