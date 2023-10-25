﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text _score;
    [SerializeField] private Image _lives;
    [SerializeField] private Sprite[] _livesSpriteArray;
    [SerializeField] private GameObject _gameOverUI;
    [SerializeField] private GameObject _restartUI;
    [SerializeField] private float _gameOverBlinkTime = 0.5f;
    private bool _displayGameOver = false;
    private GameManager _gameManager;
    [SerializeField] private GameObject[] _shieldsArray;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("GameManager is null!");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateScore(int score)
    {
        _score.text = $"Score: {score}";
    }

    public void UpdateLives(int lives)
    {
        _lives.sprite = _livesSpriteArray[lives];

        if (lives < 1)
        {
            InitiateGameOverUI();
        }
    }

    public void UpdateShields(int shieldLevel)
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

    private void InitiateGameOverUI()
    {
        // Set game time back to normal in case SpeedBoost is active when the player dies
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        StartCoroutine(GameOverBlink());
        StartCoroutine(DisplayRestart());
    }

    private IEnumerator GameOverBlink()
    {
        while (true)
        {
            _displayGameOver = !_displayGameOver;
            _gameOverUI.SetActive(_displayGameOver);

            yield return new WaitForSeconds(_gameOverBlinkTime);
        }
    }

    private IEnumerator DisplayRestart()
    {
        yield return new WaitForSeconds(3);
        _restartUI.SetActive(true);
        _gameManager.GameOver();
    }
}
