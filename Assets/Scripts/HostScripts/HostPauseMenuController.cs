﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HostPauseMenuController : MonoBehaviour
{
    public static HostPauseMenuController instance;

    private bool isPaused = false;
    [SerializeField] private GameObject pauseMenuPanel = null;
    [SerializeField] private GameObject multiplayerMenuPanel = null;
    //private bool multiplayerEnabled = false;
    [SerializeField] private Text serverStatusText = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists! Destroying object!");
            Destroy(transform.root.gameObject);
        }
    }

    private void Update()
    {
        CheckPauseButtonPressed();
    }

    private void CheckPauseButtonPressed()
    {
        if(Input.GetButtonDown("PauseGame"))
        {
            TogglePauseGame();
        }
    }

    private void TogglePauseGame()
    {
        if(isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        isPaused = true;
    }
    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
        multiplayerMenuPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;
    }

    #region PauseMenuPanel
    public void GoToMultiplayerMenu()
    {
        pauseMenuPanel.SetActive(false);
        multiplayerMenuPanel.SetActive(true);
    }

    public void QuitToMenu()
    {
        if(NetworkManager.instance.serverActive)
        {
            NetworkManager.instance.StopServer();
        }
        SceneManager.LoadScene("MainMenuScene");
    }
    #endregion

    #region MultiplayerMenuPanel
    public void EnableMultiplayer()
    {
        //Turn on server.
        Debug.Log("Activating Server...");
        //multiplayerEnabled = true;
        serverStatusText.text = "Enabled";
        NetworkManager.instance.StartServer(1);
    }

    public void GoToPauseMenu()
    {
        multiplayerMenuPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }
    #endregion
}
