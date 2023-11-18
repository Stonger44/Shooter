using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitApplication();
        }
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(1); // Game Scene
    }

    public void OpenControlsPanel()
    {
        
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
