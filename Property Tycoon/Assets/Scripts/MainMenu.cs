using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// Loads the lobby scene
    /// </summary>
    public void LoadGame()
    {
        SceneManager.LoadScene("Lobby");
    }
    /// <summary>
    /// Quits the game
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }
}
