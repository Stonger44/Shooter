using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text _score;
    [SerializeField] private Image _lives;
    [SerializeField] private Sprite[] _livesSpriteArray;
    [SerializeField] private GameObject _gameOver;
    [SerializeField] private float _gameOverFlickerTime = 0.5f;
    private bool _gameOverActive = false;

    // Start is called before the first frame update
    void Start()
    {

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
            // Set game time back to normal in case SpeedBoost is active when the player dies
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            StartCoroutine(GameOverFlicker(lives));
        }
    }

    private IEnumerator GameOverFlicker(int lives)
    {
        while (lives < 1)
        {
            _gameOverActive = !_gameOverActive;
            _gameOver.SetActive(_gameOverActive);

            yield return new WaitForSeconds(_gameOverFlickerTime);
        }
    }
}
