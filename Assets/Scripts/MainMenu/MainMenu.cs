using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _controlsPanel;

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
        _controlsPanel.SetActive(true);
    }

    public void CloseControlsPanel()
    {
        _controlsPanel.SetActive(false);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
