using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyHandler : MonoBehaviour
{
    int gameVersion = 0; // 0 == standard, 1 == abridged, ...

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
                Destroy(this);
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
