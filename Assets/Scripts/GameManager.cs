using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool _gameOver = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_gameOver)
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
    }

    public void GameOver()
    {
        _gameOver = true;
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
