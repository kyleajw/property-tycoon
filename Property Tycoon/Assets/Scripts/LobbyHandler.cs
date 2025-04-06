using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyHandler : MonoBehaviour
{
    int gameVersion = 0; // 0 == standard, 1 == abridged, ...
    int gameDurationIndex = 0;
    private void Update()
    {

    }

    public void LoadGame()
    {
        gameObject.GetComponent<GameManager>().SetPlayers(FindFirstObjectByType<PlayersList>().GetPlayerCards());
        switch (gameVersion)
        {
            case 0:
                SceneManager.LoadScene("GameScene");
                gameObject.GetComponent<GameManager>().SetGameVersion(0);
                Destroy(this);
                break;
            case 1:
                SceneManager.LoadScene("GameScene");
                gameObject.GetComponent<GameManager>().SetGameVersion(1);
                switch (gameDurationIndex)
                {
                    case 0:
                        gameObject.GetComponent<GameManager>().SetGameDuration(30);
                        break;
                    case 1:
                        gameObject.GetComponent<GameManager>().SetGameDuration(600);
                        break;
                    case 2:
                        gameObject.GetComponent<GameManager>().SetGameDuration(1800);
                        break;
                    case 3:
                        gameObject.GetComponent<GameManager>().SetGameDuration(3600);
                        break;
                }
                Destroy(this);
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
    public void SetGameDuration(int duration)
    {
        gameDurationIndex = duration;
        Debug.Log($"game timer set to {gameDurationIndex}");
    } 

    void AddPlayerCardDataToPlayerList()
    {

    }
}
