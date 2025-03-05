using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyHandler : MonoBehaviour
{
    private static LobbyHandler _instance;
    public static LobbyHandler Instance
    {
        get { return _instance; }
    }

    List<GameObject> players;

    int gameVersion = 0; // 0 == standard, 1 == abridged, ...

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {

    }

    public void LoadGame()
    {
        switch (gameVersion)
        {
            case 0:
                SceneManager.LoadScene("GameScene");
                break;
            case 1:
                //load abridged
                break;
        }
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene("StartupScene");
        Destroy(gameObject);
    }

    public void SetGameVersion(int version)
    {
        gameVersion = version;
        Debug.Log($"game version set to {gameVersion}");
    }

    void AddPlayerCardDataToPlayerList()
    {

    }

}
