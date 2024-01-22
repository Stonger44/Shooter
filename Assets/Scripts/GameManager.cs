using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private Player _player;
    private AudioSource _bgmAudio;
    private SpawnManager _spawnManager;
    private UIManager _uiManager;

    [Header("Game State")]
    [SerializeField] private bool _gameOver = false;
    [SerializeField] private bool _gamePaused = false;
    [SerializeField] private bool _gameClear = false;
    [SerializeField] private bool _isTesting = false;

    private int _score = 0;

    [Header("Wave")]
    [SerializeField] private int _currentWave = 1;
    [SerializeField] private int _waveEnemyMultiplier = 5;
    [SerializeField] private int _waveEnemyTotalCount;
    [SerializeField] private int _enemiesRemaining;
    [SerializeField] private int _bossWave = 5;

    public static event Action onBossWaveFinalEnemy;

    private void OnEnable()
    {
        UIManager.onGameClear += SetGameClear;
    }

    private void OnDisable()
    {
        UIManager.onGameClear -= SetGameClear;
    }

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is null!");
        }
        _bgmAudio = GameObject.Find("BackgroundMusic").GetComponent<AudioSource>();
        if (_bgmAudio == null)
        {
            Debug.LogError("BGM Audio is null!");
        }
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("SpawnManager is null!");
        }
        _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("UIManager is null!");
        }

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        if (!_isTesting)
        {
            _waveEnemyTotalCount = CalculateWaveEnemyCount();
            _enemiesRemaining = _waveEnemyTotalCount;
            StartCoroutine(Startwave()); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitApplication();
        }

        if (_gamePaused || _gameOver || _gameClear)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
            
            if (Input.GetKeyDown(KeyCode.M))
            {
                ReturnToMainMenu(); 
            }
        }

        if (!_gameOver && Input.GetKeyDown(KeyCode.P) && _player.GetPlayerLives() > 0)
        {
            if (_gamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void UpdateScore(int points)
    {
        _score += points;
        _uiManager.UpdateScoreUI(_score);
    }

    public void UpdateEnemyCount()
    {
        _enemiesRemaining--;
        _uiManager.UpdateEnemyCount(_enemiesRemaining);

        if (IsBossWave())
        {
            if (_enemiesRemaining == 1)
            {
                onBossWaveFinalEnemy?.Invoke();
            }
            else if (_enemiesRemaining == 0)
            {
                // Display YOU WIN UI
                // Display/Activate Continue, Restart, and Main Menu buttons
            }
        }
        else
        {
            if (_enemiesRemaining == 0)
            {
                StartCoroutine(WaveCleared()); 
            }
        }
    }

    public bool IsBossWave()
    {
        return _currentWave == _bossWave;
    }

    public int GetCurrentWave()
    {
        return _currentWave;
    }

    public int GetWaveEnemyTotalCount()
    {
        return _waveEnemyTotalCount;
    }

    public void GameOver()
    {
        _gameOver = true;
    }

    public bool IsGameOver()
    {
        return _gameOver;
    }

    public void PauseBGM()
    {
        _bgmAudio.Pause();
    }

    private void SetGameClear()
    {
        _gameClear = true;
    }

    private void QuitApplication()
    {
        Application.Quit();
    }

    private IEnumerator Startwave()
    {
        StartCoroutine(_uiManager.UpdateWave(_currentWave, _waveEnemyTotalCount));
        yield return new WaitForSeconds(2f);
        _spawnManager.StartSpawning();
    }

    private IEnumerator WaveCleared()
    {
        StartCoroutine(_uiManager.WaveCleared(_currentWave));
        _currentWave++;
        _waveEnemyTotalCount = CalculateWaveEnemyCount();
        _enemiesRemaining = _waveEnemyTotalCount;
        yield return new WaitForSeconds(2f);
        StartCoroutine(Startwave());
    }

    private int CalculateWaveEnemyCount()
    {
        return _currentWave * _waveEnemyMultiplier;
    }

    private void PauseGame()
    {
        _gamePaused = true;
        Time.timeScale = 0;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        PauseBGM();
        _uiManager.TogglePausedUI();
    }

    private void ResumeGame()
    {
        _gamePaused = false;
        Time.timeScale = _player.IsSpeedBoostActive() ? _player.GetSpeedBoostTimeScale() : 1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        _bgmAudio.UnPause();
        _uiManager.TogglePausedUI();
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(1); // Game Scene
    }

    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene(0); // Main Menu Scene
    }

}
