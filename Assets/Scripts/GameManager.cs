using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private Player _player;
    private AudioSource _bgmAudio;
    private UIManager _uiManager;

    [Header("Game State")]
    [SerializeField] private bool _gameOver = false;
    [SerializeField] private bool _gamePaused = false;

    private int _score = 0;

    [Header("Wave")]
    [SerializeField] private int _currentWave = 1;
    [SerializeField] private int _waveEnemyCount = 20;
    [SerializeField] private int _waveEnemyCountIncrease = 10;

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
        _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("UIManager is null!");
        }

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitApplication();
        }

        if (_gamePaused || _gameOver)
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
        PauseBGM();
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

    public void QuitApplication()
    {
        Application.Quit();
    }
}
